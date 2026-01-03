import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../../services/ticket.service';

import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-assigned-tickets',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './assigned-tickets.html',
  styleUrls: ['./assigned-tickets.css']
})
export class AssignedTicketsComponent implements OnInit {

  displayedColumns: string[] = [
    'ticketId',
    'title',
    'category',
    'priority',
    'status',
    'createdAt'
  ];

  tickets: any[] = [];
  loading = true;

  constructor(private ticketService: TicketService) {}

  ngOnInit(): void {
    this.ticketService.getAssignedTickets().subscribe({
      next: (data) => {
        this.tickets = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
