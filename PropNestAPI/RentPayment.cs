namespace PropNest.Models
{
    public class RentPayment
    {
        public int PaymentID { get; set; }
        public int AgreementID { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string Status { get; set; } = "Pending";

        // Receipt metadata
        public string? ReceiptPath { get; set; }
        public bool ReceiptGenerated { get; set; } = false;

        // Stage 7: late fee and reminder tracking
        public decimal LateFeeAmount { get; set; } = 0m;
        public bool LateFeeApplied { get; set; } = false;
        public DateTime? ReminderSentAt { get; set; }
        public bool ReminderSent { get; set; } = false;

        public RentalAgreement? RentalAgreement { get; set; }
    }
}
