import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { AdminService } from '../../../services/admin.service';
import { FormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PagedRequest } from '../../../models/shared.models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTabsModule,
    MatTableModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatIconModule,
    MatSelectModule,
    MatPaginatorModule,
    MatSortModule,
    MatTooltipModule
  ],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {

  categories: any[] = [];
  priorities: any[] = [];
  slas: any[] = [];
  users: any[] = [];
  availableRoles: any[] = [];

  newCategoryName = '';
  newPriorityName = '';

  isLoading = false;

  // Pagination for Users
  usersTotalCount = 0;
  usersPageSize = 10;
  usersPageIndex = 0;
  usersSortBy = 'Name';
  usersSortDescending = false;

  constructor(
    private authService: AuthService,
    private adminService: AdminService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadData();
    this.loadUsers();
  }

  loadData(): void {
    this.isLoading = true;
    this.adminService.getCategories().subscribe(res => this.categories = res);
    this.adminService.getPriorities().subscribe(res => this.priorities = res);
    this.adminService.getSLAs().subscribe({
      next: (res) => {
        this.slas = res;
        setTimeout(() => this.isLoading = false);
      },
      error: (err) => {
        console.error('Failed to load SLAs', err);
        setTimeout(() => this.isLoading = false);
      }
    });

    this.adminService.getRoles().subscribe(res => this.availableRoles = res);
  }

  loadUsers(): void {
    const request: PagedRequest = {
      pageNumber: this.usersPageIndex + 1,
      pageSize: this.usersPageSize,
      sortBy: this.usersSortBy,
      sortDescending: this.usersSortDescending
    };

    this.adminService.getUsers(request).subscribe({
      next: (data) => {
        this.users = data.items;
        this.usersTotalCount = data.totalCount;
      },
      error: (err) => console.error('Failed to load users', err)
    });
  }

  onUsersPageChange(event: PageEvent): void {
    this.usersPageIndex = event.pageIndex;
    this.usersPageSize = event.pageSize;
    this.loadUsers();
  }

  onUsersSortChange(sort: Sort): void {
    this.usersSortBy = sort.active;
    this.usersSortDescending = sort.direction === 'desc';
    this.loadUsers();
  }

  // Categories
  addCategory(): void {
    if (!this.newCategoryName) return;
    this.adminService.createCategory(this.newCategoryName).subscribe({
      next: () => {
        this.newCategoryName = '';
        this.loadData();
      },
      error: (err) => alert(err.error?.Message || 'Failed to add category')
    });
  }

  deleteCategory(id: number): void {
    if (!confirm('Are you sure?')) return;
    this.adminService.deleteCategory(id).subscribe({
      next: () => this.loadData(),
      error: (err) => alert(err.error?.Message || 'Failed to delete category')
    });
  }

  // Priorities
  addPriority(): void {
    if (!this.newPriorityName) return;
    this.adminService.createPriority(this.newPriorityName).subscribe({
      next: () => {
        this.newPriorityName = '';
        this.loadData();
      },
      error: (err) => alert(err.error?.Message || 'Failed to add priority')
    });
  }

  deletePriority(id: number): void {
    if (!confirm('Are you sure?')) return;
    this.adminService.deletePriority(id).subscribe({
      next: () => this.loadData(),
      error: (err) => alert(err.error?.Message || 'Failed to delete priority')
    });
  }

  // SLAs
  updateSLA(sla: any): void {
    this.adminService.updateSLA(sla.slaId, sla.responseHours).subscribe({
      next: () => console.log('SLA updated'),
      error: (err) => alert('Failed to update SLA')
    });
  }

  // User Management
  updateUserRole(user: any): void {
    this.adminService.updateUserRole(user.userId, user.roleId).subscribe({
      next: (res) => {
        alert(res.Message || 'User role updated successfully');
        this.loadUsers(); // Refresh users list
      },
      error: (err) => {
        alert(err.error?.Message || 'Failed to update user role');
        this.loadUsers(); // Revert UI
      }
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
