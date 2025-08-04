using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SocialMediaDetailAppBackend.BusinessLayer;
using SocialMediaDetailAppBackend.DataLayer;
using SocialMediaDetailAppBackend.Model;
using System.Net.Http;

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
            var summaryDL = new DetailsDL(connectionString, new HttpClient());
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
        public IActionResult updateUserAppStatus([FromBody] SubmitDescription description)
        {

            bool success = _detailsBL.updateUserAppStatus(description);
            return success ? Ok("Status updated successfully.") : StatusCode(500, "Failed to update status.");
        }

        //[HttpPost("send-webhook-accept")]
        //public IActionResult SendWebhookAccept([FromBody] WebhookPayload payload)
        //{
        //    bool success = _detailsBL.SendWebhookAcceptAsync(payload);
        //    return success
        //        ? Ok("Webhook sent successfully.")
        //        : StatusCode(500, "Failed to send webhook.");
        //}

        //[HttpPost("send-webhook-reject")]
        //public IActionResult SendWebhookReject([FromBody] WebhookPayload payload)
        //{
        //    bool success = _detailsBL.SendWebhookRejectAsync(payload);
        //    return success
        //        ? Ok("Webhook sent successfully.")
        //        : StatusCode(500, "Failed to send webhook.");
        //}

        

        [HttpGet("automated-links/{userId}")]
        public IActionResult GetAutomatedLinks(string userId)
        {
            try
            {
                var links = _detailsBL.GetAllLinks(userId);

                if (links == null || links.Count == 0)
                    return NotFound("No automated links found for the specified user.");

                return Ok(links);
            }
            catch (Exception ex)
            {
                // Log error here if needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("flagged-automated-links/details/{userId}")]
        public IActionResult GetFlaggedLinksWithDetails(string userId)
        {
            try
            {
                var data = _detailsBL.GetFlaggedLinksWithDetails(userId);

                if (data == null || !data.Any())
                    return NotFound("No flagged links or details found for this user.");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost("delete-automated-link")]
        public IActionResult DeleteAutomatedLink([FromBody] DeleteLinkRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Link))
            {
                return BadRequest("UserId and Link must be provided.");
            }

            bool success = _detailsBL.DeleteAutomatedLink(request.UserId, request.Link);
            return success ? Ok("Link deleted successfully.") : NotFound("Link not found.");
        }

        [HttpPost("updateReasoning")]
        public IActionResult UpdateReasoning([FromBody] UpdateReasoningRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) ||
                string.IsNullOrWhiteSpace(request.Url) ||
                string.IsNullOrWhiteSpace(request.Reasoning))
            {
                return BadRequest("All fields are required.");
            }

            bool success = _detailsBL.UpdateReasoning(request);
            return success
                ? Ok("Reasoning updated successfully.")
                : NotFound("Matching record not found or update failed.");
        }

        [HttpPost("send-webhook")]
        public async Task<IActionResult> SendWebhook([FromBody] UserDecisionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrWhiteSpace(request.Result))
                return BadRequest("Invalid payload");

            try
            {
                var result = await _detailsBL.SendWebhookAsync(request.Id, request.Result);

                if (result.IsSuccess)
                    return Ok(new { message = "Webhook sent", payload = result.Payload });
                else if (result.NotFound)
                    return NotFound(result.ErrorMessage);
                else
                    return StatusCode(500, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                // Log error if needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
