namespace PropNest.Models
{
    public class Tenant
    {
        public int TenantID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? EmergencyContact { get; set; }
        public string Status { get; set; } = "Active";
    }
}