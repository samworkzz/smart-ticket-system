import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../../services/ticket.service';
import { PagedRequest } from '../../../../models/shared.models';

import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';

@Component({
  selector: 'app-assigned-tickets',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatSortModule
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
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  sortBy = 'CreatedAt';
  sortDescending = true;

  constructor(private ticketService: TicketService) { }

  ngOnInit(): void {
    this.loadTickets();
  }

  loadTickets(): void {
    this.loading = true;
    const request: PagedRequest = {
      pageNumber: this.pageIndex + 1,
      pageSize: this.pageSize,
      sortBy: this.sortBy,
      sortDescending: this.sortDescending
    };

    this.ticketService.getAssignedTickets(request).subscribe({
      next: (data) => {
        this.tickets = data.items;
        this.totalCount = data.totalCount;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadTickets();
  }

  onSortChange(sort: Sort): void {
    this.sortBy = sort.active;
    this.sortDescending = sort.direction === 'desc';
    this.loadTickets();
  }
}
