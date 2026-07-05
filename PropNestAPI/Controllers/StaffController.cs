using Microsoft.AspNetCore.Mvc;
using PropNest.Models;

namespace PropNestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly StaffRepository _repo;

        public StaffController(StaffRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var s = _repo.GetById(id);
            return s == null ? NotFound() : Ok(s);
        }

        [HttpPost]
        public IActionResult Create(Staff s)
        {
            s.StaffID = _repo.Add(s);
            return Ok(s);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Staff s)
        {
            if (_repo.GetById(id) == null) return NotFound();
            s.StaffID = id;
            _repo.Update(s);
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
