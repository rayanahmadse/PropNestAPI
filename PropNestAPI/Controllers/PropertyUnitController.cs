using Microsoft.AspNetCore.Mvc;
using PropNest.Models;

namespace PropNestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyUnitsController : ControllerBase
    {
        private readonly PropertyUnitRepository _repo;

        public PropertyUnitsController(PropertyUnitRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var unit = _repo.GetById(id);
            return unit == null ? NotFound() : Ok(unit);
        }

        [HttpPost]
        public IActionResult Create(PropertyUnit unit)
        {
            unit.UnitID = _repo.Add(unit);
            return Ok(unit);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, PropertyUnit unit)
        {
            if (_repo.GetById(id) == null) return NotFound();
            unit.UnitID = id;
            _repo.Update(unit);
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
