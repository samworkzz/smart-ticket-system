import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { NavbarComponent } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl:'admin-dashboard.component.html',
  styleUrl:'admin-dashboard.component.css'
})
export class AdminDashboardComponent {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
