using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
using SmartTicketApi.Models.DTOs.Manager;
using SmartTicketApi.Models.DTOs.Manager;
using SmartTicketApi.Models.Entities;
using SmartTicketApi.Extensions;
using SmartTicketApi.Models.DTOs.Shared;

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
        // ==================================
        // 2️⃣ Get all unassigned tickets
        // ==================================
        public async Task<PagedResponseDto<UnassignedTicketDto>> GetUnassignedTicketsAsync(PagedRequestDto pagination)
        {
            var query = _context.Tickets
                .AsNoTracking()
                .Where(t => t.AssignedToId == null);

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(t => t.TicketCategory)
                .Include(t => t.TicketPriority)
                .ApplySorting(pagination.SortBy, pagination.SortDescending)
                .ApplyPaging(pagination.PageNumber, pagination.PageSize)
                .Select(t => new UnassignedTicketDto
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Category = t.TicketCategory.CategoryName,
                    Priority = t.TicketPriority.PriorityName,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return new PagedResponseDto<UnassignedTicketDto>(items, totalCount, pagination.PageNumber, pagination.PageSize);
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


            ticket.AssignedToId = agentId;

            // Optional: Move ticket to "In Progress"
            // (Assuming StatusId = 2 is In Progress)
            ticket.TicketStatusId = 2;

            await _context.SaveChangesAsync();
        }
        //public async Task AssignTicketAsync(int ticketId, int agentId)
        //{
        //    Console.WriteLine($"AssignTicket called with agentId = {agentId}");

        //    var allUsers = await _context.Users
        //        .Select(u => new { u.UserId, u.Name, u.RoleId })
        //        .ToListAsync();

        //    Console.WriteLine("Users in DB:");
        //    foreach (var u in allUsers)
        //    {
        //        Console.WriteLine($"UserId={u.UserId}, Name={u.Name}, RoleId={u.RoleId}");
        //    }

        //    var agent = await _context.Users
        //        .Include(u => u.Role)
        //        .FirstOrDefaultAsync(u => u.UserId == agentId);

        //    if (agent == null)
        //        throw new Exception("Agent not found");

        //    if (agent.Role.RoleName != "SupportAgent")
        //        throw new Exception("User is not a SupportAgent");

        //    var ticket = await _context.Tickets
        //        .FirstOrDefaultAsync(t => t.TicketId == ticketId);

        //    if (ticket == null)
        //        throw new Exception("Ticket not found");

        //    ticket.AssignedToId = agentId;
        //    ticket.TicketStatusId = 2;

        //    await _context.SaveChangesAsync();
        //}
    }
}
