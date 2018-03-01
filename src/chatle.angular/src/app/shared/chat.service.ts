import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HttpConnection } from '@aspnet/signalr';
import { SignalrService } from './signalr-client';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { Settings } from './settings';
import { User } from './user';
import { Message } from './message';
import { Conversation } from './conversation';

interface ChatClient {
  userConnected: (user: User) => void;
  userDisconnected: (user: UserDiscconnected) => void;
  messageReceived: (message: Message) => void;
  joinConversation: (conversation: Conversation) => void;
}

interface UserDiscconnected {
  id: string;
  isRemoved: boolean;
}

export enum ConnectionState {
  Connected = 1,
  Disconnected = 2,
  Error = 3
}

@Injectable()
export class ChatService {
  currentState = ConnectionState.Disconnected;
  connectionState: Observable<ConnectionState>;
  userConnected: Observable<User>;
  userDiscconnected: Observable<UserDiscconnected>;
  messageReceived: Observable<Message>;
  joinConversation: Observable<Conversation>;
  openConversation: Observable<Conversation>;

  private connectionStateSubject = new Subject<ConnectionState>();
  private userConnectedSubject = new Subject<User>();
  private userDisconnectedSubject = new Subject<UserDiscconnected>();
  private messageReceivedSubject = new Subject<Message>();
  private joinConversationSubject = new Subject<Conversation>();
  private openConversationSubject = new Subject<Conversation>();

  constructor(
    private http: HttpClient,
    private settings: Settings,
    private signalrService: SignalrService
  ) {
    this.connectionState = this.connectionStateSubject.asObservable();
    this.messageReceived = this.messageReceivedSubject.asObservable();
    this.userConnected = this.userConnectedSubject.asObservable();
    this.userDiscconnected = this.userDisconnectedSubject.asObservable();
    this.joinConversation = this.joinConversationSubject.asObservable();
    this.openConversation = this.openConversationSubject.asObservable();
    signalrService.on('userConnected', user => this.onUserConnected(user));
    signalrService.on('userDisconnected', user =>
      this.onUserDisconnected(user)
    );
    signalrService.on('messageReceived', message =>
      this.onMessageReceived(message)
    );
    signalrService.on('joinConversation', conv =>
      this.onJoinConversation(conv)
    );
    signalrService.closed.subscribe(error => {
      this.start(true);
    }, error => {
      this.currentState = ConnectionState.Error;
      this.connectionStateSubject.next(this.currentState);
    });
  }

  start(debug: boolean): Observable<ConnectionState> {
    this.signalrService.connect().subscribe(
      () => {
        this.currentState = ConnectionState.Connected;
        this.connectionStateSubject.next(this.currentState);
      },
      err => {
        this.currentState = ConnectionState.Error;
        this.connectionStateSubject.next(this.currentState);
      }
    );
    return this.connectionState;
  }

  showConversation(conversation: Conversation) {
    this.openConversationSubject.next(conversation);
  }

  sendMessage(
    conversation: Conversation,
    message: string
  ): Observable<Message> {
    const m = new Message();
    m.conversationId = conversation.id;
    m.from = this.settings.userName;
    m.text = message;

    if (conversation.id) {
      return this.http
        .post(this.settings.chatAPI, {
          to: conversation.id,
          text: message
        }).map(value => m);
    } else {
      const attendee = conversation.attendees.find(
        a => a.userId !== this.settings.userName
      );
      return this.http
        .post<string>(this.settings.convAPI, {
          to: attendee.userId,
          text: message
        })
        .map(
          response => {
            conversation.id = response;
            this.setConversationTitle(conversation);
            this.joinConversationSubject.next(conversation);
            return m;
          });
    }
  }

  getUsers(): Observable<User[]> {
    return this.http.get<any>(this.settings.userAPI).map(
      response => {
        const data = response;
        if (data && data.users) {
          return data.users as User[];
        }
      });
  }

  getConversations(): Observable<Conversation[]> {
    return this.http.get<Conversation[]>(this.settings.chatAPI).map(
      conversations => {
        conversations.forEach(value => this.setConversationTitle(value));
        return conversations;
      });
  }

  private setConversationTitle(conversation: Conversation) {
    let title = '';
    conversation.attendees.forEach(attendee => {
      if (
        attendee &&
        attendee.userId &&
        attendee.userId !== this.settings.userName
      ) {
        title += attendee.userId + ' ';
      }
    });
    conversation.title = title.trim();
  }

  private setConnectionState(connectionState: ConnectionState) {
    console.log('connection state changed to: ' + connectionState);
    this.currentState = connectionState;
    this.connectionStateSubject.next(connectionState);
  }

  private onReconnected() {
    this.setConnectionState(ConnectionState.Connected);
  }

  private onDisconnected() {
    this.setConnectionState(ConnectionState.Disconnected);
  }

  private onError(error: any) {
    this.connectionStateSubject.error(error);
  }

  private onUserConnected(user: User) {
    console.log('Chat Hub new user connected: ' + user.id);
    this.userConnectedSubject.next(user);
  }

  private onUserDisconnected(user: UserDiscconnected) {
    console.log('Chat Hub user disconnected: ' + user.id);
    if (user.id !== this.settings.userName) {
      this.userDisconnectedSubject.next(user);
    }
  }

  private onMessageReceived(message: Message) {
    this.messageReceivedSubject.next(message);
  }

  private onJoinConversation(conversation: Conversation) {
    this.setConversationTitle(conversation);
    this.joinConversationSubject.next(conversation);
  }
}
