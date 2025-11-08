import {
  ApplicationConfig,
  provideZoneChangeDetection,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    // Performance optimization
    provideZoneChangeDetection({ eventCoalescing: true }),

    // Handles global browser errors (available in Angular 17+)
    provideBrowserGlobalErrorListeners(),

    // Set up routing
    provideRouter(routes),

    // Enable HttpClient globally
    provideHttpClient(),
  ],
};
