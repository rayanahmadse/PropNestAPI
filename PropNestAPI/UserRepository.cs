using Microsoft.Data.SqlClient;

namespace PropNestAPI
{
    public class UserRepository
    {
        private readonly string _connectionString = string.Empty;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<User> GetAll()
        {
            var list = new List<User>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM Users", con);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Map(r));
            return list;
        }

        public User? GetByUsername(string username)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM Users WHERE Username = @Username", con);
            cmd.Parameters.AddWithValue("@Username", username);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        public int Add(User u)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"INSERT INTO Users (Username, Password, Role)
                           VALUES (@Username, @Password, @Role);
                           SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Username", u.Username);
            cmd.Parameters.AddWithValue("@Password", u.Password);
            cmd.Parameters.AddWithValue("@Role", u.Role);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private User Map(SqlDataReader r) => new User
        {
            UserID   = (int)r["UserID"],
            Username = r["Username"].ToString()!,
            Password = r["Password"].ToString()!,
            Role     = r["Role"].ToString()!
        };
    }
}
