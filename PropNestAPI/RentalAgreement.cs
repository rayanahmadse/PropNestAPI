namespace PropNest.Models
{
    public class RentalAgreement
    {
        public int AgreementID { get; set; }
        public int TenantID { get; set; }
        public int UnitID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal SecurityDeposit { get; set; } = 0;
        public string AgreementStatus { get; set; } = "Active";
        public int Version { get; set; } = 1;

        public Tenant? Tenant { get; set; }
        public PropertyUnit? PropertyUnit { get; set; }
    }
}