import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: 
  [CommonModule, 
    FormsModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
  RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  email = '';
  password = '';
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login(): void {
    this.authService.login(this.email, this.password).subscribe({
      next: (res) => {
        this.authService.storeToken(res.token);

        const role = this.authService.getUserRole();

        if (role === 'EndUser') {
          this.router.navigate(['/enduser']);
        } else if (role === 'SupportManager') {
          this.router.navigate(['/manager']);
        } else if (role === 'SupportAgent') {
          this.router.navigate(['/agent']);
        } else if (role === 'Admin') {
          this.router.navigate(['/admin']);
        }
      },
      error: () => {
        this.errorMessage = 'Invalid email or password';
      }
    });
  }
}
