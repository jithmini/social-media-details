using Microsoft.AspNetCore.Mvc;
using SocialMediaDetailAppBackend.BusinessLayer;
using SocialMediaDetailAppBackend.DataLayer;

namespace SocialMediaDetailAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : Controller
    {
        private readonly DetailsBL _detailsBL;

        public DetailsController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var summaryDL = new DetailsDL(connectionString);
            _detailsBL = new DetailsBL(summaryDL);
        }
        [HttpGet("userdetails/{userId}")]
        public IActionResult GetUserDetailById(string userId)
        {
            var userDetail = _detailsBL.GetUserDetailById(userId);
            if (userDetail == null)
            {
                return NotFound();
            }
            return Ok(userDetail);
        }
    }
}
