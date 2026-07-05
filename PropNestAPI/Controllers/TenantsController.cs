using Microsoft.AspNetCore.Mvc;
using PropNest.Models;

namespace PropNestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private readonly TenantRepository _repo;

        public TenantsController(TenantRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var t = _repo.GetById(id);
            return t == null ? NotFound() : Ok(t);
        }

        [HttpPost]
        public IActionResult Create(Tenant t)
        {
            t.TenantID = _repo.Add(t);
            return Ok(t);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Tenant t)
        {
            if (_repo.GetById(id) == null) return NotFound();
            t.TenantID = id;
            _repo.Update(t);
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