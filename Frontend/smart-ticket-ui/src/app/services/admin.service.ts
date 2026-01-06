import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedRequest, PagedResponse } from '../models/shared.models';
import { HttpParams } from '@angular/common/http';

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
    // Priorities
    getPriorities(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/priorities`, this.getAuthHeaders());
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

    // User Management
    getUsers(pagination?: PagedRequest): Observable<PagedResponse<any>> {
        let params = new HttpParams();
        if (pagination) {
            params = params.set('pageNumber', pagination.pageNumber)
                .set('pageSize', pagination.pageSize);
            if (pagination.sortBy) params = params.set('sortBy', pagination.sortBy);
            if (pagination.sortDescending !== undefined) params = params.set('sortDescending', pagination.sortDescending);
        }
        return this.http.get<PagedResponse<any>>(`${this.apiUrl}/users`, { ...this.getAuthHeaders(), params });
    }

    getRoles(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/roles`, this.getAuthHeaders());
    }

    updateUserRole(userId: number, roleId: number): Observable<any> {
        return this.http.put(`${this.apiUrl}/users/role`, { userId, roleId }, {
            headers: {
                ...this.getAuthHeaders().headers,
                'Content-Type': 'application/json'
            }
        });
    }

    getAdminReport(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/reports`, this.getAuthHeaders());
    }
}
