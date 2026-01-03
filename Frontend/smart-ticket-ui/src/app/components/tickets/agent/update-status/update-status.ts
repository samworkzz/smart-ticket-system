import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TicketService } from '../../../../services/ticket.service.js';


@Component({
  selector: 'app-update-status',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: 'update-status.html'
})
export class UpdateStatusComponent {

  ticketId!: number;
  ticketStatusId!: number;

  successMessage = '';
  errorMessage = '';

  constructor(private ticketService: TicketService) {}

  update() {
    this.ticketService.updateStatus(this.ticketId, this.ticketStatusId).subscribe({
      next: () => {
        this.successMessage = 'Status updated';
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'Update failed';
        this.successMessage = '';
      }
    });
  }
}
