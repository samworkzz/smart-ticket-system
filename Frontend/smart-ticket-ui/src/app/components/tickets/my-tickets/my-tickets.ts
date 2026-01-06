import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../services/ticket.service';
import { MatTableModule } from '@angular/material/table';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { PagedRequest } from '../../../models/shared.models';

@Component({
  selector: 'app-my-tickets',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatSortModule
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
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  sortBy = 'CreatedAt';
  sortDescending = true;

  constructor(private ticketService: TicketService, private router: Router, @Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadTickets();
    }
  }

  loadTickets(): void {
    this.loading = true;

    const request: PagedRequest = {
      pageNumber: this.pageIndex + 1,
      pageSize: this.pageSize,
      sortBy: this.sortBy,
      sortDescending: this.sortDescending
    };

    this.ticketService.getMyTickets(request).subscribe({
      next: (data) => {
        this.tickets = data.items;
        this.totalCount = data.totalCount;
        this.loading = false;
      },
      error: () => {
        this.tickets = [];
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

  openDetails(ticket: any): void {
    this.router.navigate(['/ticket', ticket.ticketId]);
  }
}
