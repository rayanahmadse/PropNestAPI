using Microsoft.Data.SqlClient;
using PropNest.Models;

namespace PropNestAPI
{
    public class RentalAgreementRepository
    {
        private readonly string _connectionString = string.Empty;

        public RentalAgreementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<RentalAgreement> GetAll()
        {
            var list = new List<RentalAgreement>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM RentalAgreement", con);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Map(r));
            return list;
        }

        public RentalAgreement? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM RentalAgreement WHERE AgreementID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        public int Add(RentalAgreement a)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"INSERT INTO RentalAgreement(TenantID, UnitID, StartDate, EndDate, MonthlyRent, SecurityDeposit, AgreementStatus, Version)
                           VALUES(@TenantID, @UnitID, @StartDate, @EndDate, @MonthlyRent, @SecurityDeposit, @AgreementStatus, @Version);
                           SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, con);
            SetParams(cmd, a);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(RentalAgreement a)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"UPDATE RentalAgreement SET TenantID=@TenantID, UnitID=@UnitID, StartDate=@StartDate, EndDate=@EndDate,
                           MonthlyRent=@MonthlyRent, SecurityDeposit=@SecurityDeposit, AgreementStatus=@AgreementStatus, Version=@Version
                           WHERE AgreementID=@id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", a.AgreementID);
            SetParams(cmd, a);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("DELETE FROM RentalAgreement WHERE AgreementID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private RentalAgreement Map(SqlDataReader r) => new RentalAgreement
        {
            AgreementID = (int)r["AgreementID"],
            TenantID = (int)r["TenantID"],
            UnitID = (int)r["UnitID"],
            StartDate = r["StartDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)r["StartDate"],
            EndDate = r["EndDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)r["EndDate"],
            MonthlyRent = r["MonthlyRent"] == DBNull.Value ? 0m : (decimal)r["MonthlyRent"],
            SecurityDeposit = r["SecurityDeposit"] == DBNull.Value ? 0m : (decimal)r["SecurityDeposit"],
            AgreementStatus = r["AgreementStatus"] as string ?? string.Empty,
            Version = r["Version"] == DBNull.Value ? 1 : (int)r["Version"]
        };

        private void SetParams(SqlCommand cmd, RentalAgreement a)
        {
            cmd.Parameters.AddWithValue("@TenantID", a.TenantID);
            cmd.Parameters.AddWithValue("@UnitID", a.UnitID);
            cmd.Parameters.AddWithValue("@StartDate", a.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", a.EndDate);
            cmd.Parameters.AddWithValue("@MonthlyRent", a.MonthlyRent);
            cmd.Parameters.AddWithValue("@SecurityDeposit", a.SecurityDeposit);
            cmd.Parameters.AddWithValue("@AgreementStatus", a.AgreementStatus);
            cmd.Parameters.AddWithValue("@Version", a.Version);
        }
    }
}
