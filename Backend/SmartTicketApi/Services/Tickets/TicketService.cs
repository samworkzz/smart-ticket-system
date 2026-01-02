using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
using SmartTicketApi.Models.DTOs;
using SmartTicketApi.Models.DTOs.Tickets;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.Tickets
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;

        public TicketService(AppDbContext context)
        {
            _context = context;
        }

        // EndUser: Create Ticket

        public async Task<int> CreateTicketAsync(int userId, CreateTicketDto dto)
        {
            var createdStatus = await _context.TicketStatuses
                .FirstAsync(s => s.StatusName == "Created");

            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                TicketCategoryId = dto.TicketCategoryId,
                TicketPriorityId = dto.TicketPriorityId,
                TicketStatusId = createdStatus.TicketStatusId,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                IsEscalated = false
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            await AddActivityLog(
                ticket.TicketId,
                "Ticket Created",
                null,
                "Created"
            );

            return ticket.TicketId;
        }
        //End User get ticket
        public async Task<IEnumerable<TicketListDto>> GetTicketsForEndUserAsync(int userId)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.CreatedById == userId)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketCategory)
                .Include(t => t.AssignedTo)
                .Select(t => new TicketListDto
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Status = t.TicketStatus.StatusName,
                    Priority = t.TicketPriority.PriorityName,
                    Category = t.TicketCategory.CategoryName,
                    CreatedAt = t.CreatedAt,
                    AssignedTo = t.AssignedTo != null ? t.AssignedTo.Name : null
                })
                .ToListAsync();
        }


        // SupportManager: Assign Ticket

        public async Task AssignTicketAsync(AssignTicketDto dto)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.TicketId == dto.TicketId);

            if (ticket == null)
                throw new Exception("Ticket not found");

            ticket.AssignedToId = dto.AssignedToUserId;
            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.AssignedAt = DateTime.UtcNow;

            var assignedStatus = await _context.TicketStatuses
                .FirstAsync(s => s.StatusName == "Assigned");

            ticket.TicketStatusId = assignedStatus.TicketStatusId;

            await _context.SaveChangesAsync();

            await AddActivityLog(
                ticket.TicketId,
                "Ticket Assigned",
                null,
                $"AssignedToUserId={dto.AssignedToUserId}"
            );
        }
        //support manager get all tickets
        public async Task<IEnumerable<TicketListDto>> GetAllTicketsAsync()
        {
            return await _context.Tickets
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketCategory)
                .Include(t => t.AssignedTo)
                .Select(t => new TicketListDto
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Status = t.TicketStatus.StatusName,
                    Priority = t.TicketPriority.PriorityName,
                    Category = t.TicketCategory.CategoryName,
                    CreatedAt = t.CreatedAt,
                    AssignedTo = t.AssignedTo != null ? t.AssignedTo.Name : null
                })
                .ToListAsync();
        }

        // SupportAgent: Update Status

        public async Task UpdateTicketStatusAsync(UpdateTicketStatusDto dto)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.TicketId == dto.TicketId);

            if (ticket == null)
                throw new Exception("Ticket not found");

            var oldStatusId = ticket.TicketStatusId;
            ticket.TicketStatusId = dto.TicketStatusId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await AddActivityLog(
                ticket.TicketId,
                "Status Updated",
                oldStatusId.ToString(),
                dto.TicketStatusId.ToString()
            );
        }

        //Agent-get ticket
        public async Task<IEnumerable<TicketListDto>> GetTicketsForAgentAsync(int agentId)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.AssignedToId == agentId)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketCategory)
                .Include(t => t.AssignedTo)
                .Select(t => new TicketListDto
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Status = t.TicketStatus.StatusName,
                    Priority = t.TicketPriority.PriorityName,
                    Category = t.TicketCategory.CategoryName,
                    CreatedAt = t.CreatedAt,
                    AssignedTo = t.AssignedTo!.Name
                })
                .ToListAsync();
        }


        // Admin: Update Ticket Priority
        public async Task UpdateTicketPriorityAsync(int ticketId, int ticketPriorityId)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found");

            var oldPriorityId = ticket.TicketPriorityId;
            ticket.TicketPriorityId = ticketPriorityId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await AddActivityLog(
                ticket.TicketId,
                "Priority Updated",
                oldPriorityId.ToString(),
                ticketPriorityId.ToString()
            );
        }

        private async Task CheckAndEscalateIfBreachedAsync(Ticket ticket)
        {
            if (ticket.AssignedAt == null)
                return;

            if (ticket.IsEscalated)
                return;

            var sla = await _context.SLAs
                .Include(s => s.TicketPriority)
                .FirstOrDefaultAsync(s => s.TicketPriorityId == ticket.TicketPriorityId);

            if (sla == null)
                return;

            var elapsedHours = (DateTime.UtcNow - ticket.AssignedAt.Value).TotalHours;

            if (elapsedHours > sla.ResponseHours &&
                ticket.TicketStatus.StatusName != "Resolved" &&
                ticket.TicketStatus.StatusName != "Closed")
            {
                ticket.IsEscalated = true;
                ticket.UpdatedAt = DateTime.UtcNow;

                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();

                var log = new TicketActivityLog
                {
                    TicketId = ticket.TicketId,
                    Action = "SLA Breached",
                    OldValue = null,
                    NewValue = $"Exceeded {sla.ResponseHours} hours",
                    CreatedAt = DateTime.UtcNow
                };

                _context.TicketActivityLogs.Add(log);
                await _context.SaveChangesAsync();
            }
        }


        // Helper: Activity Log

        private async Task AddActivityLog(
            int ticketId,
            string action,
            string? oldValue,
            string? newValue
        )
        {
            var log = new TicketActivityLog
            {
                TicketId = ticketId,
                Action = action,
                OldValue = oldValue,
                NewValue = newValue,
                CreatedAt = DateTime.UtcNow
            };

            _context.TicketActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
