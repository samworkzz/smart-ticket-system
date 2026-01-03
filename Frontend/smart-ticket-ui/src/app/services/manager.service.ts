import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ManagerService {

  private apiUrl = 'https://localhost:7088/api/manager';

  constructor(private http: HttpClient) {}

  getAgents() {
    return this.http.get<any[]>(`${this.apiUrl}/agents`);
  }

  getUnassignedTickets() {
    return this.http.get<any[]>(`${this.apiUrl}/unassigned-tickets`);
  }

  assignTicket(ticketId: number, agentId: number) {
    return this.http.put(`${this.apiUrl}/assign`, {
      ticketId,
      agentId
    });
  }
}
