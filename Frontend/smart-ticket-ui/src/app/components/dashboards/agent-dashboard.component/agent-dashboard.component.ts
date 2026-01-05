import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../services/ticket.service';
import { AssignedTicket, TicketStatus } from '../../../models/agent.models';
import { Router } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-agent-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressBarModule
  ],
  templateUrl: './agent-dashboard.component.html',
  styleUrls: ['./agent-dashboard.component.css']
})
export class AgentDashboardComponent implements OnInit {

  tickets: AssignedTicket[] = [];
  isLoading = true;

  constructor(private ticketService: TicketService, private router: Router) { }

  ngOnInit(): void {
    this.loadAssignedTickets();
  }

  viewDetails(ticketId: number): void {
    this.router.navigate(['/ticket', ticketId]);
  }

  loadAssignedTickets(): void {
    this.isLoading = true;
    this.ticketService.getAssignedTickets().subscribe({
      next: (data) => {
        this.tickets = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load tickets', err);
        this.isLoading = false;
      }
    });
  }

  startWork(ticketId: number): void {
    this.updateStatus(ticketId, TicketStatus.InProgress);
  }

  closeTicket(ticketId: number): void {
    if (!confirm('Are you sure you want to close this ticket? This will notify the user and managers.')) return;
    this.updateStatus(ticketId, TicketStatus.Closed);
  }

  private updateStatus(ticketId: number, statusId: number): void {
    this.isLoading = true; // Simple global loading for now
    this.ticketService.updateStatus(ticketId, statusId).subscribe({
      next: () => {
        this.loadAssignedTickets(); // Reload to refresh UI
      },
      error: (err) => {
        alert('Failed to update status: ' + err.message);
        this.isLoading = false;
      }
    });
  }

  getPriorityClass(priority: string): string {
    switch (priority?.toLowerCase()) {
      case 'high': return 'priority-high';
      case 'medium': return 'priority-medium';
      case 'low': return 'priority-low';
      default: return '';
    }
  }

  getStatusClass(status: string): string {
    // Backend returns string like "In Progress", "Closed"
    return status.replace(/\s+/g, '-').toLowerCase();
  }
}
