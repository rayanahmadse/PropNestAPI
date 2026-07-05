using Microsoft.AspNetCore.Mvc;
using PropNest.Models;

namespace PropNestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceRequestsController : ControllerBase
    {
        private readonly MaintenanceRequestRepository _repo;
        private readonly PropertyUnitRepository _unitRepo;

        public MaintenanceRequestsController(MaintenanceRequestRepository repo, PropertyUnitRepository unitRepo)
        {
            _repo = repo;
            _unitRepo = unitRepo;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var m = _repo.GetById(id);
            return m == null ? NotFound() : Ok(m);
        }

        [HttpGet("by-unit/{unitId}")]
        public IActionResult GetByUnit(int unitId)
        {
            var requests = _repo.GetByUnitId(unitId);
            return Ok(requests);
        }

        [HttpGet("by-status/{status}")]
        public IActionResult GetByStatus(string status)
        {
            var requests = _repo.GetByStatus(status);
            return Ok(requests);
        }

        [HttpGet("overdue-open")]
        public IActionResult GetOverdueOpen([FromQuery] int? days)
        {
            var threshold = days ?? 30;
            var requests = _repo.GetOverdueOpenRequests(threshold);
            return Ok(requests);
        }

        [HttpPost]
        public IActionResult Create(MaintenanceRequest m)
        {
            m.RequestID = _repo.Add(m);

            // If the request is Open, mark the unit as UnderMaintenance
            try
            {
                if (string.Equals(m.Status, "Open", StringComparison.OrdinalIgnoreCase))
                {
                    var unit = _unitRepo.GetById(m.UnitID);
                    if (unit != null && unit.Status != "UnderMaintenance")
                    {
                        unit.Status = "UnderMaintenance";
                        _unitRepo.Update(unit);
                    }
                }
            }
            catch
            {
                // Ignore failures to avoid breaking request creation
            }

            return Ok(m);
        }

        [HttpPost("auto-close-old")]
        public IActionResult AutoCloseOld([FromQuery] int? days)
        {
            var threshold = days ?? 30;
            var closed = _repo.AutoCloseOldRequests(threshold);
            return Ok(new { closed });
        }

        [HttpPost("check-duplicate")]
        public IActionResult CheckDuplicate([FromQuery] int unitId, [FromQuery] string category)
        {
            if (string.IsNullOrEmpty(category))
                return BadRequest("Category is required");

            var exists = _repo.DuplicateOpenRequestExists(unitId, category);
            return Ok(new { exists });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, MaintenanceRequest m)
        {
            if (_repo.GetById(id) == null) return NotFound();
            m.RequestID = id;
            _repo.Update(m);
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
