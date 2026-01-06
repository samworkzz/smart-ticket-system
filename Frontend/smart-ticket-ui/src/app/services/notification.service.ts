import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class NotificationService {

    private apiUrl = 'http://localhost:5125/api/notification';

    constructor(private http: HttpClient) { }

    private getAuthHeaders() {
        const token = localStorage.getItem('token');
        return {
            headers: {
                Authorization: `Bearer ${token}`
            }
        };
    }

    getNotifications(): Observable<any[]> {
        return this.http.get<any[]>(this.apiUrl, this.getAuthHeaders());
    }

    markAsRead(id: number): Observable<any> {
        return this.http.post(`${this.apiUrl}/read/${id}`, {}, this.getAuthHeaders());
    }
}
