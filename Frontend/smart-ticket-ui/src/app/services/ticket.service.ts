import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TicketService {

  private apiUrl = 'https://localhost:7088/api/ticket';

  constructor(private http: HttpClient) {}

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
  getMyTickets(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/my`,
      this.getAuthHeaders()
    );
  }

  /**
   * SUPPORT AGENT: Get assigned tickets
   */
  getAssignedTickets(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/assigned`,
      this.getAuthHeaders()
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
    return this.http.put(
      `${this.apiUrl}/assign`,
      { ticketId, assignedToUserId },
      this.getAuthHeaders()
    );
  }

  /**
   * SUPPORT AGENT / MANAGER: Update ticket status
   */
  updateStatus(ticketId: number, ticketStatusId: number): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/status`,
      { ticketId, ticketStatusId },
      this.getAuthHeaders()
    );
  }
}
  