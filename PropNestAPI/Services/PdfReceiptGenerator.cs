using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PropNest.Models;

namespace PropNestAPI.Services
{
    public class PdfReceiptGenerator
    {
        public byte[] Generate(RentPayment payment, RentalAgreement agreement, Tenant tenant, PropertyUnit unit)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Receipt #{payment.PaymentID}").FontSize(20).Bold();
                        col.Item().Text($"Date: {DateTime.Now:d}");
                        col.Item().Text($"Tenant: {tenant.FullName}");
                        col.Item().Text($"Unit: {unit.UnitNumber}");
                        col.Item().Text($"Agreement: {agreement.AgreementID}");
                        col.Item().Text($"Payment Date: {(payment.PaymentDate?.ToString("d") ?? "(not paid)")}");
                        col.Item().Text($"Amount: {payment.AmountPaid:C}");
                        col.Item().Text($"Method: {payment.PaymentMethod}");
                    });
                });
            });

            using var ms = new MemoryStream();
            doc.GeneratePdf(ms);
            return ms.ToArray();
        }
    }
}
