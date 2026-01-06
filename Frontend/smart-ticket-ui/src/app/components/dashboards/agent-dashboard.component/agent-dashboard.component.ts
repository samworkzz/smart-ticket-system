import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../services/ticket.service';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Router } from '@angular/router';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PagedRequest } from '../../../models/shared.models';

@Component({
  selector: 'app-agent-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTooltipModule
  ],
  templateUrl: './agent-dashboard.component.html',
  styleUrls: ['./agent-dashboard.component.css']
})
export class AgentDashboardComponent implements OnInit {

  tickets: any[] = [];
  isLoading = false;

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
    this.loadAssignedTickets();
  }

  loadAssignedTickets(): void {
    this.isLoading = true;

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
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Failed to load tickets', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadAssignedTickets();
  }

  onSortChange(sort: Sort): void {
    this.sortBy = sort.active;
    this.sortDescending = sort.direction === 'desc';
    this.loadAssignedTickets();
  }

  viewDetails(ticketId: number): void {
    this.router.navigate(['/ticket', ticketId]);
  }

  startWork(ticketId: number): void {
    this.ticketService.updateStatus(ticketId, 4).subscribe({ // 4 = In Progress
      next: () => {
        this.loadAssignedTickets();
        // optionally show toast
      },
      error: (err: any) => alert('Failed to start work')
    });
  }

  resolveTicket(ticketId: number): void {
    this.ticketService.updateStatus(ticketId, 5).subscribe({ // 5 = Resolved
      next: () => {
        this.loadAssignedTickets();
        // optionally show toast
      },
      error: (err: any) => alert('Failed to resolve ticket')
    });
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
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
