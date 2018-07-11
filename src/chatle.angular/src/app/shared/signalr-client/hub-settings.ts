import { LogLevel } from '@aspnet/signalr';

export class HubSettings {
  url: string;
  logLevel? = LogLevel.Trace;
}
