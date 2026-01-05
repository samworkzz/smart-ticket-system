
using SmartTicketApi.Models.DTOs.Manager;

namespace SmartTicketApi.Services.Manager
{
    public interface IManagerService
    {
        Task<List<AgentWorkloadDto>> GetAgentWorkloadsAsync();
        Task<List<UnassignedTicketDto>> GetUnassignedTicketsAsync();
        Task AssignTicketAsync(int ticketId, int agentId);
    }
}
