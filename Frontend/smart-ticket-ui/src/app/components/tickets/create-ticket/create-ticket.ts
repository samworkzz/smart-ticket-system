import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TicketService } from '../../../services/ticket.service';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-create-ticket',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule
  ],
  templateUrl: './create-ticket.html',
  styleUrls: ['./create-ticket.css']
})
export class CreateTicketComponent {

  @Output() ticketCreated = new EventEmitter<void>();

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

        // 🔑 Notify parent dashboard
        this.ticketCreated.emit();
      },
      error: () => {
        this.errorMessage = 'Failed to create ticket';
        this.successMessage = '';
      }
    });
  }
}
