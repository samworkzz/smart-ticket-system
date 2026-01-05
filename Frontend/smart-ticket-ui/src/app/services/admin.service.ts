import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AdminService {
    private apiUrl = 'http://localhost:5125/api/admin';

    constructor(private http: HttpClient) { }

    private getAuthHeaders() {
        const token = localStorage.getItem('token');
        return {
            headers: {
                Authorization: `Bearer ${token}`
            }
        };
    }

    // Categories
    getCategories(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/categories`, this.getAuthHeaders());
    }

    createCategory(name: string): Observable<any> {
        return this.http.post(`${this.apiUrl}/categories`, `"${name}"`, {
            headers: {
                ...this.getAuthHeaders().headers,
                'Content-Type': 'application/json'
            }
        });
    }

    updateCategory(id: number, name: string): Observable<any> {
        return this.http.put(`${this.apiUrl}/categories/${id}`, `"${name}"`, {
            headers: {
                ...this.getAuthHeaders().headers,
                'Content-Type': 'application/json'
            }
        });
    }

    deleteCategory(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/categories/${id}`, this.getAuthHeaders());
    }

    // Priorities
    getPriorities(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/priorities`, this.getAuthHeaders());
    }

    createPriority(name: string): Observable<any> {
        return this.http.post(`${this.apiUrl}/priorities`, `"${name}"`, {
            headers: {
                ...this.getAuthHeaders().headers,
                'Content-Type': 'application/json'
            }
        });
    }

    updatePriority(id: number, name: string): Observable<any> {
        return this.http.put(`${this.apiUrl}/priorities/${id}`, `"${name}"`, {
            headers: {
                ...this.getAuthHeaders().headers,
                'Content-Type': 'application/json'
            }
        });
    }

    deletePriority(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/priorities/${id}`, this.getAuthHeaders());
    }

    // SLAs
    getSLAs(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/slas`, this.getAuthHeaders());
    }

    updateSLA(id: number, responseHours: number): Observable<any> {
        return this.http.put(`${this.apiUrl}/slas/${id}`, responseHours, {
            headers: {
                ...this.getAuthHeaders().headers,
                'Content-Type': 'application/json'
            }
        });
    }
}
