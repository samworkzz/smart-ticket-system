import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../services/ticket.service';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
    selector: 'app-reports',
    standalone: true,
    imports: [CommonModule, NgxChartsModule, MatCardModule, MatProgressBarModule],
    templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.css']
})
export class ReportsComponent implements OnInit {

    metrics: any = null;
    isLoading = true;

    // Chart options
    colorScheme: any = {
        domain: ['#5AA454', '#A10A28', '#C7B42C', '#AAAAAA', '#2196F3', '#FF9800']
    };

    constructor(private ticketService: TicketService, private cdr: ChangeDetectorRef) { }

    ngOnInit(): void {
        console.log('ReportsComponent initialized');
        this.loadMetrics();
    }

    loadMetrics(): void {
        console.log('Loading metrics...');
        this.isLoading = true;
        this.ticketService.getMetrics().subscribe({
            next: (data) => {
                console.log('Metrics received:', data);
                this.metrics = data;
                setTimeout(() => {
                    this.isLoading = false;
                    this.cdr.detectChanges();
                });
            },
            error: (err) => {
                console.error('Failed to load metrics', err);
                setTimeout(() => {
                    this.isLoading = false;
                    this.cdr.detectChanges();
                });
            }
        });
    }
}
