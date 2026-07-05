using Microsoft.Data.SqlClient;
using PropNest.Models;


namespace PropNestAPI
{
    public class PropertyUnitRepository
    {
        private readonly string _connectionString = string.Empty;

        public PropertyUnitRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public List<PropertyUnit> GetAll()
        {
            var list = new List<PropertyUnit>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM PropertyUnit", con);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(Map(r));
            return list;
        }

        public PropertyUnit? GetById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("SELECT * FROM PropertyUnit WHERE UnitID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        public int Add(PropertyUnit unit)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"INSERT INTO PropertyUnit(UnitNumber, PropertyType, FloorLevel, AreaSqFt, Amenities, Status, AskingRent, VacantSince)
                           VALUES(@UnitNumber, @PropertyType, @FloorLevel, @AreaSqFt, @Amenities, @Status, @AskingRent, @VacantSince);
                           SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, con);
            SetParams(cmd, unit);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(PropertyUnit unit)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            string sql = @"UPDATE PropertyUnit SET UnitNumber=@UnitNumber, PropertyType=@PropertyType, FloorLevel=@FloorLevel, AreaSqFt=@AreaSqFt, Amenities=@Amenities, Status=@Status, AskingRent=@AskingRent, VacantSince=@VacantSince
                           WHERE UnitID=@id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", unit.UnitID);
            SetParams(cmd, unit);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand("DELETE FROM PropertyUnit WHERE UnitID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private PropertyUnit Map(SqlDataReader r) => new PropertyUnit
        {
            UnitID = (int)r["UnitID"],
            UnitNumber = r["UnitNumber"].ToString()!,
            PropertyType = r["PropertyType"].ToString()!,
            FloorLevel = r["FloorLevel"] as string,
            AreaSqFt = r["AreaSqFt"] == DBNull.Value ? null : (decimal?)r["AreaSqFt"],
            Amenities = r["Amenities"] as string,
            Status = r["Status"].ToString()!,
            AskingRent = r["AskingRent"] == DBNull.Value ? 0m : (decimal)r["AskingRent"],
            VacantSince = r["VacantSince"] as DateTime?
        };

        private void SetParams(SqlCommand cmd, PropertyUnit unit)
        {
            cmd.Parameters.AddWithValue("@UnitNumber", unit.UnitNumber);
            cmd.Parameters.AddWithValue("@PropertyType", unit.PropertyType);
            cmd.Parameters.AddWithValue("@FloorLevel", (object?)unit.FloorLevel ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AreaSqFt", (object?)unit.AreaSqFt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Amenities", (object?)unit.Amenities ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", unit.Status);
            cmd.Parameters.AddWithValue("@AskingRent", unit.AskingRent);
            cmd.Parameters.AddWithValue("@VacantSince", (object?)unit.VacantSince ?? DBNull.Value);
        }

    }


}
