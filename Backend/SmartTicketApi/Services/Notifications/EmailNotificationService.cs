using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.Notifications
{
    public class EmailNotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public EmailNotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task NotifyTicketClosedAsync(Ticket ticket, User agent)
        {
            // 1. Notify Creator
            Console.WriteLine($"[EMAIL] To: Product Owner/Creator (UserId: {ticket.CreatedById})");
            Console.WriteLine($"[SUBJECT] Your Ticket #{ticket.TicketId} has been Closed.");
            Console.WriteLine($"[BODY] Ticket '{ticket.Title}' was closed by {agent.Name}.");

            // 2. Notify Support Managers
            var managers = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "SupportManager")
                .ToListAsync();

            foreach (var manager in managers)
            {
                Console.WriteLine($"[EMAIL] To: Manager {manager.Name} ({manager.Email})");
                Console.WriteLine($"[SUBJECT] Ticket #{ticket.TicketId} Closed");
                Console.WriteLine($"[BODY] Ticket '{ticket.Title}' was closed by agent {agent.Name}.");
            }

            // 3. Notify Admins
            var admins = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Admin")
                .ToListAsync();

            foreach (var admin in admins)
            {
                Console.WriteLine($"[EMAIL] To: Admin {admin.Name} ({admin.Email})");
                Console.WriteLine($"[SUBJECT] Ticket #{ticket.TicketId} Closed");
            }
        }
    }
}
