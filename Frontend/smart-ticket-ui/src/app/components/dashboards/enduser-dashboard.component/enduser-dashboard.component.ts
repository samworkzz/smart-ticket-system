import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { TicketService } from '../../../services/ticket.service';

import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { CreateTicketComponent } from '../../tickets/create-ticket/create-ticket';

@Component({
  selector: 'app-enduser-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    CreateTicketComponent
  ],
  templateUrl: './enduser-dashboard.component.html',
  styleUrls: ['./enduser-dashboard.component.css']
})
export class EnduserDashboardComponent implements OnInit {

  currentView: 'list' | 'create' = 'create';

  tickets: any[] = [];
  loading = false;

  displayedColumns: string[] = [
    'ticketId',
    'title',
    'category',
    'priority',
    'status',
    'createdAt',
    'actions'
  ];

  constructor(
    private authService: AuthService,
    private ticketService: TicketService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) { }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadTickets();
    }
  }

  loadTickets(): void {
    this.loading = true;

    this.ticketService.getMyTickets().subscribe({
      next: (data) => {
        this.tickets = Array.isArray(data) ? data : [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Ticket load failed', err);
        this.tickets = [];
        this.loading = false;
      }
    });
  }

  onTicketCreated(): void {
    this.currentView = 'list';
    this.loadTickets(); // refresh list
  }

  viewDetails(ticketId: number): void {
    this.router.navigate(['/ticket', ticketId]);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
