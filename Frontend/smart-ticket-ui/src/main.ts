import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';


import { appConfig } from './app/app.config';
import { mergeApplicationConfig } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

const browserConfig = mergeApplicationConfig(appConfig, {
  providers: [provideAnimationsAsync()]
});

bootstrapApplication(App, browserConfig)
  .catch((err) => console.error(err));