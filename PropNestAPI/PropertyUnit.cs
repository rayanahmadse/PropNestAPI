namespace PropNest.Models
{
    public class PropertyUnit
    {
        public int UnitID { get; set; }
        public string UnitNumber { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string? FloorLevel { get; set; }
        public decimal? AreaSqFt { get; set; }
        public string? Amenities { get; set; }
        public string Status { get; set; } = "Vacant";
        public decimal AskingRent { get; set; }
        public DateTime? VacantSince { get; set; }
    }
}