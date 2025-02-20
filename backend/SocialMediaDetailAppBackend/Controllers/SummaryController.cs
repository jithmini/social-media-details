using Microsoft.AspNetCore.Mvc;
using SocialMediaDetailAppBackend.BusinessLayer;
using SocialMediaDetailAppBackend.DataLayer;

namespace SocialMediaDetailAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryController : Controller
    {
        private readonly SummaryBL _summaryBL;

        public SummaryController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var summaryDL = new SummaryDL(connectionString);
            _summaryBL = new SummaryBL(summaryDL);
        }

        [HttpGet("userdetails")]
        public IActionResult GetUserDetails()
        {
            var userDetails = _summaryBL.GetUserDetails();
            return Ok(userDetails);
        }
    }
}
