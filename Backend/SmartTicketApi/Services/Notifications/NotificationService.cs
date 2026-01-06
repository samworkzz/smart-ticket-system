using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private async Task CreateNotification(int userId, string message, int ticketId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var notification = new Notification
                {
                    UserId = userId,
                    Message = message,
                    TicketId = ticketId,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                context.Notifications.Add(notification);
                await context.SaveChangesAsync();
            }
        }

        public async Task NotifyTicketCreatedAsync(Ticket ticket)
        {
            // Notify all Managers
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var managers = await context.Users
                    .AsNoTracking()
                    .Include(u => u.Role)
                    .Where(u => u.Role.RoleName == "SupportManager" || u.Role.RoleName == "Admin")
                    .ToListAsync();

                foreach (var manager in managers)
                {
                    await CreateNotification(manager.UserId, $"New Ticket Created: #{ticket.TicketId} - {ticket.Title}", ticket.TicketId);
                }
            }
        }

        public async Task NotifyTicketAssignedAsync(Ticket ticket, User agent)
        {
             // Notify the Agent
             await CreateNotification(agent.UserId, $"You have been assigned Ticket #{ticket.TicketId}: {ticket.Title}", ticket.TicketId);
        }

        public async Task NotifyTicketClosedAsync(Ticket ticket, User agent)
        {
             // Notify Creator
             if (ticket.CreatedById != agent.UserId) // if agent didn't close their own ticket (unlikely for enduser created, but logic holds)
                await CreateNotification(ticket.CreatedById, $"Your ticket #{ticket.TicketId} has been closed.", ticket.TicketId);
        }
    }
}
