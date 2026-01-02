import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { NavbarComponent } from '../../../shared/navbar/navbar';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-enduser-dashboard',
  standalone: true,
  imports: [
  CommonModule,
  NavbarComponent,
  MatCardModule,
  MatButtonModule
],
  templateUrl: 'enduser-dashboard.component.html',
  styleUrl:'enduser-dashboard.component.css'
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
