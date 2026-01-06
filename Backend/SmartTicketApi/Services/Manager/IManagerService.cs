using SmartTicketApi.Models.DTOs.Shared;
using SmartTicketApi.Models.DTOs.Manager;

namespace SmartTicketApi.Services.Manager
{
    public interface IManagerService
    {
        Task<List<AgentWorkloadDto>> GetAgentWorkloadsAsync();
        Task<PagedResponseDto<UnassignedTicketDto>> GetUnassignedTicketsAsync(PagedRequestDto pagination);
        Task AssignTicketAsync(int ticketId, int agentId);
    }
}
