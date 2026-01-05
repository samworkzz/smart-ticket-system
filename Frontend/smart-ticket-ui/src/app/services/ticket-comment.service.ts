import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class TicketCommentService {

    private apiUrl = 'http://localhost:5125/api/ticket-comments';

    constructor(private http: HttpClient) { }

    private getAuthHeaders() {
        const token = localStorage.getItem('token');
        return {
            headers: {
                Authorization: `Bearer ${token}`
            }
        };
    }

    addComment(payload: { ticketId: number, comment: string }): Observable<any> {
        return this.http.post(
            this.apiUrl,
            payload,
            this.getAuthHeaders()
        );
    }
}
