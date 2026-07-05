using Microsoft.AspNetCore.Mvc;
using PropNest.Models;

namespace PropNestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalAgreementsController : ControllerBase
    {
        private readonly RentalAgreementRepository _repo;
        private readonly RentPaymentRepository _paymentRepo;

        public RentalAgreementsController(RentalAgreementRepository repo, RentPaymentRepository paymentRepo)
        {
            _repo = repo;
            _paymentRepo = paymentRepo;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var a = _repo.GetById(id);
            return a == null ? NotFound() : Ok(a);
        }

        [HttpPost]
        public IActionResult Create(RentalAgreement a)
        {
            a.AgreementID = _repo.Add(a);
            return Ok(a);
        }

        [HttpPost("{agreementId}/generate-payments")]
        public IActionResult GeneratePayments(int agreementId, [FromQuery] int? paymentCount)
        {
            var agreement = _repo.GetById(agreementId);
            if (agreement == null) return NotFound();

            int count = paymentCount ?? 12;
            var payments = new List<RentPayment>();

            // Start from the first of the agreement's start month
            var start = new DateTime(agreement.StartDate.Year, agreement.StartDate.Month, 1);
            for (int i = 0; i < count; i++)
            {
                var due = start.AddMonths(i);
                if (_paymentRepo.ExistsForAgreementMonth(agreementId, due.Year, due.Month))
                    continue; // skip duplicates

                var p = new RentPayment
                {
                    AgreementID = agreementId,
                    DueDate = due,
                    PaymentDate = null,
                    AmountPaid = agreement.MonthlyRent,
                    PaymentMethod = "Pending",
                    Status = "Pending"
                };
                payments.Add(p);
            }

            var created = payments.Count > 0 ? _paymentRepo.AddPayments(payments) : 0;
            return Created($"api/RentalAgreements/{agreementId}/generate-payments", new { created });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, RentalAgreement a)
        {
            if (_repo.GetById(id) == null) return NotFound();
            a.AgreementID = id;
            _repo.Update(a);
            return NoContent();
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
