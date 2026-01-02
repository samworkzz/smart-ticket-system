import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { NavbarComponent } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl:'manager-dashboard.component.html',
  styleUrl:'manager-dashboard.component.css'
})
export class ManagerDashboardComponent {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
