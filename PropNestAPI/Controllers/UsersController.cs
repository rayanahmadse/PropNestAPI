using Microsoft.AspNetCore.Mvc;

namespace PropNestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _repo;

        public UsersController(UserRepository repo)
        {
            _repo = repo;
        }

        // GET api/Users — list all users (for admin panel)
        [HttpGet]
        public IActionResult GetAll() => Ok(_repo.GetAll());

        // POST api/Users/register — create a new user account
        [HttpPost("register")]
        public IActionResult Register(User u)
        {
            // Check if username already exists
            var existing = _repo.GetByUsername(u.Username);
            if (existing != null)
                return BadRequest("Username already taken.");

            u.UserID = _repo.Add(u);
            return Ok(u);
        }

        // POST api/Users/login — check credentials
        [HttpPost("login")]
        public IActionResult Login([FromBody] User loginRequest)
        {
            var user = _repo.GetByUsername(loginRequest.Username);

            if (user == null || user.Password != loginRequest.Password)
                return Unauthorized("Invalid username or password.");

            // Return user info (no password sent back)
            return Ok(new { user.UserID, user.Username, user.Role });
        }
    }
}
