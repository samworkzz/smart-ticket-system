import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import {jwtDecode} from 'jwt-decode';
import { AuthResponse, LoginRequest, RegisterRequest } from './auth.models';

interface JwtPayload {
  sub: string;
  email: string;
  role: string;
  exp: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private readonly API = 'https://localhost:5001/api/auth';
  private readonly TOKEN_KEY = 'auth_token';

  login(data: LoginRequest) {
    return this.http.post<AuthResponse>(`${this.API}/login`, data);
  }

  register(data: RegisterRequest) {
    return this.http.post<AuthResponse>(`${this.API}/register`, data);
  }

  saveToken(token: string) {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    const decoded = jwtDecode<JwtPayload>(token);
    return decoded.exp * 1000 > Date.now();
  }

  getUserRole(): string | null {
    const token = this.getToken();
    if (!token) return null;

    return jwtDecode<JwtPayload>(token).role;
  }

  getUserId(): number | null {
    const token = this.getToken();
    if (!token) return null;

    return +jwtDecode<JwtPayload>(token).sub;
  }
}
