import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs'; // 1. Import BehaviorSubject

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'https://localhost:7088/api/auth';
  
  // 2. Create the Subject to track the role state
  // We initialize it with null, then update it in the constructor
  private roleSubject = new BehaviorSubject<string | null>(null);
  
  // 3. Expose it as an Observable for the Navbar to subscribe to
  public role$ = this.roleSubject.asObservable();

  constructor(private http: HttpClient) {
    // 4. On application load, check if we already have a user
    if (this.isBrowser()) {
      const currentRole = this.getUserRole();
      this.roleSubject.next(currentRole);
    }
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, {
      email,
      password
    });
  }

  register(data: any) {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  getToken(): string | null {
    if (!this.isBrowser()) {
      return null;
    }
    return localStorage.getItem('token');
  }

  // 5. Update this method to NOTIFY subscribers when token is stored
  storeToken(token: string): void {
    if (this.isBrowser()) {
      localStorage.setItem('token', token);
      
      // Calculate the role from the new token immediately
      const role = this.getUserRole(); 
      
      // Notify the Navbar (and everyone else) that the role changed!
      this.roleSubject.next(role);
    }
  }

  // 6. Update logout to clear the subject
  logout(): void {
    if (this.isBrowser()) {
      localStorage.removeItem('token');
      localStorage.removeItem('role'); // clear this too if you use it
      
      // Notify the Navbar that the user is gone
      this.roleSubject.next(null);
    }
  }

  // You can keep this, but getUserRole() is safer as it uses the token
  getRole(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem('role');
  }

  storeUserData(token: string, role: string) {
    if (this.isBrowser()) {
      localStorage.setItem('token', token);
      localStorage.setItem('role', role);
      
      // Notify subscribers
      this.roleSubject.next(role); 
    }
  }

  getUserRole(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      // Decode the JWT
      const payload = JSON.parse(atob(token.split('.')[1]));
      // Return the specific claim for role
      return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    } catch (e) {
      return null;
    }
  }

  private isBrowser(): boolean {
    return typeof window !== 'undefined';
  }
}