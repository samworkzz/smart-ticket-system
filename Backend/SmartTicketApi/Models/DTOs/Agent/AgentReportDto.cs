using SmartTicketApi.Models.DTOs.Manager;

namespace SmartTicketApi.Models.DTOs.Agent
{
    public class AgentReportDto
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int TotalAssigned { get; set; }
        public int ResolvedCount { get; set; }
        public int EscalatedCount { get; set; } // Currently Escalated
        public int SlaBreachedCount { get; set; } // Total Breached (Historical + Current)
        
        public double AvgResolutionHours { get; set; }
        public int ResolvedToday { get; set; }
        public int AssignedToday { get; set; }

        public List<TicketSummaryDto> EscalatedTickets { get; set; } = new();
        public List<TicketSummaryDto> SlaBreachedTickets { get; set; } = new();
    }
}
