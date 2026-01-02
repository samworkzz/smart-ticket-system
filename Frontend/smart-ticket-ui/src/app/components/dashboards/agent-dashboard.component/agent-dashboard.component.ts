import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { NavbarComponent } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-agent-dashboard',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl:'agent-dashboard.component.html',
  styleUrl:'agent-dashboard.component.css'
})
export class AgentDashboardComponent {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
