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
    component: ManagerDashboardComponent,
    canActivate: [authGuard, roleGuard],
    data: { role: 'SupportManager' }
  },
  {
    path: 'agent',
    component: AgentDashboardComponent,
    canActivate: [authGuard, roleGuard],
    data: { role: 'SupportAgent' }
  },
  {
    path: 'admin',
    component: AdminDashboardComponent,
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' }
  },
  
  {
  path: 'create-ticket',
  component: CreateTicketComponent,
  canActivate: [authGuard, roleGuard],
  data: { role: 'EndUser' }
},

// {
//   path:'my-tickets',
//   component: MyTicketsComponent,
//   canActivate:[authGuard,roleGuard],
//   data:{role:'EndUser'}
// },
  { path: '', redirectTo: 'login', pathMatch: 'full' }
];
