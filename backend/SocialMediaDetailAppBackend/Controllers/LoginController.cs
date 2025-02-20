using Microsoft.AspNetCore.Mvc;
using SocialMediaDetailAppBackend.BusinessLayer;
using SocialMediaDetailAppBackend.DataLayer;
using SocialMediaDetailAppBackend.Model;

namespace SocialMediaDetailAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly LoginBL _loginBL;

        public LoginController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var loginDL = new LoginDL(connectionString);
            _loginBL = new LoginBL(loginDL);
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {            
            if (_loginBL.ValidateUser(loginRequest.Username, loginRequest.Password))
            {
                return Ok(new { message = "Login Successful" });
            }
            else
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_loginBL.RegisterUser(loginRequest.Username, loginRequest.Password))
            {
                return Ok(new { message = "Registration Successful" });
            }
            else
            {
                return BadRequest(new { message = "Registration failed" });
            }
        }
    }
}
