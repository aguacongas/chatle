import { Injectable, Inject, InjectionToken } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/observable/fromPromise';
import 'rxjs/add/operator/map';

/**
 * Hub connection factory type
 */
export type HubConnectionFactory = () => HubConnection;
/**
 * {@link HubConnectionFactory} injection token
 */
export const HUB_CONNECTION_FACTORY = new InjectionToken<HubConnectionFactory>('HUB_CONNECTION_FACTORY');

export interface ServerMethodCallback {
  methodName: string;
  method: (...args: any[]) => void;
}

/**
 * SignalR core service
 */
@Injectable()
export class SignalrService {

  /**
   * Hub connection closed event
   */
  get closed(): Observable<Error> {
    return this.connectionCloseSubject;
  }
  /**
   * True if the hub connection is connected
   */
  isConnected = false;

  private connection: HubConnection;
  private connectionCloseSubject = new Subject<Error>();
  private serverMethods: ServerMethodCallback[];

  /**
   * Initialise a new instance of {@link SignalrService}
   * @param factory the hub connection factorx
   */
  constructor(@Inject(HUB_CONNECTION_FACTORY) private factory: HubConnectionFactory) {
  }

  /**
   * Connects to the hub
   */
  connect(): Observable<void> {
    this.connection = this.factory();
    this.serverMethods.forEach(callback => {
      this.connection.on(callback.methodName, callback.method);
    });
    this.connection.onclose(e => {
      this.isConnected = false;
      this.connectionCloseSubject.next(e);
    });

    return Observable.fromPromise(this.connection.start())
      .map(() => {
        this.isConnected = true;
      });
  }

  /**
   * Disconnect from the hub
   */
  disconnect(): void {
    this.connection.stop();
  }

  /**
   * Adds a server method callback
   * @param methodName the server method name
   * @param method the callback
   */
  on(methodName: string, method: (...args: any[]) => void): void {
    this.serverMethods = this.serverMethods || [];
    this.serverMethods.push({ methodName: methodName, method: method });
  }

  /**
   * Invokes a server methods
   * @param methodName the method name
   * @param args the method arguments
   */
  invoke(methodName: string, ...args: any[]): Observable<any> {
    if (!this.isConnected) {
      return;
    }
    const argsArray = Array.prototype.slice.call(arguments);

    const subject = new Subject<any>();
    const promise = this.connection.invoke.apply(this.connection, argsArray)
      .then(result => {
        subject.next(result);
      })
      .catch(err => {
        subject.error(err);
      });
    return subject;
  }
}
