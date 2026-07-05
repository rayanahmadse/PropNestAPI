namespace PropNest.Models
{
    public class Staff
    {
        public int StaffID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? Specialty { get; set; }
        public string Status { get; set; } = "Active";
    }
}