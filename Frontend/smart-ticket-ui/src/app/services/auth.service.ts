import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'http://localhost:5125/api/auth';

  constructor(private http: HttpClient) { }

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, {
      email,
      password
    });
  }

  register(data: any) {
    return this.http.post(`${this.apiUrl}/register`, data);
  }


  storeToken(token: string): void {
    if (typeof window !== 'undefined') {
      localStorage.setItem('token', token);
      // Also store role for easier access
      const role = this.decodeRole(token);
      if (role) {
        localStorage.setItem('role', role);
      }
    }
  }

  getToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem('token');
  }

  logout(): void {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('token');
      localStorage.removeItem('role');
    }
  }

  getUserRole(): string | null {
    if (typeof window === 'undefined') return null;
    const storedRole = localStorage.getItem('role');
    if (storedRole) return storedRole;

    const token = this.getToken();
    return token ? this.decodeRole(token) : null;
  }

  private decodeRole(token: string): string | null {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));

      const payload = JSON.parse(jsonPayload);
      return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
    } catch (e) {
      console.error('Error decoding role from token', e);
      return null;
    }
  }



}
