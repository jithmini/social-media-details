using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SocialMediaDetailAppBackend.BusinessLayer;
using SocialMediaDetailAppBackend.DataLayer;
using SocialMediaDetailAppBackend.Model;

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

        [HttpGet("userapps/{userId}")]
        public IActionResult GetUserApps(string userId)
        {
            var userApps = _detailsBL.GetUserApps(userId);
            if (userApps == null || userApps.Count == 0 || !userApps.Any())
            {
                return NotFound("No social media links found for this user.");
            }
            return Ok(userApps);
        }

        [HttpPost("updateDescription")]
        public IActionResult AddDescription([FromBody] UserAppDescription description)
        {
            if (description == null || string.IsNullOrWhiteSpace(description.Description))
            {
                return BadRequest("Invalid input.");
            }

            bool success = _detailsBL.AddDescription(description);
            return success ? Ok("Description added successfully.") : StatusCode(500, "Failed to add description.");
        }

        [HttpDelete("deleteDescription/{id}")]
        public IActionResult DeleteDescription(int id)
        {
            bool success = _detailsBL.DeleteDescription(id);
            return success ? Ok("Deleted successfully") : NotFound("Description not found.");
        }

        [HttpGet("getDescriptions/{userId}/{appName}")]
        public IActionResult GetDescriptions(string userId, string appName)
        {
            var descriptions = _detailsBL.GetDescriptions(userId, appName);
            return descriptions.Count > 0 ? Ok(descriptions) : NotFound("No descriptions found.");
        }

        [HttpPost("updateUserAppStatus")]
        public IActionResult updateUserAppStatus([FromBody] UserAppDescription description)
        {

            bool success = _detailsBL.updateUserAppStatus(description);
            return success ? Ok("Status updated successfully.") : StatusCode(500, "Failed to update status.");
        }
    }
}
