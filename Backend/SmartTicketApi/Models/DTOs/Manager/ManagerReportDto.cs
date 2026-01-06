namespace SmartTicketApi.Models.DTOs.Manager
{
    public class ManagerReportDto
    {
        public List<AgentWorkloadDto> AgentWorkload { get; set; } = new();
        public SlaComplianceDto SlaCompliance { get; set; } = new();
        public List<AgentPerformanceDto> AgentPerformance { get; set; } = new();
    }

    public class AgentWorkloadDto
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int TotalAssigned {  get; set; }
    }

    public class SlaComplianceDto
    {
        public int MetSlaCount { get; set; }
        public int BreachedSlaCount { get; set; }
        public int ApproachingBreachCount { get; set; } // "In Progress" & near due date
        public double ComplianceRate { get; set; }
        public List<TicketSummaryDto> BreachedTickets { get; set; } = new();
        public List<TicketSummaryDto> AtRiskTickets { get; set; } = new();
    }

    public class AgentPerformanceDto
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public double AvgResolutionHours { get; set; }
        public int ResolvedCount { get; set; }
    }

    public class TicketSummaryDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public double HoursElapsed { get; set; }
    }
}
