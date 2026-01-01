using SmartTicketApi.Models.DTOs.Tickets;

namespace SmartTicketApi.Services.Tickets
{
    public interface ITicketService
    {
        // End User
        Task<int> CreateTicketAsync(int userId, CreateTicketDto dto);

        // Support Manager
        Task AssignTicketAsync(AssignTicketDto dto);

        // Support Agent
        Task UpdateTicketStatusAsync(UpdateTicketStatusDto dto);

        // Admin
        Task UpdateTicketPriorityAsync(int ticketId, int ticketPriorityId);
    }
}
