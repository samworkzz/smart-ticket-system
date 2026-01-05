import { CanActivateFn, ActivatedRouteSnapshot, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const expectedRole = route.data['role'];
  const userRole = authService.getUserRole();

  let hasRole = false;
  if (Array.isArray(expectedRole)) {
    hasRole = expectedRole.includes(userRole);
  } else {
    hasRole = userRole === expectedRole;
  }

  if (hasRole) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
