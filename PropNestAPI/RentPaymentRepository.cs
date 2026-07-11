using Microsoft.Data.SqlClient;
using PropNest.Models;

namespace PropNestAPI
{
    public class RentPaymentRepository
    {
        private readonly string _connectionString = string.Empty;

        public RentPaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<RentPayment> GetAll()
        {
            var list = new List<RentPayment>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM RentPayment", con);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Map(r));
            return list;
        }

        public RentPayment? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM RentPayment WHERE PaymentID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        public int Add(RentPayment p)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"INSERT INTO RentPayment(AgreementID, PaymentDate, DueDate, AmountPaid, PaymentMethod, Status, ReceiptPath, ReceiptGenerated, LateFeeAmount, LateFeeApplied, ReminderSentAt, ReminderSent)
                           VALUES(@AgreementID, @PaymentDate, @DueDate, @AmountPaid, @PaymentMethod, @Status, @ReceiptPath, @ReceiptGenerated, @LateFeeAmount, @LateFeeApplied, @ReminderSentAt, @ReminderSent);
                           SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, con);
            SetParams(cmd, p);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(RentPayment p)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"UPDATE RentPayment SET AgreementID=@AgreementID, PaymentDate=@PaymentDate, DueDate=@DueDate,
                           AmountPaid=@AmountPaid, PaymentMethod=@PaymentMethod, Status=@Status, ReceiptPath=@ReceiptPath, ReceiptGenerated=@ReceiptGenerated,
                           LateFeeAmount=@LateFeeAmount, LateFeeApplied=@LateFeeApplied, ReminderSentAt=@ReminderSentAt, ReminderSent=@ReminderSent
                           WHERE PaymentID=@id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", p.PaymentID);
            SetParams(cmd, p);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("DELETE FROM RentPayment WHERE PaymentID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // Check if a payment exists for the same agreement and month
        public bool ExistsForAgreementMonth(int agreementId, int year, int month)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM RentPayment WHERE AgreementID=@agreementId AND YEAR(DueDate)=@year AND MONTH(DueDate)=@month", con);
            cmd.Parameters.AddWithValue("@agreementId", agreementId);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@month", month);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        // Add multiple payments in a single transaction
        public int AddPayments(IEnumerable<RentPayment> payments)
        {
            int created = 0;
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var tx = con.BeginTransaction();
            try
            {
                foreach (var p in payments)
                {
                    string sql = @"INSERT INTO RentPayment(AgreementID, PaymentDate, DueDate, AmountPaid, PaymentMethod, Status, ReceiptPath, ReceiptGenerated, LateFeeAmount, LateFeeApplied, ReminderSentAt, ReminderSent)
                                       VALUES(@AgreementID, @PaymentDate, @DueDate, @AmountPaid, @PaymentMethod, @Status, @ReceiptPath, @ReceiptGenerated, @LateFeeAmount, @LateFeeApplied, @ReminderSentAt, @ReminderSent);
                                       SELECT SCOPE_IDENTITY();";
                    using var cmd = new SqlCommand(sql, con, tx);
                    SetParams(cmd, p);
                    var id = Convert.ToInt32(cmd.ExecuteScalar());
                    if (id > 0) created++;
                }
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
            return created;
        }

        // Get pending payments
        public List<RentPayment> GetPendingPayments()
        {
            var list = new List<RentPayment>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM RentPayment WHERE Status='Pending'", con);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(Map(r));
            return list;
        }

        // Mark overdue payments before a given date
        public int MarkOverdueForDueBefore(DateTime date)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("UPDATE RentPayment SET Status='Overdue' WHERE Status='Pending' AND DueDate < @date", con);
            cmd.Parameters.AddWithValue("@date", date.Date);
            return cmd.ExecuteNonQuery();
        }

        public int ApplyLateFees(int graceDays = 5, decimal lateFeeRate = 0.05m)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand(@"
                UPDATE RentPayment
                SET LateFeeAmount = ROUND(AmountPaid * @rate, 2),
                    LateFeeApplied = 1
                WHERE Status = 'Overdue'
                  AND LateFeeApplied = 0
                  AND PaymentDate IS NULL
                  AND DueDate < DATEADD(DAY, -@graceDays, GETDATE())", con);
            cmd.Parameters.AddWithValue("@graceDays", graceDays);
            cmd.Parameters.AddWithValue("@rate", lateFeeRate);
            return cmd.ExecuteNonQuery();
        }

        public List<RentPayment> GetReminderCandidates(int reminderDays = 3)
        {
            var list = new List<RentPayment>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT * FROM RentPayment
                WHERE PaymentDate IS NULL
                  AND ReminderSent = 0
                  AND DueDate <= DATEADD(DAY, @days, GETDATE())
                ORDER BY DueDate ASC", con);
            cmd.Parameters.AddWithValue("@days", reminderDays);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(Map(r));
            return list;
        }

        public RentPayment? MarkReminderSent(int id)
        {
            var p = GetById(id);
            if (p == null) return null;
            p.ReminderSent = true;
            p.ReminderSentAt = DateTime.Now;
            Update(p);
            return p;
        }

        // Mark a single payment overdue if unpaid and past due
        public RentPayment? MarkOverdueIfDueAndUnpaid(int id)
        {
            var p = GetById(id);
            if (p == null) return null;
            if (p.PaymentDate == null && p.DueDate < DateTime.Today && p.Status != "Overdue")
            {
                p.Status = "Overdue";
                Update(p);
            }
            return p;
        }

        private static bool HasColumn(SqlDataReader r, string name)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (r.GetName(i).Equals(name, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private RentPayment Map(SqlDataReader r) => new RentPayment
        {
            PaymentID     = (int)r["PaymentID"],
            AgreementID   = (int)r["AgreementID"],
            PaymentDate   = r["PaymentDate"] == DBNull.Value ? null : (DateTime?)r["PaymentDate"],
            DueDate       = r["DueDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)r["DueDate"],
            AmountPaid    = r["AmountPaid"] == DBNull.Value ? 0m : (decimal)r["AmountPaid"],
            PaymentMethod = r["PaymentMethod"] as string ?? string.Empty,
            Status        = r["Status"] as string ?? string.Empty,
            ReceiptPath      = HasColumn(r, "ReceiptPath")      && r["ReceiptPath"]      != DBNull.Value ? r["ReceiptPath"] as string : null,
            ReceiptGenerated = HasColumn(r, "ReceiptGenerated") && r["ReceiptGenerated"] != DBNull.Value ? (bool)r["ReceiptGenerated"] : false,
            LateFeeAmount    = HasColumn(r, "LateFeeAmount")    && r["LateFeeAmount"]    != DBNull.Value ? (decimal)r["LateFeeAmount"] : 0m,
            LateFeeApplied   = HasColumn(r, "LateFeeApplied")   && r["LateFeeApplied"]   != DBNull.Value ? (bool)r["LateFeeApplied"] : false,
            ReminderSentAt   = HasColumn(r, "ReminderSentAt")   && r["ReminderSentAt"]   != DBNull.Value ? (DateTime?)r["ReminderSentAt"] : null,
            ReminderSent     = HasColumn(r, "ReminderSent")     && r["ReminderSent"]     != DBNull.Value ? (bool)r["ReminderSent"] : false
        };

        private void SetParams(SqlCommand cmd, RentPayment p)
        {
            cmd.Parameters.AddWithValue("@AgreementID", p.AgreementID);
            cmd.Parameters.AddWithValue("@PaymentDate", (object?)p.PaymentDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DueDate", p.DueDate);
            cmd.Parameters.AddWithValue("@AmountPaid", p.AmountPaid);
            cmd.Parameters.AddWithValue("@PaymentMethod", p.PaymentMethod);
            cmd.Parameters.AddWithValue("@Status", p.Status);
            cmd.Parameters.AddWithValue("@ReceiptPath", (object?)p.ReceiptPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReceiptGenerated", p.ReceiptGenerated);
            cmd.Parameters.AddWithValue("@LateFeeAmount", p.LateFeeAmount);
            cmd.Parameters.AddWithValue("@LateFeeApplied", p.LateFeeApplied);
            cmd.Parameters.AddWithValue("@ReminderSentAt", (object?)p.ReminderSentAt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReminderSent", p.ReminderSent);
        }
    }
}
