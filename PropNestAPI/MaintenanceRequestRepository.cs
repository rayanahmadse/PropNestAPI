using Microsoft.Data.SqlClient;
using PropNest.Models;

namespace PropNestAPI
{
    public class MaintenanceRequestRepository
    {
        private readonly string _connectionString = string.Empty;

        public MaintenanceRequestRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<MaintenanceRequest> GetAll()
        {
            var list = new List<MaintenanceRequest>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM MaintenanceRequest", con);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Map(r));
            return list;
        }

        public MaintenanceRequest? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM MaintenanceRequest WHERE RequestID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        public int Add(MaintenanceRequest m)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"INSERT INTO MaintenanceRequest(UnitID, StaffID, Category, Description, DateLogged, DateResolved, Status)
                           VALUES(@UnitID, @StaffID, @Category, @Description, @DateLogged, @DateResolved, @Status);
                           SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, con);
            SetParams(cmd, m);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(MaintenanceRequest m)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"UPDATE MaintenanceRequest SET UnitID=@UnitID, StaffID=@StaffID, Category=@Category, Description=@Description,
                           DateLogged=@DateLogged, DateResolved=@DateResolved, Status=@Status
                           WHERE RequestID=@id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", m.RequestID);
            SetParams(cmd, m);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("DELETE FROM MaintenanceRequest WHERE RequestID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private MaintenanceRequest Map(SqlDataReader r) => new MaintenanceRequest
        {
            RequestID = (int)r["RequestID"],
            UnitID = (int)r["UnitID"],
            StaffID = r["StaffID"] == DBNull.Value ? null : (int?)r["StaffID"],
            Category = r["Category"] as string,
            Description = r["Description"] as string,
            DateLogged = r["DateLogged"] == DBNull.Value ? DateTime.MinValue : (DateTime)r["DateLogged"],
            DateResolved = r["DateResolved"] == DBNull.Value ? null : (DateTime?)r["DateResolved"],
            Status = r["Status"] as string ?? string.Empty
        };

        private void SetParams(SqlCommand cmd, MaintenanceRequest m)
        {
            cmd.Parameters.AddWithValue("@UnitID", m.UnitID);
            cmd.Parameters.AddWithValue("@StaffID", (object?)m.StaffID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Category", (object?)m.Category ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", (object?)m.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DateLogged", m.DateLogged);
            cmd.Parameters.AddWithValue("@DateResolved", (object?)m.DateResolved ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", m.Status);
        }

        // Get requests by unit ID
        public List<MaintenanceRequest> GetByUnitId(int unitId)
        {
            var list = new List<MaintenanceRequest>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM MaintenanceRequest WHERE UnitID=@unitId ORDER BY DateLogged DESC", con);
            cmd.Parameters.AddWithValue("@unitId", unitId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Map(r));
            return list;
        }

        // Get open requests by status
        public List<MaintenanceRequest> GetByStatus(string status)
        {
            var list = new List<MaintenanceRequest>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM MaintenanceRequest WHERE Status=@status ORDER BY DateLogged ASC", con);
            cmd.Parameters.AddWithValue("@status", status);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Map(r));
            return list;
        }

        // Get overdue open requests (open for 30+ days)
        public List<MaintenanceRequest> GetOverdueOpenRequests(int daysThreshold = 30)
        {
            var list = new List<MaintenanceRequest>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand(
                "SELECT * FROM MaintenanceRequest WHERE Status='Open' AND DATEDIFF(DAY, DateLogged, GETDATE()) >= @days ORDER BY DateLogged ASC", 
                con);
            cmd.Parameters.AddWithValue("@days", daysThreshold);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Map(r));
            return list;
        }

        // Auto-close old open requests (set status to 'Resolved' and set DateResolved)
        public int AutoCloseOldRequests(int daysThreshold = 30)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand(
                @"UPDATE MaintenanceRequest 
                  SET Status='Resolved', DateResolved=GETDATE() 
                  WHERE Status='Open' AND DATEDIFF(DAY, DateLogged, GETDATE()) >= @days",
                con);
            cmd.Parameters.AddWithValue("@days", daysThreshold);
            return cmd.ExecuteNonQuery();
        }

        // Check if a duplicate open request exists for the same unit and category
        public bool DuplicateOpenRequestExists(int unitId, string category)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM MaintenanceRequest WHERE UnitID=@unitId AND Category=@category AND Status='Open'",
                con);
            cmd.Parameters.AddWithValue("@unitId", unitId);
            cmd.Parameters.AddWithValue("@category", category);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}
