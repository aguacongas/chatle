import { TransportType } from '@aspnet/signalr-client';

export class HubSettings {
  url: string;
  transportType?: TransportType;
}
