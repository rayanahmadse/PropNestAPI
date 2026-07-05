namespace PropNest.Models
{
    public class MaintenanceRequest
    {
        public int RequestID { get; set; }
        public int UnitID { get; set; }
        public int? StaffID { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public DateTime DateLogged { get; set; } = DateTime.Now;
        public DateTime? DateResolved { get; set; }
        public string Status { get; set; } = "Open";

        // Navigation properties
        public PropertyUnit? PropertyUnit { get; set; }
        public Staff? Staff { get; set; }
    }
}