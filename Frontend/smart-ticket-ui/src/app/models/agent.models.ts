export interface AssignedTicket {
    ticketId: number;
    title: string;
    description: string;
    category: string;
    priority: string;
    status: string;
    createdAt: string;
}

export const TicketStatus = {
    Assigned: 2,
    InProgress: 3,
    Resolved: 4,
    Closed: 5
};
