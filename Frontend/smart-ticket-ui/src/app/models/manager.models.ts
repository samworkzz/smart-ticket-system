// AgentWorkload removed. Use AgentWorkloadDto instead.

export interface UnassignedTicket {
    ticketId: number;
    title: string;
    category: string;
    priority: string;
    createdAt: string;
}

export interface AssignTicketPayload {
    ticketId: number;
    assignedToUserId: number;
}

export interface ManagerReportData {
    agentWorkload: AgentWorkloadDto[];
    slaCompliance: SlaComplianceDto;
    agentPerformance: AgentPerformanceDto[];
}

export interface AgentWorkloadDto {
    agentId: number;
    agentName: string;
    openTickets: number;
    inProgressTickets: number;
    totalAssigned: number;
}

export interface SlaComplianceDto {
    metSlaCount: number;
    breachedSlaCount: number;
    approachingBreachCount: number;
    complianceRate: number;
    breachedTickets: TicketSummaryDto[];
    atRiskTickets: TicketSummaryDto[];
}

export interface AgentPerformanceDto {
    agentId: number;
    agentName: string;
    avgResolutionHours: number;
    resolvedCount: number;
}

export interface TicketSummaryDto {
    ticketId: number;
    title: string;
    assignedTo: string;
    createdAt: string;
    hoursElapsed: number;
}
