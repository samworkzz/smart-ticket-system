import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagerService } from '../../../services/manager.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { AgentWorkload, UnassignedTicket } from '../../../models/manager.models';

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
    MatListModule
  ],
  templateUrl: './manager-dashboard.component.html',
  styleUrl: './manager-dashboard.component.css'
})
export class ManagerDashboardComponent implements OnInit {

  agents: AgentWorkload[] = [];
  unassignedTickets: UnassignedTicket[] = [];
  isLoading = false;

  constructor(private managerService: ManagerService, private router: Router) { }

  ngOnInit(): void {
    this.loadData();
  }

  viewDetails(ticketId: number): void {
    this.router.navigate(['/ticket', ticketId]);
  }

  loadData(): void {
    this.isLoading = true;
    // Use forkJoin if RxJS is imported, or just separate calls for simplicity now
    this.managerService.getAgents().subscribe({
      next: (data) => this.agents = data,
      error: (err) => console.error('Failed to load agents', err)
    });

    this.managerService.getUnassignedTickets().subscribe({
      next: (data) => {
        this.unassignedTickets = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load tickets', err);
        this.isLoading = false;
      }
    });
  }

  assign(ticketId: number, agentId: number): void {
    if (!agentId) return;

    this.isLoading = true;
    this.managerService.assignTicket(ticketId, agentId).subscribe({
      next: () => {
        this.loadData();
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
      case 'high': return 'badge-high';
      case 'medium': return 'badge-medium';
      case 'low': return 'badge-low';
      default: return 'badge-default';
    }
  }
}

