import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {

  private isBrowser = typeof window !== 'undefined';

  constructor(private router: Router) {}

  isLoggedIn(): boolean {
    if (!this.isBrowser) return false;
    return !!localStorage.getItem('token');
  }

  getRole(): string | null {
    if (!this.isBrowser) return null;
    return localStorage.getItem('role');
  }

  logout(): void {
    if (this.isBrowser) {
      localStorage.clear();
    }
    this.router.navigate(['/login']);
  }
}
