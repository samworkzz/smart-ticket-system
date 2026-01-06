import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../services/ticket.service';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-reports',
    standalone: true,
    imports: [CommonModule, NgxChartsModule, MatCardModule, MatProgressBarModule, MatButtonModule, MatIconModule],
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

    slaColorScheme: any = {
        domain: ['#4CAF50', '#F44336']
    };

    // Chart Data Holders
    statusData: any[] = [];
    priorityData: any[] = [];

    // Manager Charts
    workloadData: any[] = [];
    slaData: any[] = [];
    performanceData: any[] = [];

    managerReport: any = null;

    constructor(private ticketService: TicketService, private cdr: ChangeDetectorRef) { }

    ngOnInit(): void {
        this.loadMetrics();
    }

    loadMetrics(): void {
        this.isLoading = true;

        // Load basic metrics
        this.ticketService.getMetrics().subscribe({
            next: (data) => {
                this.metrics = data;
                this.formatBasicCharts();
                this.cdr.detectChanges();
            },
            error: (err) => console.error('Failed to load metrics', err)
        });

        // Load Manager Reports
        this.ticketService.getManagerReports().subscribe({
            next: (data) => {
                this.managerReport = data;
                this.formatManagerCharts();
                this.isLoading = false;
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('Failed to load manager reports', err);
                this.isLoading = false;
            }
        });
    }

    formatBasicCharts() {
        if (!this.metrics) return;
        this.statusData = this.metrics.statusMetrics;
        this.priorityData = this.metrics.priorityMetrics;
    }

    formatManagerCharts() {
        if (!this.managerReport) return;

        // 1. Workload: Stacked Bar (Open vs In Progress)
        this.workloadData = this.managerReport.agentWorkload.map((a: any) => {
            return {
                name: a.agentName,
                series: [
                    { name: 'Open', value: a.openTickets },
                    { name: 'In Progress', value: a.inProgressTickets }
                ]
            };
        });

        // 2. SLA: Pie Chart (Met vs Breached)
        this.slaData = [
            { name: 'Met SLA', value: this.managerReport.slaCompliance.metSlaCount },
            { name: 'Breached SLA', value: this.managerReport.slaCompliance.breachedSlaCount }
        ];

        // 3. Performance: Bar Chart (Avg Resolution Time)
        this.performanceData = this.managerReport.agentPerformance.map((a: any) => {
            return {
                name: a.agentName,
                value: a.avgResolutionHours
            };
        });
    }

    downloadPdf(): void {
        window.print();
    }
}
