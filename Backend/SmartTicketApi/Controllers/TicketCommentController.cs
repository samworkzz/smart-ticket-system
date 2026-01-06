using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicketApi.Models.DTOs.TicketComments;
using SmartTicketApi.Services.TicketComments;
using System.Security.Claims;

namespace SmartTicketApi.Controllers
{
    [ApiController]
    [Route("api/ticket-comments")]
    [Authorize]
    public class TicketCommentController : ControllerBase
    {
        private readonly ITicketCommentService _commentService;

        public TicketCommentController(ITicketCommentService commentService)
        {
            _commentService = commentService;
        }

        // EndUser & SupportAgent can comment
        // EndUser, SupportAgent & SupportManager can comment
        [Authorize(Roles = "EndUser,SupportAgent,SupportManager")]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] AddTicketCommentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserIdFromToken();

            await _commentService.AddCommentAsync(userId, dto);

            return Ok(new
            {
                Message = "Comment added successfully"
            });
        }

        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                        ?? User.FindFirst("sub");

            return int.Parse(claim!.Value);
        }
    }
}
