using SmartTicketApi.Models.DTOs.Agent;
using SmartTicketApi.Models.DTOs.Manager;
using SmartTicketApi.Models.DTOs.Tickets;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.Tickets
{
    public interface ITicketService
    {
        // End User
        Task<int> CreateTicketAsync(int userId, CreateTicketDto dto);
        Task<IEnumerable<TicketListDto>> GetTicketsForEndUserAsync(int userId);

        //Support Manager
        Task<IEnumerable<TicketListDto>> GetAllTicketsAsync();

        // Support Agent
        Task UpdateTicketStatusAsync(UpdateTicketStatusDto dto);
        Task<IEnumerable<TicketListDto>> GetTicketsForAgentAsync(int agentId);

        // Admin
        Task UpdateTicketPriorityAsync(int ticketId, int ticketPriorityId);


       
        
        
    }
}
