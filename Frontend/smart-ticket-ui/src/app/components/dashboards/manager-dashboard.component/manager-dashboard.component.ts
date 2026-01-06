import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagerService } from '../../../services/manager.service';
import { TicketService } from '../../../services/ticket.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTabsModule } from '@angular/material/tabs';
import { AgentWorkload, UnassignedTicket } from '../../../models/manager.models';
import { PagedRequest } from '../../../models/shared.models';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatSelectModule,
    MatCardModule,
    MatButtonModule,
    MatProgressBarModule,
    MatIconModule,
    MatListModule,
    MatPaginatorModule,
    MatSortModule,
    MatTooltipModule,
    MatTabsModule
  ],
  templateUrl: './manager-dashboard.component.html',
  styleUrl: './manager-dashboard.component.css'
})
export class ManagerDashboardComponent implements OnInit {

  agents: AgentWorkload[] = [];
  unassignedTickets: UnassignedTicket[] = [];
  resolvedTickets: any[] = []; // Tickets waiting for closure
  isLoading = false;

  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  sortBy = 'CreatedAt';
  sortDescending = true;

  displayedColumns: string[] = ['id', 'title', 'category', 'priority', 'createdAt', 'assign', 'actions'];
  resolvedColumns: string[] = ['id', 'title', 'category', 'priority', 'assignedTo', 'createdAt', 'actions'];

  constructor(
    private managerService: ManagerService,
    private ticketService: TicketService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  viewDetails(ticketId: number): void {
    this.router.navigate(['/ticket', ticketId]);
  }

  loadData(): void {
    this.isLoading = true;
    this.managerService.getAgents().subscribe({
      next: (data) => this.agents = data,
      error: (err) => console.error('Failed to load agents', err)
    });

    this.loadUnassignedTickets();
    this.loadResolvedTickets();
  }

  loadResolvedTickets(): void {
    this.ticketService.getAllTickets().subscribe({
      next: (data) => {
        // Filter for 'Resolved' status (Case insensitive check just in case)
        this.resolvedTickets = data.filter(t => t.status?.toLowerCase() === 'resolved');
        // Note: getAllTickets returns all, filtering client side. 
        // For larger details, we'd want a backend filter endpoint.
      },
      error: (err) => console.error('Failed to load resolved tickets', err)
    });
  }

  closeTicket(ticketId: number): void {
    if (!confirm('Are you sure you want to CLOSE this ticket? This action is final.')) return;

    this.isLoading = true;
    // Assuming 6 is the ID for "Closed"
    this.ticketService.updateStatus(ticketId, 6).subscribe({
      next: () => {
        this.loadResolvedTickets(); // Refresh list
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to close ticket', err);
        this.isLoading = false;
        alert('Failed to close ticket');
      }
    });
  }

  loadUnassignedTickets(): void {
    const request: PagedRequest = {
      pageNumber: this.pageIndex + 1,
      pageSize: this.pageSize,
      sortBy: this.sortBy,
      sortDescending: this.sortDescending
    };

    this.managerService.getUnassignedTickets(request).subscribe({
      next: (data) => {
        this.unassignedTickets = data.items;
        this.totalCount = data.totalCount;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load tickets', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadUnassignedTickets();
  }

  onSortChange(sort: Sort): void {
    this.sortBy = sort.active;
    this.sortDescending = sort.direction === 'desc';
    this.loadUnassignedTickets();
  }

  assign(ticketId: number, agentId: number): void {
    if (!agentId) return;

    this.isLoading = true;
    this.managerService.assignTicket(ticketId, agentId).subscribe({
      next: () => {
        this.loadUnassignedTickets();
        this.managerService.getAgents().subscribe(data => this.agents = data); // Refresh workload
        // Optional: Show success toast
      },
      error: (err) => {
        console.error('Assignment failed', err);
        this.isLoading = false;
        alert('Failed to assign ticket: ' + (err.error?.message || err.message));
      }
    });
  }

  getPriorityClass(priority: string): string {
    switch (priority?.toLowerCase()) {
      case 'high': return 'priority-high'; // Using global priority classes where possible
      case 'medium': return 'priority-medium';
      case 'low': return 'priority-low';
      default: return 'priority-default';
    }
  }
}
