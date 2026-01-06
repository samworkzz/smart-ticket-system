using SmartTicketApi.Models.DTOs.Agent;
using SmartTicketApi.Models.DTOs.Shared;
using SmartTicketApi.Models.DTOs.Manager;
using SmartTicketApi.Models.DTOs.Tickets;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.Tickets
{
    public interface ITicketService
    {
        // End User
        Task<int> CreateTicketAsync(int userId, CreateTicketDto dto);
        Task<PagedResponseDto<TicketListDto>> GetTicketsForEndUserAsync(int userId, PagedRequestDto pagination);
        Task<AgentReportDto> GetAgentReportAsync(int agentId);


        // Support Manager
        Task<IEnumerable<TicketListDto>> GetAllTicketsAsync();
        Task AssignTicketAsync(AssignTicketDto dto);

        // Support Agent
        Task UpdateTicketStatusAsync(UpdateTicketStatusDto dto, string requestorRole);
        Task<PagedResponseDto<TicketListDto>> GetTicketsForAgentAsync(int agentId, PagedRequestDto pagination);

        // Admin
        Task UpdateTicketPriorityAsync(int ticketId, int ticketPriorityId);

        // Shared (Details)
        Task<TicketDetailsDto?> GetTicketDetailsAsync(int ticketId, int requestorId, string requestorRole);
        Task ReopenTicketAsync(int ticketId);
        Task CancelTicketAsync(int ticketId);

        Task<object> GetDashboardMetricsAsync();
        Task<ManagerReportDto> GetManagerReportsAsync();


       
        
        
    }
}
