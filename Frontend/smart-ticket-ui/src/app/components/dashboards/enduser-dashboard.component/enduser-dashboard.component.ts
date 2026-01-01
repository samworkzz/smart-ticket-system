import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-enduser-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <h2>End User Dashboard</h2>

    <a routerLink="/enduser/create-ticket">Create Ticket</a>

    <button (click)="logout()">Logout</button>

  `
})
export class EnduserDashboardComponent {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
