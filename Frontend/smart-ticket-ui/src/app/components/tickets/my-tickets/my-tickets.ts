import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../services/ticket.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TicketDetailsComponent } from '../ticket-details/ticket-details';
@Component({
  selector: 'app-my-tickets',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatDialogModule,
  ],
  templateUrl: './my-tickets.html',
  styleUrls: ['./my-tickets.css']
})
export class MyTicketsComponent implements OnInit {

  displayedColumns: string[] = [
    'ticketId',
    'title',
    'category',
    'priority',
    'status',
    'createdAt',
    'actions'
  ];

  tickets: any[] = [];
  loading = true;

  constructor(private ticketService: TicketService, private dialog: MatDialog,  @Inject(PLATFORM_ID) private platformId: Object) {}

    ngOnInit(): void {
    // 🚨 CRITICAL FIX
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
      error: () => {
        this.tickets = [];
        this.loading = false;
      }
    });
  }
  openDetails(ticket: any): void {
  this.dialog.open(TicketDetailsComponent, {
    width: '500px',
    data: ticket
  });
}
}
