import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AgentWorkload, UnassignedTicket, AssignTicketPayload } from '../models/manager.models';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ManagerService {

  private apiUrl = 'http://localhost:5125/api/manager';

  constructor(private http: HttpClient) { }

  getAgents(): Observable<AgentWorkload[]> {
    return this.http.get<AgentWorkload[]>(`${this.apiUrl}/agents`);
  }

  getUnassignedTickets(): Observable<UnassignedTicket[]> {
    return this.http.get<UnassignedTicket[]>(`${this.apiUrl}/unassigned-tickets`);
  }

  assignTicket(ticketId: number, agentId: number): Observable<any> {
    const payload: AssignTicketPayload = {
      ticketId,
      assignedToUserId: agentId
    };
    return this.http.put(`${this.apiUrl}/assign`, payload);
  }
}
