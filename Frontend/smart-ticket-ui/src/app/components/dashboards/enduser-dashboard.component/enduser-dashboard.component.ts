import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../services/ticket.service';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Router, RouterModule } from '@angular/router';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PagedRequest } from '../../../models/shared.models';

@Component({
  selector: 'app-enduser-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    RouterModule,
    MatTooltipModule
  ],
  templateUrl: './enduser-dashboard.component.html',
  styleUrls: ['./enduser-dashboard.component.css']
})
export class EnduserDashboardComponent implements OnInit {

  tickets: any[] = [];
  loading = false;

  // Pagination & Sorting state
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  sortBy = 'CreatedAt';
  sortDescending = true;

  displayedColumns: string[] = ['id', 'title', 'category', 'priority', 'status', 'createdAt', 'actions'];

  constructor(
    private ticketService: TicketService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

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

    this.ticketService.getMyTickets(request).subscribe({
      next: (data) => {
        this.tickets = data.items;
        this.totalCount = data.totalCount;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load tickets', err);
        this.loading = false;
        this.cdr.detectChanges();
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

  viewDetails(ticketId: number): void {
    this.router.navigate(['/ticket', ticketId]);
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'created': return 'status-created';
      case 'open': return 'status-open';
      case 'assigned': return 'status-assigned';
      case 'in progress': return 'status-in-progress';
      case 'resolved': return 'status-resolved';
      case 'closed': return 'status-closed';
      default: return 'status-default';
    }
  }

  getPriorityClass(priority: string): string {
    switch (priority?.toLowerCase()) {
      case 'high': return 'priority-high';
      case 'medium': return 'priority-medium';
      case 'low': return 'priority-low';
      default: return 'priority-default';
    }
  }
}
