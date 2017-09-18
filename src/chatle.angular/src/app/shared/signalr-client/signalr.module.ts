import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpConnection, HubConnection } from '@aspnet/signalr-client';

import { HubSettings } from './hub-settings';
import { SignalrService, HUB_CONNECTION_FACTORY, HubConnectionFactory } from './signalr.service';

@NgModule({
  imports: [
    CommonModule
  ],
  providers: [
    { provide: HUB_CONNECTION_FACTORY, useFactory: createHubConnection, deps: [HubSettings] },
    SignalrService
  ]
})
export class SignalrModule { }

export function createHubConnection(hubSettings: HubSettings): HubConnectionFactory {
  return () => new HubConnection(
    new HttpConnection(
      hubSettings.url,
      hubSettings.transportType ?
        { transport: hubSettings.transportType} :
        undefined));
}
