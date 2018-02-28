import { TransportType, LogLevel } from '@aspnet/signalr';

export class HubSettings {
  url: string;
  transportType?: TransportType;
  logLevel? = LogLevel.Trace;
}
