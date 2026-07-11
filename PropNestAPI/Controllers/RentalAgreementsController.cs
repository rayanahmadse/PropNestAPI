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
        private readonly TenantRepository _tenantRepo;

        public RentalAgreementsController(RentalAgreementRepository repo, RentPaymentRepository paymentRepo, TenantRepository tenantRepo)
        {
            _repo = repo;
            _paymentRepo = paymentRepo;
            _tenantRepo = tenantRepo;
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

            if (a.AgreementStatus == "Active")
            {
                _tenantRepo.UpdateStatus(a.TenantID, "Active");
            }
            else
            {
                var remaining = _repo.GetAll()
                    .Where(ag => ag.TenantID == a.TenantID && ag.AgreementID != a.AgreementID && ag.AgreementStatus == "Active")
                    .ToList();
                if (!remaining.Any())
                {
                    _tenantRepo.UpdateStatus(a.TenantID, "Inactive");
                }
            }

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
            var original = _repo.GetById(id);
            if (original == null) return NotFound();

            a.AgreementID = id;
            _repo.Update(a);

            // Re-evaluate tenant statuses:
            // Case 1: Tenant changed
            if (original.TenantID != a.TenantID)
            {
                // Activate new tenant
                if (a.AgreementStatus == "Active")
                {
                    _tenantRepo.UpdateStatus(a.TenantID, "Active");
                }

                // Check and deactivate old tenant
                var remainingForOld = _repo.GetAll()
                    .Where(ag => ag.TenantID == original.TenantID && ag.AgreementStatus == "Active")
                    .ToList();
                if (!remainingForOld.Any())
                {
                    _tenantRepo.UpdateStatus(original.TenantID, "Inactive");
                }
            }
            else
            {
                // Case 2: Tenant did not change, but status changed
                if (original.AgreementStatus != a.AgreementStatus)
                {
                    if (a.AgreementStatus == "Active")
                    {
                        _tenantRepo.UpdateStatus(a.TenantID, "Active");
                    }
                    else if (a.AgreementStatus == "Expired" || a.AgreementStatus == "Terminated")
                    {
                        var remainingForTenant = _repo.GetAll()
                            .Where(ag => ag.TenantID == a.TenantID && ag.AgreementID != id && ag.AgreementStatus == "Active")
                            .ToList();
                        if (!remainingForTenant.Any())
                        {
                            _tenantRepo.UpdateStatus(a.TenantID, "Inactive");
                        }
                    }
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var agreement = _repo.GetById(id);
            if (agreement == null) return NotFound();

            _repo.Delete(id);

            // Check if tenant still has any other active agreements
            var remaining = _repo.GetAll()
                .Where(ag => ag.TenantID == agreement.TenantID && ag.AgreementID != id && ag.AgreementStatus == "Active")
                .ToList();

            if (!remaining.Any())
            {
                // No more active agreements — set tenant back to Inactive
                _tenantRepo.UpdateStatus(agreement.TenantID, "Inactive");
            }

            return NoContent();
        }
    }
}
