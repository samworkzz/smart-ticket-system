export interface AgentWorkload {
    agentId: number;
    name: string;
    assignedTicketCount: number;
}

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
