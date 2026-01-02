import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TicketService } from '../../../services/ticket.service.ts';
import { NavbarComponent } from '../../../shared/navbar/navbar.js';

@Component({
  selector: 'app-create-ticket',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './create-ticket.html'
})
export class CreateTicketComponent {

  title = '';
  description = '';
  ticketCategoryId = 1;
  ticketPriorityId = 1;

  successMessage = '';
  errorMessage = '';

  constructor(private ticketService: TicketService) {}

  submit(): void {
    const payload = {
      title: this.title,
      description: this.description,
      ticketCategoryId: this.ticketCategoryId,
      ticketPriorityId: this.ticketPriorityId
    };

    this.ticketService.createTicket(payload).subscribe({
      next: () => {
        this.successMessage = 'Ticket created successfully';
        this.errorMessage = '';
        this.title = '';
        this.description = '';
      },
      error: () => {
        this.errorMessage = 'Failed to create ticket';
        this.successMessage = '';
      }
    });
  }
}
