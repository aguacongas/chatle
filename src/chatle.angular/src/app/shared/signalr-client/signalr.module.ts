import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@aspnet/signalr';

import { HubSettings } from './hub-settings';
import {
  SignalrService,
  HUB_CONNECTION_FACTORY,
  HubConnectionFactory
} from './signalr.service';

@NgModule({
  imports: [CommonModule],
  providers: [
    {
      provide: HUB_CONNECTION_FACTORY,
      useFactory: createHubConnection,
      deps: [HubSettings]
    },
    SignalrService
  ]
})
export class SignalrModule {}

export function createHubConnection(
  hubSettings: HubSettings
): HubConnectionFactory {
  return () =>
    new HubConnectionBuilder()
      .withUrl(hubSettings.url)
      .configureLogging(hubSettings.logLevel)
      .build();
}
