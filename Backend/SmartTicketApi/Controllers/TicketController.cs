using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicketApi.Models.DTOs.Tickets;
using SmartTicketApi.Services.Tickets;
using System.Security.Claims;

namespace SmartTicketApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // EndUser: Create Ticket

        [Authorize(Roles = "EndUser")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserIdFromToken();

            var ticketId = await _ticketService.CreateTicketAsync(userId, dto);

            return Ok(new
            {
                Message = "Ticket created successfully",
                TicketId = ticketId
            });
        }


        // SupportManager: Assign Ticket

        [Authorize(Roles = "SupportManager")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignTicket([FromBody] AssignTicketDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ticketService.AssignTicketAsync(dto);

            return Ok(new
            {
                Message = "Ticket assigned successfully"
            });
        }


        // SupportAgent: Update Ticket Status

        [Authorize(Roles = "SupportAgent")]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateTicketStatus([FromBody] UpdateTicketStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ticketService.UpdateTicketStatusAsync(dto);

            return Ok(new
            {
                Message = "Ticket status updated successfully"
            });
        }


        // Admin: Update Ticket Priority

        [Authorize(Roles = "Admin")]
        [HttpPut("priority")]
        public async Task<IActionResult> UpdateTicketPriority(
            [FromQuery] int ticketId,
            [FromQuery] int ticketPriorityId)
        {
            await _ticketService.UpdateTicketPriorityAsync(ticketId, ticketPriorityId);

            return Ok(new
            {
                Message = "Ticket priority updated successfully"
            });
        }

        // Helper: Extract UserId from JWT

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("sub");

            return int.Parse(userIdClaim!.Value);
        }
    }
}
