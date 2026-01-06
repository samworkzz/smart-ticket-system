using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicketApi.Models.DTOs.Manager;
using SmartTicketApi.Models.DTOs.Shared;
using SmartTicketApi.Services.Manager;

namespace SmartTicketApi.Controllers
{
    [ApiController]
    [Route("api/manager")]
    [Authorize(Roles = "SupportManager")]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        
        // 1️ Get all agents + workload
     
        [HttpGet("agents")]
        public async Task<IActionResult> GetAgents()
        {
            var agents = await _managerService.GetAgentWorkloadsAsync();
            return Ok(agents);
        }

      
        // 2️ Get unassigned tickets
        
        [HttpGet("unassigned-tickets")]
        public async Task<IActionResult> GetUnassignedTickets([FromQuery] PagedRequestDto pagination)
        {
            var tickets = await _managerService.GetUnassignedTicketsAsync(pagination);
            return Ok(tickets);
        }

        // 3️ Assign ticket to agent
        [HttpPut("assign")]
        public async Task<IActionResult> AssignTicket([FromBody] AssignTicketDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid payload");

            try
            {
                await _managerService.AssignTicketAsync(dto.TicketId, dto.AssignedToUserId);
                return Ok(new { message = "Ticket assigned successfully" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // 🔧 Debug endpoint (optional)
        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok("Manager controller is reachable");
        }
    }
}
