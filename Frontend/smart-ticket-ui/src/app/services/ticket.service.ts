import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedRequest, PagedResponse } from '../models/shared.models';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class TicketService {

  private apiUrl = 'http://localhost:5125/api/ticket';

  constructor(private http: HttpClient) { }

  /**
   * Helper method to attach JWT token to requests
   * Follows same pattern as lab OrderService
   */
  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: {
        Authorization: `Bearer ${token}`
      }
    };
  }

  /**
   * END USER: Create a new ticket
   */
  createTicket(data: any): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/create`,
      data,
      this.getAuthHeaders()
    );
  }

  /**
   * END USER: Get my tickets
   */
  getMyTickets(pagination?: PagedRequest): Observable<PagedResponse<any>> {
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber)
        .set('pageSize', pagination.pageSize);
      if (pagination.sortBy) params = params.set('sortBy', pagination.sortBy);
      if (pagination.sortDescending !== undefined) params = params.set('sortDescending', pagination.sortDescending);
    }

    return this.http.get<PagedResponse<any>>(
      `${this.apiUrl}/my`,
      { ...this.getAuthHeaders(), params }
    );
  }

  /**
   * SUPPORT AGENT: Get assigned tickets
   */
  getAssignedTickets(pagination?: PagedRequest): Observable<PagedResponse<any>> {
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber)
        .set('pageSize', pagination.pageSize);
      if (pagination.sortBy) params = params.set('sortBy', pagination.sortBy);
      if (pagination.sortDescending !== undefined) params = params.set('sortDescending', pagination.sortDescending);
    }

    return this.http.get<PagedResponse<any>>(
      `${this.apiUrl}/assigned`,
      { ...this.getAuthHeaders(), params }
    );
  }

  /**
   * SUPPORT MANAGER: Get all tickets
   */
  getAllTickets(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/all`,
      this.getAuthHeaders()
    );
  }

  /**
   * SUPPORT MANAGER: Assign ticket to agent
   */
  assignTicket(ticketId: number, assignedToUserId: number): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/assign`,
      { ticketId, assignedToUserId },
      this.getAuthHeaders()
    );
  }

  /**
   * SUPPORT AGENT / MANAGER: Update ticket status
   */
  updateStatus(ticketId: number, ticketStatusId: number, resolutionDetails?: string): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/status`,
      { ticketId, ticketStatusId, resolutionDetails },
      this.getAuthHeaders()
    );
  }
  /**
   * SHARED: Get Ticket Details
   */
  getTicketDetails(ticketId: number): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}/${ticketId}`,
      this.getAuthHeaders()
    );
  }

  reopenTicket(ticketId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/reopen/${ticketId}`, {}, this.getAuthHeaders());
  }

  cancelTicket(ticketId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/cancel/${ticketId}`, {}, this.getAuthHeaders());
  }

  getMetrics(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/metrics`, this.getAuthHeaders());
  }

  getManagerReports(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/reports/manager`, this.getAuthHeaders());
  }
}
