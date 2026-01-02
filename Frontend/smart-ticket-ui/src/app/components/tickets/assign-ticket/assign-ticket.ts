import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TicketService } from './../../../services/ticket.service.ts';
import { NavbarComponent } from '../../../shared/navbar/navbar.js';

@Component({
  selector: 'app-assign-ticket',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './assign-ticket.html'
})
export class AssignTicketComponent {

  ticketId!: number;
  agentUserId!: number;

  successMessage = '';
  errorMessage = '';

  constructor(private ticketService: TicketService) {}

  assign() {
    this.ticketService.assignTicket(this.ticketId, this.agentUserId).subscribe({
      next: () => {
        this.successMessage = 'Ticket assigned successfully';
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'Assignment failed';
        this.successMessage = '';
      }
    });
  }
}
