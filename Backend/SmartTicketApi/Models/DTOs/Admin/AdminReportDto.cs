using SmartTicketApi.Models.DTOs.Manager;

namespace SmartTicketApi.Models.DTOs.Admin
{
    public class AdminReportDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int EscalatedTickets { get; set; }

        public List<ManagerSummaryDto> ManagerSummaries { get; set; } = new();
        public List<AgentPerformanceDto> AgentSummaries { get; set; } = new();
    }

    public class ManagerSummaryDto
    {
        public int ManagerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // Placeholder for future metrics (e.g., Team Size if we had teams)
    }
}
