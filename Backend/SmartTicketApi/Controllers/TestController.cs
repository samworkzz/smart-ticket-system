using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartTicketApi.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        // Any logged-in user
        [Authorize]
        [HttpGet("authenticated")]
        public IActionResult Authenticated()
        {
            return Ok("You are authenticated");
        }

        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            return Ok("Admin access granted");
        }

        // Support Manager only
        [Authorize(Roles = "SupportManager")]
        [HttpGet("manager")]
        public IActionResult ManagerOnly()
        {
            return Ok("Support Manager access granted");
        }
    }
}
