using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicketApi.Models.DTOs.Agent;
using SmartTicketApi.Models.DTOs.Manager;
using SmartTicketApi.Models.DTOs.Tickets;
using SmartTicketApi.Services.Tickets;
using SmartTicketApi.Models.DTOs.Shared;
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

        // Shared: Get Ticket Details
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var userId = GetUserIdFromToken();
            var role = User.FindFirstValue(ClaimTypes.Role);

            var ticket = await _ticketService.GetTicketDetailsAsync(id, userId, role!);

            if (ticket == null)
                return NotFound("Ticket not found or access denied.");

            return Ok(ticket);
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





        // SupportAgent: Update Ticket Status

        [Authorize(Roles = "SupportAgent,SupportManager")]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateTicketStatus([FromBody] UpdateTicketStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = User.FindFirstValue(ClaimTypes.Role) ?? "EndUser";

            try 
            {
                await _ticketService.UpdateTicketStatusAsync(dto, role);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

            return Ok(new
            {
                Message = "Ticket status updated successfully"
            });
        }


        // Admin: Update Ticket Priority

        [Authorize(Roles = "Admin,SupportManager")]
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
        // END USER
        [Authorize(Roles = "EndUser")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTickets([FromQuery] PagedRequestDto pagination)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var tickets = await _ticketService.GetTicketsForEndUserAsync(userId, pagination);
            return Ok(tickets);
        }

        // SUPPORT AGENT
        [Authorize(Roles = "SupportAgent")]
        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedTickets([FromQuery] PagedRequestDto pagination)
        {
            var agentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var tickets = await _ticketService.GetTicketsForAgentAsync(agentId, pagination);
            return Ok(tickets);
        }

        // SUPPORT MANAGER
        [Authorize(Roles = "SupportManager")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [Authorize(Roles = "SupportManager")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignTicket([FromBody] AssignTicketDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ticketService.AssignTicketAsync(dto);
            return Ok(new { Message = "Ticket assigned successfully" });
        }

        // REOPEN & CANCEL
        [HttpPost("reopen/{id}")]
        public async Task<IActionResult> ReopenTicket(int id)
        {
            await _ticketService.ReopenTicketAsync(id);
            return Ok(new { Message = "Ticket reopened successfully" });
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelTicket(int id)
        {
            await _ticketService.CancelTicketAsync(id);
            return Ok(new { Message = "Ticket cancelled successfully" });
        }
        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics()
        {
            var metrics = await _ticketService.GetDashboardMetricsAsync();
            return Ok(metrics);
        }

        [Authorize(Roles = "SupportManager,SupportAgent")]
        [HttpGet("reports/manager")]
        public async Task<IActionResult> GetManagerReports()
        {
            var report = await _ticketService.GetManagerReportsAsync();
            return Ok(report);
        }

        [Authorize(Roles = "SupportAgent")]
        [HttpGet("reports/agent")]
        public async Task<IActionResult> GetAgentReport()
        {
            try 
            {
                var agentId = GetUserIdFromToken();
                var report = await _ticketService.GetAgentReportAsync(agentId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("sub");

            return int.Parse(userIdClaim!.Value);
        }
    }
}
