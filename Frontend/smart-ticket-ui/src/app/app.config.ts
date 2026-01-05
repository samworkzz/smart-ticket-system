import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './interceptors/auth.interceptor';

import { routes } from './app.routes';


export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    // provideAnimationsAsync() removed, moving to main.ts

    // 2. Add this line to enable HttpClient with Fetch
    provideHttpClient(
      withFetch(),
      withInterceptors([authInterceptor])
    )
  ]
};