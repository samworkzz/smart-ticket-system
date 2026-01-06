import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';

@Component({
    selector: 'app-notifications',
    standalone: true,
    imports: [CommonModule, MatCardModule, MatIconModule, MatButtonModule, MatBadgeModule, MatMenuModule],
    template: `
    <button mat-icon-button [matMenuTriggerFor]="menu" (click)="loadNotifications()">
      <mat-icon [matBadge]="unreadCount" [matBadgeHidden]="unreadCount === 0" matBadgeColor="warn">notifications</mat-icon>
    </button>
    <mat-menu #menu="matMenu" class="notification-menu">
      <div class="menu-header" (click)="$event.stopPropagation()">
        <h3>Notifications</h3>
        <button mat-button color="primary" (click)="refresh($event)">Refresh</button>
      </div>
      
      <div *ngIf="notifications.length === 0" class="empty-state">
        No notifications
      </div>

      <button mat-menu-item *ngFor="let n of notifications" [class.unread]="!n.isRead" (click)="markRead(n)">
        <mat-icon *ngIf="!n.isRead" color="primary">fiber_manual_record</mat-icon>
        <span class="message">{{ n.message }}</span>
        <span class="time">{{ n.createdAt | date:'short' }}</span>
      </button>
    </mat-menu>
  `,
    styles: [`
    .notification-menu {
      max-width: 350px !important;
    }
    .menu-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0 16px;
      border-bottom: 1px solid #eee;
    }
    .empty-state {
      padding: 20px;
      text-align: center;
      color: #888;
    }
    .unread {
      background-color: #f0f7ff;
      font-weight: 500;
    }
    .message {
      display: block;
      white-space: normal;
      font-size: 0.9rem;
      line-height: 1.2;
    }
    .time {
      display: block;
      font-size: 0.75rem;
      color: #999;
      margin-top: 4px;
    }
    ::ng-deep .mat-mdc-menu-content {
      max-height: 400px;
      overflow-y: auto;
    }
  `]
})
export class NotificationsComponent implements OnInit {

    notifications: any[] = [];
    unreadCount = 0;

    constructor(private notificationService: NotificationService) { }

    ngOnInit(): void {
        this.loadNotifications();
        // Poll every 30 seconds
        setInterval(() => this.loadNotifications(), 30000);
    }

    loadNotifications() {
        this.notificationService.getNotifications().subscribe(data => {
            this.notifications = data;
            this.unreadCount = data.filter(n => !n.isRead).length;
        });
    }

    refresh(event: Event) {
        event.stopPropagation();
        this.loadNotifications();
    }

    markRead(notification: any) {
        if (!notification.isRead) {
            this.notificationService.markAsRead(notification.notificationId).subscribe(() => {
                notification.isRead = true;
                this.unreadCount = Math.max(0, this.unreadCount - 1);
            });
        }
    }
}
