import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { TicketService } from '../../services/ticket.service';
import { FormsModule } from '@angular/forms';
import { TicketCommentService } from '../../services/ticket-comment.service';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
    selector: 'app-ticket-details',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        MatCardModule,
        MatButtonModule,
        MatIconModule,
        MatTabsModule,
        MatFormFieldModule,
        MatInputModule,
        MatListModule,
        MatDividerModule,
        MatProgressBarModule
    ],
    templateUrl: './ticket-details.component.html',
    styleUrls: ['./ticket-details.component.css']
})
export class TicketDetailsComponent implements OnInit {

    ticket: any = null;
    isLoading = true;
    newComment = '';
    isSubmittingComment = false;

    constructor(
        private route: ActivatedRoute,
        private ticketService: TicketService,
        private commentService: TicketCommentService,
        private cdr: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        console.log('TicketDetailsComponent initialized');
        const id = this.route.snapshot.paramMap.get('id');
        console.log('Ticket ID from route:', id);
        if (id) {
            this.loadTicket(Number(id));
        } else {
            console.error('No Ticket ID found in route');
            this.isLoading = false;
        }
    }

    loadTicket(id: number): void {
        console.log('Loading ticket details for ID:', id);
        this.isLoading = true;
        this.ticketService.getTicketDetails(id).subscribe({
            next: (data) => {
                console.log('Ticket data received:', data);
                this.ticket = data;
                setTimeout(() => {
                    this.isLoading = false;
                    this.cdr.detectChanges();
                });
            },
            error: (err) => {
                console.error('Failed to load ticket', err);
                setTimeout(() => {
                    this.isLoading = false;
                    this.cdr.detectChanges();
                });
            }
        });
    }

    submitComment(): void {
        if (!this.newComment.trim()) return;

        this.isSubmittingComment = true;
        const payload = {
            ticketId: this.ticket.ticketId,
            comment: this.newComment
        };

        this.commentService.addComment(payload).subscribe({
            next: () => {
                this.newComment = '';
                this.isSubmittingComment = false;
                // Refresh details to see new comment and log
                this.loadTicket(this.ticket.ticketId);
            },
            error: (err) => {
                alert('Failed to add comment');
                this.isSubmittingComment = false;
            }
        });
    }

    reopen(): void {
        if (!confirm('Are you sure you want to reopen this ticket?')) return;
        this.isLoading = true;
        this.ticketService.reopenTicket(this.ticket.ticketId).subscribe({
            next: () => this.loadTicket(this.ticket.ticketId),
            error: (err) => {
                alert('Failed to reopen ticket');
                this.isLoading = false;
            }
        });
    }

    cancel(): void {
        if (!confirm('Are you sure you want to cancel this ticket?')) return;
        this.isLoading = true;
        this.ticketService.cancelTicket(this.ticket.ticketId).subscribe({
            next: () => this.loadTicket(this.ticket.ticketId),
            error: (err) => {
                alert('Failed to cancel ticket');
                this.isLoading = false;
            }
        });
    }

    canReopen(): boolean {
        return this.ticket?.status === 'Closed' || this.ticket?.status === 'Resolved';
    }

    canCancel(): boolean {
        return this.ticket?.status !== 'Closed' && this.ticket?.status !== 'Resolved';
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
        return status?.replace(/\s+/g, '-').toLowerCase() || '';
    }
}
