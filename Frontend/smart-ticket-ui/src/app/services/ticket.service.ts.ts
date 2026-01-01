import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TicketService {

  private apiUrl = 'https://localhost:7088/api/ticket';

  constructor(private http: HttpClient) {}

  createTicket(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/create`, data);
  }
}
