using Microsoft.Data.SqlClient;
using PropNest.Models;

namespace PropNestAPI
{
    public class TenantRepository
    {
        private readonly string _connectionString = string.Empty;

        public TenantRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Tenant> GetAll()
        {
            var list = new List<Tenant>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM Tenant", con);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Map(r));
            return list;
        }

        public Tenant? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM Tenant WHERE TenantID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        public int Add(Tenant t)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"INSERT INTO Tenant(FullName,CNIC,Email,ContactNumber,EmergencyContact,Status)
                           VALUES(@FullName,@CNIC,@Email,@ContactNumber,@EmergencyContact,@Status);
                           SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, con);
            SetParams(cmd, t);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Tenant t)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"UPDATE Tenant SET FullName=@FullName, CNIC=@CNIC, Email=@Email,
                           ContactNumber=@ContactNumber, EmergencyContact=@EmergencyContact,
                           Status=@Status WHERE TenantID=@id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", t.TenantID);
            SetParams(cmd, t);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("DELETE FROM Tenant WHERE TenantID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private Tenant Map(SqlDataReader r) => new Tenant
        {
            TenantID = (int)r["TenantID"],
            FullName = r["FullName"].ToString()!,
            CNIC = r["CNIC"].ToString()!,
            Email = r["Email"] as string,
            ContactNumber = r["ContactNumber"] as string,
            EmergencyContact = r["EmergencyContact"] as string,
            Status = r["Status"].ToString()!
        };

        private void SetParams(SqlCommand cmd, Tenant t)
        {
            cmd.Parameters.AddWithValue("@FullName", t.FullName);
            cmd.Parameters.AddWithValue("@CNIC", t.CNIC);
            cmd.Parameters.AddWithValue("@Email", (object?)t.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactNumber", (object?)t.ContactNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmergencyContact", (object?)t.EmergencyContact ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", t.Status);
        }
    }
}