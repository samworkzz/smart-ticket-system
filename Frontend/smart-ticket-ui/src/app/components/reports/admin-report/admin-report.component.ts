import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../services/admin.service';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatDividerModule } from '@angular/material/divider';
import { NgxChartsModule } from '@swimlane/ngx-charts';

@Component({
    selector: 'app-admin-report',
    standalone: true,
    imports: [CommonModule, MatCardModule, MatTableModule, MatDividerModule, NgxChartsModule],
    templateUrl: './admin-report.component.html',
    styleUrls: ['./admin-report.component.css']
})
export class AdminReportComponent implements OnInit {

    report: any = null;
    isLoading = true;
    errorMessage = '';

    // Charts
    ticketStatusData: any[] = [];
    colorScheme: any = {
        domain: ['#5AA454', '#A10A28', '#C7B42C', '#AAAAAA']
    };

    displayedColumnsManagers: string[] = ['id', 'name', 'email'];
    displayedColumnsAgents: string[] = ['id', 'name', 'resolved', 'avgTime'];

    constructor(
        private adminService: AdminService,
        private cdr: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.loadReport();
    }

    loadReport() {
        this.isLoading = true;
        this.adminService.getAdminReport().subscribe({
            next: (data) => {
                this.report = data;
                this.formatCharts();
                this.isLoading = false;
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('Error loading admin report', err);
                this.errorMessage = 'Failed to load report.';
                this.isLoading = false;
                this.cdr.detectChanges();
            }
        });
    }

    formatCharts() {
        if (!this.report) return;

        this.ticketStatusData = [
            { name: 'Open', value: this.report.openTickets },
            { name: 'Resolved', value: this.report.resolvedTickets },
            { name: 'Escalated', value: this.report.escalatedTickets }
        ];
    }
}
