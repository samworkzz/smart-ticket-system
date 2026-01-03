using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
using SmartTicketApi.Models.DTOs.Manager;
using SmartTicketApi.Models.DTOs.Manager;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.Manager
{
    public class ManagerService : IManagerService
    {
        private readonly AppDbContext _context;

        public ManagerService(AppDbContext context)
        {
            _context = context;
        }

        // ==================================================
        // 1️⃣ Get all support agents + their ticket workload
        // ==================================================
        public async Task<List<AgentWorkloadDto>> GetAgentWorkloadsAsync()
        {
            // Get all users who are Support Agents
            var agents = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "SupportAgent")
                .ToListAsync();

            // Map workload
            var result = agents.Select(agent => new AgentWorkloadDto
            {
                AgentId = agent.UserId,
                Name = agent.Name,
                AssignedTicketCount = _context.Tickets
                    .Count(t => t.AssignedToId == agent.UserId)
            }).ToList();

            return result;
        }

        // ==================================
        // 2️⃣ Get all unassigned tickets
        // ==================================
        public async Task<List<object>> GetUnassignedTicketsAsync()
        {
            var tickets = await _context.Tickets
                .Where(t => t.AssignedToId == null)
                .Include(t => t.TicketCategory)
                .Include(t => t.TicketPriority)
                .Select(t => new
                {
                    t.TicketId,
                    t.Title,
                    Category = t.TicketCategory.CategoryName,
                    Priority = t.TicketPriority.PriorityName,
                    t.CreatedAt
                })
                .ToListAsync();

            return tickets.Cast<object>().ToList();
        }

        // ================================
        // 3️⃣ Assign a ticket to an agent
        // ================================
        public async Task AssignTicketAsync(int ticketId, int agentId)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found");

            var agentExists = await _context.Users
                .Include(u => u.Role)
                .AnyAsync(u => u.UserId == agentId && u.Role.RoleName == "SupportAgent");

            if (!agentExists)
                throw new Exception("Agent not found or not a SupportAgent");


            if (!agentExists)
                throw new Exception("Agent not found");

            ticket.AssignedToId = agentId;

            // Optional: Move ticket to "In Progress"
            // (Assuming StatusId = 2 is In Progress)
            ticket.TicketStatusId = 2;

            await _context.SaveChangesAsync();
        }
    }
}
