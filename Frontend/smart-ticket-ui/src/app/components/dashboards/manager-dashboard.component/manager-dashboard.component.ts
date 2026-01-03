import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagerService } from '../../../services/manager.service';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, MatTableModule, MatSelectModule],
  templateUrl:'./manager-dashboard.component.html',
  styleUrl:'./manager-dashboard.component.css' 
})
export class ManagerDashboardComponent implements OnInit {

  agents: any[] = [];
  unassignedTickets: any[] = [];

  constructor(private managerService: ManagerService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.managerService.getAgents().subscribe(a => this.agents = a);
    this.managerService.getUnassignedTickets().subscribe(t => this.unassignedTickets = t);
  }

  assign(ticketId: number, agentId: number): void {
    this.managerService.assignTicket(ticketId, agentId).subscribe(() => {
      this.loadData(); // refresh both tables
    });
  }
}

