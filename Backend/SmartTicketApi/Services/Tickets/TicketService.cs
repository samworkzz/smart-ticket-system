using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
using SmartTicketApi.Models.DTOs.Agent;
using SmartTicketApi.Models.DTOs.Manager;
using SmartTicketApi.Models.DTOs.Tickets;
using SmartTicketApi.Models.Entities;
using SmartTicketApi.Services.Notifications;

namespace SmartTicketApi.Services.Tickets
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public TicketService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
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
        //get all tickets created by end user
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
                .Include(t => t.TicketStatus)
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
       


        //list of all the tickets 
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

            // Check if Closed (Assuming 5 is Closed based on Seed Data)
            var closedStatus = await _context.TicketStatuses.FirstOrDefaultAsync(s => s.StatusName == "Closed");
            if (closedStatus != null && dto.TicketStatusId == closedStatus.TicketStatusId)
            {
                // We need the agent info
                 var agent = await _context.Users.FindAsync(ticket.AssignedToId);
                 if (agent != null)
                 {
                     await _notificationService.NotifyTicketClosedAsync(ticket, agent);
                 }
            }
        }

        //get all assigned tickets

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

        public async Task<TicketDetailsDto?> GetTicketDetailsAsync(int ticketId, int requestorId, string requestorRole)
        {
            var ticket = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketCategory)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Include(t => t.ActivityLogs)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
                return null;

            // Authorization Check
            if (requestorRole == "EndUser" && ticket.CreatedById != requestorId)
                return null; // Not authorized

            // Map to DTO
            return new TicketDetailsDto
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                Category = ticket.TicketCategory?.CategoryName ?? "Unknown",
                Priority = ticket.TicketPriority?.PriorityName ?? "Normal",
                Status = ticket.TicketStatus?.StatusName ?? "Unknown",
                CreatedAt = ticket.CreatedAt,
                CreatedBy = ticket.CreatedBy?.Name ?? "Unknown",
                AssignedTo = ticket.AssignedTo?.Name,
                Comments = ticket.Comments?.Select(c => new TicketCommentDto
                {
                    CommentId = c.TicketCommentId,
                    CommentText = c.CommentText,
                    UserName = c.User?.Name ?? "Unknown",
                    CreatedAt = c.CreatedAt
                }).OrderBy(c => c.CreatedAt).ToList() ?? new(),
                ActivityLogs = ticket.ActivityLogs?.Select(l => new TicketLogDto
                {
                    LogId = l.TicketActivityLogId,
                    Action = l.Action,
                    OldValue = l.OldValue,
                    NewValue = l.NewValue,
                    CreatedAt = l.CreatedAt
                }).OrderByDescending(l => l.CreatedAt).ToList() ?? new()
            };
        }

        public async Task ReopenTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.TicketStatus)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found");

            var statusName = ticket.AssignedToId.HasValue ? "In Progress" : "Created";
            var status = await _context.TicketStatuses.FirstAsync(s => s.StatusName == statusName);
            
            var oldStatusId = ticket.TicketStatusId;
            ticket.TicketStatusId = status.TicketStatusId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await AddActivityLog(
                ticket.TicketId,
                "Ticket Reopened",
                oldStatusId.ToString(),
                status.TicketStatusId.ToString()
            );
        }

        public async Task CancelTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.TicketStatus)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found");

            var status = await _context.TicketStatuses.FirstOrDefaultAsync(s => s.StatusName == "Closed");
            // Optionally add "Cancelled" status if needed, but "Closed" is used here for simplicity as per seed.
            
            var oldStatusId = ticket.TicketStatusId;
            ticket.TicketStatusId = status!.TicketStatusId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await AddActivityLog(
                ticket.TicketId,
                "Ticket Cancelled",
                oldStatusId.ToString(),
                status.TicketStatusId.ToString()
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





        public async Task<object> GetDashboardMetricsAsync()
        {
            var statusMetrics = await _context.Tickets
                .GroupBy(t => t.TicketStatus.StatusName)
                .Select(g => new { Name = g.Key, Value = g.Count() })
                .ToListAsync();

            var priorityMetrics = await _context.Tickets
                .GroupBy(t => t.TicketPriority.PriorityName)
                .Select(g => new { Name = g.Key, Value = g.Count() })
                .ToListAsync();

            var categoryMetrics = await _context.Tickets
                .GroupBy(t => t.TicketCategory.CategoryName)
                .Select(g => new { Name = g.Key, Value = g.Count() })
                .ToListAsync();

            var totalTickets = await _context.Tickets.CountAsync();
            var escalatedTickets = await _context.Tickets.CountAsync(t => t.IsEscalated);
            
            // SLA Compliance: Tickets not escalated / Total Tickets (assigned or closed)
            var complianceRate = totalTickets > 0 
                ? (double)(totalTickets - escalatedTickets) / totalTickets * 100 
                : 100;

            return new
            {
                StatusMetrics = statusMetrics,
                PriorityMetrics = priorityMetrics,
                CategoryMetrics = categoryMetrics,
                TotalTickets = totalTickets,
                EscalatedTickets = escalatedTickets,
                SlaComplianceRate = Math.Round(complianceRate, 2)
            };
        }
    }
}
