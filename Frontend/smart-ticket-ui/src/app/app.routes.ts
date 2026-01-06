import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login.component/login.component';
import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';
import { EnduserDashboardComponent } from './components/dashboards/enduser-dashboard.component/enduser-dashboard.component';
import { AgentDashboardComponent } from './components/dashboards/agent-dashboard.component/agent-dashboard.component';
import { ManagerDashboardComponent } from './components/dashboards/manager-dashboard.component/manager-dashboard.component';
import { AdminDashboardComponent } from './components/dashboards/admin-dashboard.component/admin-dashboard.component';
import { CreateTicketComponent } from './components/tickets/create-ticket/create-ticket';
import { RegisterComponent } from './components/auth/register.component/register.component';
import { MyTicketsComponent } from './components/tickets/my-tickets/my-tickets';
export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'enduser',
    component: EnduserDashboardComponent,
    canActivate: [authGuard, roleGuard],
    data: { role: 'EndUser' }
  },
  {
    path: 'manager',
    loadComponent: () => import('./components/dashboards/manager-dashboard.component/manager-dashboard.component').then(m => m.ManagerDashboardComponent),
    canActivate: [authGuard, roleGuard],
    data: { role: 'SupportManager' }
  },
  {
    path: 'agent',
    loadComponent: () => import('./components/dashboards/agent-dashboard.component/agent-dashboard.component').then(m => m.AgentDashboardComponent),
    canActivate: [authGuard, roleGuard],
    data: { role: 'SupportAgent' }
  },
  {
    path: 'admin-reports',
    loadComponent: () => import('./components/reports/admin-report/admin-report.component').then(m => m.AdminReportComponent),
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' }
  },
  {
    path: 'ticket/:id',
    loadComponent: () => import('./components/ticket-details/ticket-details.component').then(m => m.TicketDetailsComponent),
    canActivate: [authGuard] // Accessible to all roles (Backend handles specific restrictions)
  },
  {
    path: 'admin',
    component: AdminDashboardComponent,
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' }
  },
  {
    path: 'reports',
    loadComponent: () => import('./components/reports/reports.component').then(m => m.ReportsComponent),
    canActivate: [authGuard, roleGuard],
    data: { role: ['Admin', 'SupportManager'] }
  },
  {
    path: 'agent-reports',
    loadComponent: () => import('./components/reports/agent-report/agent-report.component').then(m => m.AgentReportComponent),
    canActivate: [authGuard, roleGuard],
    data: { role: 'SupportAgent' }
  },

  {
    path: 'create-ticket',
    component: CreateTicketComponent,
    canActivate: [authGuard, roleGuard],
    data: { role: 'EndUser' }
  },

  {
    path: 'my-tickets',
    component: MyTicketsComponent,
    canActivate: [authGuard, roleGuard],
    data: { role: 'EndUser' }
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' }
];
