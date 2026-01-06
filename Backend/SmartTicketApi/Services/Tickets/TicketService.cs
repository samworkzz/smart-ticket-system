using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
using SmartTicketApi.Models.DTOs.Agent;
using SmartTicketApi.Models.DTOs.Manager;
using SmartTicketApi.Models.DTOs.Tickets;
using SmartTicketApi.Models.Entities;
using SmartTicketApi.Extensions;
using SmartTicketApi.Models.DTOs.Shared;
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

            // Notify Managers
            await _notificationService.NotifyTicketCreatedAsync(ticket);

            return ticket.TicketId;
        }
        //get all tickets created by end user
        public async Task<PagedResponseDto<TicketListDto>> GetTicketsForEndUserAsync(int userId, PagedRequestDto pagination)
        {
            var query = _context.Tickets
                .AsNoTracking()
                .Where(t => t.CreatedById == userId);

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketCategory)
                .Include(t => t.AssignedTo)
                .ApplySorting(pagination.SortBy, pagination.SortDescending)
                .ApplyPaging(pagination.PageNumber, pagination.PageSize)
                .Select(t => new TicketListDto
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Status = t.TicketStatus.StatusName,
                    Priority = t.TicketPriority.PriorityName,
                    Category = t.TicketCategory.CategoryName,
                    CreatedAt = t.CreatedAt,
                    AssignedTo = t.AssignedTo != null ? t.AssignedTo.Name : null,
                    AssignedToId = t.AssignedToId,
                    TicketPriorityId = t.TicketPriorityId,
                    IsEscalated = t.IsEscalated
                })
                .ToListAsync();

            return new PagedResponseDto<TicketListDto>(items, totalCount, pagination.PageNumber, pagination.PageSize);
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

            // Notify Agent
            var agent = await _context.Users.FindAsync(dto.AssignedToUserId);
            if (agent != null)
            {
                await _notificationService.NotifyTicketAssignedAsync(ticket, agent);
            }
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
                    AssignedTo = t.AssignedTo != null ? t.AssignedTo.Name : null,
                    AssignedToId = t.AssignedToId,
                    TicketPriorityId = t.TicketPriorityId,
                    IsEscalated = t.IsEscalated

                })
                .ToListAsync();
        }

        // SupportAgent: Update Status

        public async Task UpdateTicketStatusAsync(UpdateTicketStatusDto dto, string requestorRole)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.TicketId == dto.TicketId);

            if (ticket == null)
                throw new Exception("Ticket not found");

            var newStatus = await _context.TicketStatuses.FindAsync(dto.TicketStatusId);
            
            // --- Validation for Reopened Tickets ---
            if (newStatus?.StatusName == "Closed" && ticket.IsReopened)
            {
                if (requestorRole != "SupportManager")
                {
                    throw new Exception("Re-opened tickets can only be closed by a Manager.");
                }
            }
            // ----------------------------------------

            var oldStatusId = ticket.TicketStatusId;
            ticket.TicketStatusId = dto.TicketStatusId;
            ticket.UpdatedAt = DateTime.UtcNow;

            // --- CHANGED: Handle ResolvedAt timestamp ---
             if (newStatus != null)
            {
                if (newStatus.StatusName == "Resolved" || newStatus.StatusName == "Closed")
                {
                    if (ticket.ResolvedAt == null) 
                        ticket.ResolvedAt = DateTime.UtcNow;
                }
                else
                {
                    // If moving back from Resolved/Closed to In Progress/etc., clear it
                    ticket.ResolvedAt = null;
                }
            }
            // ---------------------------------------------

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

        //get all assigned tickets

        public async Task<PagedResponseDto<TicketListDto>> GetTicketsForAgentAsync(int agentId, PagedRequestDto pagination)
        {
            var query = _context.Tickets
                .AsNoTracking()
                .Where(t => t.AssignedToId == agentId);

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketCategory)
                .Include(t => t.AssignedTo)
                .ApplySorting(pagination.SortBy, pagination.SortDescending)
                .ApplyPaging(pagination.PageNumber, pagination.PageSize)
                .Select(t => new TicketListDto
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Status = t.TicketStatus.StatusName,
                    Priority = t.TicketPriority.PriorityName,
                    Category = t.TicketCategory.CategoryName,
                    CreatedAt = t.CreatedAt,
                    AssignedTo = t.AssignedTo!.Name,
                    AssignedToId = t.AssignedToId,
                    TicketPriorityId = t.TicketPriorityId,
                    IsEscalated = t.IsEscalated
                })
                .ToListAsync();

            return new PagedResponseDto<TicketListDto>(items, totalCount, pagination.PageNumber, pagination.PageSize);
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
                ResolvedAt = ticket.ResolvedAt,
                ResolutionDetails = ticket.ResolutionDetails,
                IsEscalated = ticket.IsEscalated,
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
            ticket.ResolvedAt = null;
            ticket.IsReopened = true; // --- Set IsReopened ---

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
            
            // --- CHANGED: Set ResolvedAt when Cancelled/Closed ---
            if (ticket.ResolvedAt == null)
                ticket.ResolvedAt = DateTime.UtcNow;
            // -----------------------------------------------------------

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
            
            // --- CHANGED: Calculate Average Resolution Time ---
            // Only consider tickets that have a ResolvedAt timestamp
            var resolvedTickets = await _context.Tickets
                .Where(t => t.ResolvedAt != null)
                .Select(t => new { t.CreatedAt, t.ResolvedAt })
                .ToListAsync();

            double avgResolutionHours = 0;
            if (resolvedTickets.Any())
            {
                // Logic: (ResolvedAt - CreatedAt).TotalHours
                var totalHours = resolvedTickets.Sum(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalHours);
                avgResolutionHours = totalHours / resolvedTickets.Count;
            }
            // ----------------------------------------------------

            return new
            {
                StatusMetrics = statusMetrics,
                PriorityMetrics = priorityMetrics,
                CategoryMetrics = categoryMetrics,
                TotalTickets = totalTickets,
                EscalatedTickets = escalatedTickets,
                SlaComplianceRate = Math.Round(complianceRate, 2),
                AverageResolutionTime = Math.Round(avgResolutionHours, 1) // --- ADDED ---
            };
        }
        public async Task<ManagerReportDto> GetManagerReportsAsync()
        {
            var allTickets = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.AssignedTo)
                    .ThenInclude(u => u!.Role)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .ToListAsync();

            var slas = await _context.SLAs.ToListAsync();

            var report = new ManagerReportDto();

            // 1. Agent Workload (Active Tickets: Assigned, In Progress)
            // Filter: AssignedTo != null AND Status is NOT Closed/Resolved
            var activeTickets = allTickets
                .Where(t => t.AssignedToId.HasValue && 
                            t.TicketStatus.StatusName != "Closed" && 
                            t.TicketStatus.StatusName != "Resolved" &&
                            t.AssignedTo!.Role!.RoleName == "SupportAgent")
                .ToList();

            report.AgentWorkload = activeTickets
                .GroupBy(t => t.AssignedToId)
                .Select(g => new AgentWorkloadDto
                {
                    AgentId = g.Key!.Value,
                    AgentName = g.First().AssignedTo!.Name,
                    TotalAssigned = g.Count(),
                    OpenTickets = g.Count(t => t.TicketStatus.StatusName == "Assigned" || t.TicketStatus.StatusName == "Open"),
                    InProgressTickets = g.Count(t => t.TicketStatus.StatusName == "In Progress")
                })
                .ToList();

            // 2. SLA Compliance
            // Breached: IsEscalated = true
            // Met: Resolved/Closed AND IsEscalated = false
            var breachedCount = allTickets.Count(t => t.IsEscalated);
            var completedTickets = allTickets.Where(t => t.TicketStatus.StatusName == "Resolved" || t.TicketStatus.StatusName == "Closed").ToList();
            var metCount = completedTickets.Count(t => !t.IsEscalated);
            var totalWithSla = breachedCount + metCount;

            report.SlaCompliance.BreachedSlaCount = breachedCount;
            report.SlaCompliance.MetSlaCount = metCount;
            report.SlaCompliance.ComplianceRate = totalWithSla > 0 ? (double)metCount / totalWithSla * 100 : 100;

            // At Risk: In Progress, Not Escalated, > 75% of SLA time used
            var inProgressTickets = allTickets.Where(t => t.TicketStatus.StatusName == "In Progress" && !t.IsEscalated && t.AssignedAt.HasValue).ToList();
            
            foreach(var t in inProgressTickets)
            {
                var sla = slas.FirstOrDefault(s => s.TicketPriorityId == t.TicketPriorityId);
                if (sla != null)
                {
                    var elapsed = (DateTime.UtcNow - t.AssignedAt!.Value).TotalHours;
                    var threshold = sla.ResponseHours * 0.75; // 75% warning

                    if (elapsed >= threshold)
                    {
                        report.SlaCompliance.ApproachingBreachCount++;
                        report.SlaCompliance.AtRiskTickets.Add(new TicketSummaryDto
                        {
                            TicketId = t.TicketId,
                            Title = t.Title,
                            AssignedTo = t.AssignedTo?.Name ?? "Unassigned",
                            CreatedAt = t.CreatedAt,
                            HoursElapsed = Math.Round(elapsed, 1)
                        });
                    }
                }
            }

            // 3. Performance (Resolution Time)
            // Consider only Resolved/Closed tickets with ResolvedAt date
            var resolvedList = allTickets
                .Where(t => t.ResolvedAt.HasValue && t.AssignedToId.HasValue)
                .ToList();

            report.AgentPerformance = resolvedList
                .GroupBy(t => t.AssignedToId)
                .Select(g => new AgentPerformanceDto
                {
                    AgentId = g.Key!.Value,
                    AgentName = g.First().AssignedTo!.Name,
                    ResolvedCount = g.Count(),
                    AvgResolutionHours = Math.Round(g.Average(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalHours), 1)
                })
                .ToList();

            return report;
        }

        public async Task<AgentReportDto> GetAgentReportAsync(int agentId)
        {
            var today = DateTime.UtcNow.Date;

            var agentTickets = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.AssignedTo)
                .Where(t => t.AssignedToId == agentId)
                .ToListAsync();
            
            var agent = await _context.Users.FindAsync(agentId);
            if (agent == null) throw new Exception("Agent not found");

            var report = new AgentReportDto
            {
                AgentId = agentId,
                AgentName = agent.Name,
                TotalAssigned = agentTickets.Count,
                EscalatedCount = agentTickets.Count(t => t.IsEscalated),
                // Assuming "Breached" means currently escalated for simplicity, OR we track history. 
                // Based on previous code, IsEscalated=true IS the breach indicator.
                SlaBreachedCount = agentTickets.Count(t => t.IsEscalated), 
                
                AssignedToday = agentTickets.Count(t => t.AssignedAt.HasValue && t.AssignedAt.Value.Date == today),
                ResolvedToday = agentTickets.Count(t => t.ResolvedAt.HasValue && t.ResolvedAt.Value.Date == today),
                ResolvedCount = agentTickets.Count(t => t.ResolvedAt.HasValue)
            };

            // Avg Resolution Time
            var resolved = agentTickets.Where(t => t.ResolvedAt.HasValue).ToList();
            if (resolved.Any())
            {
                report.AvgResolutionHours = Math.Round(resolved.Average(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalHours), 1);
            }

            // Escalated List
            report.EscalatedTickets = agentTickets
                .Where(t => t.IsEscalated)
                .Select(t => new TicketSummaryDto
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    AssignedTo = t.AssignedTo?.Name ?? "Unassigned",
                    CreatedAt = t.CreatedAt,
                    HoursElapsed = Math.Round((DateTime.UtcNow - t.CreatedAt).TotalHours, 1)
                })
                .OrderByDescending(t => t.HoursElapsed) 
                .ToList();

            // SLA Breached (Same as Escalated for now, unless we have history of closed breached tickets)
            // If we want closed breached tickets too:
             report.SlaBreachedTickets = agentTickets
                .Where(t => t.IsEscalated) 
                .Select(t => new TicketSummaryDto
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    AssignedTo = t.AssignedTo?.Name ?? "Unassigned",
                    CreatedAt = t.CreatedAt,
                    HoursElapsed = Math.Round((DateTime.UtcNow - t.CreatedAt).TotalHours, 1)
                })
                .ToList();
            
            return report;
        }

    }
}
