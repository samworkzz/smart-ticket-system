using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.Notifications
{
    public interface INotificationService
    {
        Task NotifyTicketClosedAsync(Ticket ticket, User agent);
        Task NotifyTicketCreatedAsync(Ticket ticket);
        Task NotifyTicketAssignedAsync(Ticket ticket, User agent);
    }
}
