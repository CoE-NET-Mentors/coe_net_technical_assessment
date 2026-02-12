import { ApplicationConfig, APP_INITIALIZER } from '@angular/core';
import { provideRouter, withHashLocation } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { routes } from './app.routes';

function loadRuntimeConfig(): () => Promise<void> {
  // fetch runtime config from backend API (absolute URL to avoid dev server proxy issues)
  return () => fetch('http://localhost:5148/runtime-config')
    .then(res => res.json())
    .then(cfg => {
      (window as any).RUNTIME_CONFIG = cfg;
    })
    .catch(() => {
      // swallow errors; fallback to build-time environment
      (window as any).RUNTIME_CONFIG = null;
    });
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withHashLocation()),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: APP_INITIALIZER, useFactory: loadRuntimeConfig, multi: true }
  ]
};
