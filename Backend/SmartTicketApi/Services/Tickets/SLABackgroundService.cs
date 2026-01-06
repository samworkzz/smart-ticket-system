using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.Tickets
{
    public class SLABackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SLABackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public SLABackgroundService(IServiceProvider serviceProvider, ILogger<SLABackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SLA Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking for SLA breaches...");

                try
                {
                    await CheckSLABreachesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking SLA breaches.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("SLA Background Service is stopping.");
        }

        private async Task CheckSLABreachesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Get tickets that are assigned but not resolved/closed and not yet escalated
                var tickets = await context.Tickets
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketPriority)
                    .Where(t => !t.IsEscalated && 
                                t.TicketStatus.StatusName != "Resolved" && 
                                t.TicketStatus.StatusName != "Closed")
                    .ToListAsync();

                var slas = await context.SLAs.ToListAsync();

                foreach (var ticket in tickets)
                {
                    var sla = slas.FirstOrDefault(s => s.TicketPriorityId == ticket.TicketPriorityId);
                    if (sla == null) continue;

                    var elapsedHours = (DateTime.UtcNow - ticket.CreatedAt).TotalHours;

                    if (elapsedHours > sla.ResponseHours)
                    {
                        await EscalateTicket(context, ticket, sla.ResponseHours);
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        private async Task EscalateTicket(AppDbContext context, Ticket ticket, int slaHours)
        {
            _logger.LogWarning($"Ticket {ticket.TicketId} breached SLA ({slaHours} hours). Escalating...");

            ticket.IsEscalated = true;
            ticket.UpdatedAt = DateTime.UtcNow;

            var log = new TicketActivityLog
            {
                TicketId = ticket.TicketId,
                Action = "SLA Breached",
                OldValue = null,
                NewValue = $"Exceeded {slaHours} hours",
                CreatedAt = DateTime.UtcNow
            };

            context.TicketActivityLogs.Add(log);
        }
    }
}
