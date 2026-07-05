using Microsoft.Data.SqlClient;
using PropNest.Models;

namespace PropNestAPI
{
    public class StaffRepository
    {
        private readonly string _connectionString = string.Empty;

        public StaffRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Staff> GetAll()
        {
            var list = new List<Staff>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM Staff", con);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Map(r));
            return list;
        }

        public Staff? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM Staff WHERE StaffID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        public int Add(Staff s)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"INSERT INTO Staff(FullName, ContactNumber, Specialty, Status)
                           VALUES(@FullName, @ContactNumber, @Specialty, @Status);
                           SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, con);
            SetParams(cmd, s);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Staff s)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"UPDATE Staff SET FullName=@FullName, ContactNumber=@ContactNumber, Specialty=@Specialty, Status=@Status
                           WHERE StaffID=@StaffID";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@StaffID", s.StaffID);
            SetParams(cmd, s);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("DELETE FROM Staff WHERE StaffID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private Staff Map(SqlDataReader r) => new Staff
        {
            StaffID = (int)r["StaffID"],
            FullName = r["FullName"].ToString()!,
            ContactNumber = r["ContactNumber"] as string,
            Specialty = r["Specialty"] as string,
            Status = r["Status"].ToString()!
        };

        private void SetParams(SqlCommand cmd, Staff s)
        {
            cmd.Parameters.AddWithValue("@FullName", s.FullName);
            cmd.Parameters.AddWithValue("@ContactNumber", (object?)s.ContactNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Specialty", (object?)s.Specialty ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", s.Status);
        }
    }
}
