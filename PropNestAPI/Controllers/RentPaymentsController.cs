using Microsoft.AspNetCore.Mvc;
using PropNest.Models;
using PropNestAPI.Services;

namespace PropNestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentPaymentsController : ControllerBase
    {
        private readonly RentPaymentRepository _repo;
        private readonly RentalAgreementRepository _agreementRepo;
        private readonly TenantRepository _tenantRepo;
        private readonly PropertyUnitRepository _unitRepo;
        private readonly PdfReceiptGenerator _pdfGen;
        private readonly IWebHostEnvironment _env;

        public RentPaymentsController(RentPaymentRepository repo, RentalAgreementRepository agreementRepo, TenantRepository tenantRepo, PropertyUnitRepository unitRepo, PdfReceiptGenerator pdfGen, IWebHostEnvironment env)
        {
            _repo = repo;
            _agreementRepo = agreementRepo;
            _tenantRepo = tenantRepo;
            _unitRepo = unitRepo;
            _pdfGen = pdfGen;
            _env = env;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var p = _repo.GetById(id);
            return p == null ? NotFound() : Ok(p);
        }

        [HttpPost]
        public IActionResult Create(RentPayment p)
        {
            p.PaymentID = _repo.Add(p);
            return Ok(p);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, RentPayment p)
        {
            if (_repo.GetById(id) == null) return NotFound();
            p.PaymentID = id;
            _repo.Update(p);
            return NoContent();
        }

        [HttpPut("{id}/mark-overdue")]
        public IActionResult MarkOverdue(int id)
        {
            var updated = _repo.MarkOverdueIfDueAndUnpaid(id);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpPost("check-overdue")]
        public IActionResult CheckOverdue()
        {
            var count = _repo.MarkOverdueForDueBefore(DateTime.Today);
            return Ok(new { marked = count });
        }

        [HttpPost("apply-late-fees")]
        public IActionResult ApplyLateFees([FromQuery] int graceDays = 5, [FromQuery] decimal lateFeeRate = 0.05m)
        {
            var updated = _repo.ApplyLateFees(graceDays, lateFeeRate);
            return Ok(new { updated });
        }

        [HttpPost("{id}/generate-receipt")]
        public IActionResult GenerateReceipt(int id)
        {
            var payment = _repo.GetById(id);
            if (payment == null) return NotFound();
            if (payment.ReceiptGenerated) return Ok(new { path = payment.ReceiptPath });

            var agreement = _agreementRepo.GetById(payment.AgreementID);
            if (agreement == null) return BadRequest(new { error = "Associated rental agreement not found." });

            var tenant = _tenantRepo.GetById(agreement.TenantID);
            var unit = _unitRepo.GetById(agreement.UnitID);

            var bytes = _pdfGen.Generate(payment, agreement, tenant, unit);

            var receiptsDir = Path.Combine(_env.ContentRootPath, "wwwroot", "receipts");
            if (!Directory.Exists(receiptsDir)) Directory.CreateDirectory(receiptsDir);
            var filename = $"receipt_{payment.PaymentID}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var fullPath = Path.Combine(receiptsDir, filename);
            System.IO.File.WriteAllBytes(fullPath, bytes);

            // Update payment
            payment.ReceiptPath = $"/receipts/{filename}";
            payment.ReceiptGenerated = true;
            _repo.Update(payment);

            return Created($"api/RentPayments/{id}/receipt", new { path = payment.ReceiptPath });
        }

        [HttpGet("due-reminders")]
        public IActionResult GetDueReminders([FromQuery] int reminderDays = 3)
        {
            var reminders = _repo.GetReminderCandidates(reminderDays);
            return Ok(reminders);
        }

        [HttpPut("{id}/send-reminder")]
        public IActionResult SendReminder(int id)
        {
            var payment = _repo.MarkReminderSent(id);
            return payment == null ? NotFound() : Ok(payment);
        }

        [HttpGet("{id}/receipt")]
        public IActionResult GetReceipt(int id)
        {
            var payment = _repo.GetById(id);
            if (payment == null || string.IsNullOrEmpty(payment.ReceiptPath)) return NotFound();
            var fullPath = Path.Combine(_env.ContentRootPath, "wwwroot", payment.ReceiptPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(fullPath)) return NotFound();
            var bytes = System.IO.File.ReadAllBytes(fullPath);
            return File(bytes, "application/pdf", Path.GetFileName(fullPath));
        }

        [HttpPost("run-stage7-automation")]
        public IActionResult RunStage7Automation([FromQuery] int graceDays = 5, [FromQuery] decimal lateFeeRate = 0.05m, [FromQuery] int reminderDays = 3)
        {
            var overdue = _repo.MarkOverdueForDueBefore(DateTime.Today);
            var lateFees = _repo.ApplyLateFees(graceDays, lateFeeRate);
            var reminders = _repo.GetReminderCandidates(reminderDays);
            return Ok(new { overdue, lateFees, reminderCount = reminders.Count });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_repo.GetById(id) == null) return NotFound();
            _repo.Delete(id);
            return NoContent();
        }
    }
}
