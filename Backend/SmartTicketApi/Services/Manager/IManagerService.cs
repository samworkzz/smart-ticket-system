
using SmartTicketApi.Models.DTOs.Manager;

namespace SmartTicketApi.Services.Manager
{
    public interface IManagerService
    {
        Task<List<AgentWorkloadDto>> GetAgentWorkloadsAsync();
        Task<List<object>> GetUnassignedTicketsAsync();
        Task AssignTicketAsync(int ticketId, int agentId);
    }
}
