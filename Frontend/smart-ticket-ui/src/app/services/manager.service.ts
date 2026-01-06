import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AgentWorkloadDto, UnassignedTicket, AssignTicketPayload } from '../models/manager.models';
import { PagedRequest, PagedResponse } from '../models/shared.models';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ManagerService {

  private apiUrl = 'http://localhost:5125/api/manager';

  constructor(private http: HttpClient) { }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: {
        Authorization: `Bearer ${token}`
      }
    };
  }

  getAgents(): Observable<AgentWorkloadDto[]> {
    return this.http.get<AgentWorkloadDto[]>(`${this.apiUrl}/agents`, this.getAuthHeaders());
  }

  getUnassignedTickets(pagination?: PagedRequest): Observable<PagedResponse<UnassignedTicket>> {
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber)
        .set('pageSize', pagination.pageSize);
      if (pagination.sortBy) params = params.set('sortBy', pagination.sortBy);
      if (pagination.sortDescending !== undefined) params = params.set('sortDescending', pagination.sortDescending);
    }

    return this.http.get<PagedResponse<UnassignedTicket>>(
      `${this.apiUrl}/unassigned-tickets`,
      { ...this.getAuthHeaders(), params }
    );
  }

  assignTicket(ticketId: number, agentId: number): Observable<any> {
    const payload: AssignTicketPayload = {
      ticketId,
      assignedToUserId: agentId
    };
    return this.http.put(`${this.apiUrl}/assign`, payload, this.getAuthHeaders());
  }
}
