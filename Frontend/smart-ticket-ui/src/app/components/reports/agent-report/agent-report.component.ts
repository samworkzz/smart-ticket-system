import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../services/ticket.service';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatDividerModule } from '@angular/material/divider';

@Component({
    selector: 'app-agent-report',
    standalone: true,
    imports: [
        CommonModule,
        NgxChartsModule,
        MatCardModule,
        MatButtonModule,
        MatIconModule,
        MatTableModule,
        MatDividerModule
    ],
    templateUrl: './agent-report.component.html',
    styleUrls: ['./agent-report.component.css']
})
export class AgentReportComponent implements OnInit {

    report: any = null;
    isLoading = true;
    errorMessage = '';

    // Charts
    slaData: any[] = [];
    slaColorScheme: any = {
        domain: ['#F44336', '#4CAF50'] // Red for Breached, Green for Met (implied/remainder)
        // Actually, backend returns Breached count. 
        // Ideally we'd want "Met" too for a pie chart. 
        // AgentReportDto has "ResolvedCount" but not specifically "Resolved Met SLA".
        // For now we can show Breached vs Non-Breached (Assumed Met or In Progress within SLA)
    };

    displayedColumns: string[] = ['id', 'title', 'elapsed', 'assignedTo'];

    constructor(
        private ticketService: TicketService,
        private cdr: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.loadReport();
    }

    loadReport() {
        this.isLoading = true;
        this.ticketService.getAgentReports().subscribe({
            next: (data) => {
                this.report = data;
                this.formatCharts();
                this.isLoading = false;
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('Error loading agent report', err);
                this.errorMessage = 'Failed to load report.';
                this.isLoading = false;
                this.cdr.detectChanges();
            }
        });
    }

    formatCharts() {
        if (!this.report) return;

        // SLA Breakdown
        // If we only have Breached Count and Total Tickets (or Escalated Count)
        // Let's deduce:
        // Breached = report.slaBreachedCount
        // "Safe" = report.totalAssigned - report.slaBreachedCount (Rough approximation)

        // Better: Breached vs Resolved (if we assume resolved are met, which isn't always true)
        // Let's stick to what we have: Escalated (Breached) vs Not Escalated.

        const breached = this.report.slaBreachedCount || 0;
        const safe = (this.report.totalAssigned || 0) - breached;

        this.slaData = [
            { name: 'Breached/Escalated', value: breached },
            { name: 'Within SLA', value: safe < 0 ? 0 : safe }
        ];
    }

    downloadReport() {
        window.print();
    }
}
