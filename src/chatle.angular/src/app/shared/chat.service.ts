import { Injectable, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { HubConnection, HttpConnection } from '@aspnet/signalr-client';
import { SignalrService } from './signalr-client';
import 'rxjs/add/operator/toPromise';
import {Observable} from 'rxjs/Observable';
import {Subject} from 'rxjs/Subject';

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

    constructor(private http: Http, private settings: Settings, private signalrService: SignalrService) {
        this.connectionState = this.connectionStateSubject.asObservable();
        this.messageReceived = this.messageReceivedSubject.asObservable();
        this.userConnected = this.userConnectedSubject.asObservable();
        this.userDiscconnected = this.userDisconnectedSubject.asObservable();
        this.joinConversation = this.joinConversationSubject.asObservable();
        this.openConversation = this.openConversationSubject.asObservable();
        signalrService.on('userConnected', user => this.onUserConnected(user));
        signalrService.on('userDisconnected', user => this.onUserDisconnected(user));
        signalrService.on('messageReceived', message => this.onMessageReceived(message));
        signalrService.on('joinConversation', conv => this.onJoinConversation(conv));
    }

    start(debug: boolean): Observable<ConnectionState> {
        this.signalrService.connect().subscribe(() => {
            this.currentState = ConnectionState.Connected;
            this.connectionStateSubject.next(this.currentState);
        }, err => {
            this.currentState = ConnectionState.Error;
            this.connectionStateSubject.next(this.currentState);
        });
        return this.connectionState;
    }

    showConversation(conversation: Conversation) {
        this.openConversationSubject.next(conversation);
    }

    sendMessage(conversation: Conversation, message: string): Observable<Message> {
        const messageSubject = new Subject<Message>();

        const m = new Message();
        m.conversationId = conversation.id;
        m.from = this.settings.userName;
        m.text = message;

        if (conversation.id) {
            this.http.post(this.settings.chatAPI, {
                    to: conversation.id,
                    text: message
                })
                .subscribe(
                    response => messageSubject.next(m),
                    error => messageSubject.error(error));
        } else {
            const attendee = conversation.attendees.find(a => a.userId !== this.settings.userName);
            this.http.post(this.settings.convAPI, {
                    to: attendee.userId,
                    text: message
                })
                .subscribe(
                    response => {
                        conversation.id = response.text();
                        this.joinConversationSubject.next(conversation);
                        messageSubject.next(m);
                    },
                    error => messageSubject.error(error));
        }

        return  messageSubject.asObservable();
    }

    getUsers(): Observable<User[]> {
        const subject = new Subject<User[]>();

        this.http.get(this.settings.userAPI)
            .subscribe(
                response => {
                    const data = response.json();
                    if (data && data.users) {
                        subject.next(data.users as User[]);
                    }
                },
                error => subject.error(error));

        return subject.asObservable();
    }

    getConversations(): Observable<Conversation[]> {
        const subject = new Subject<Conversation[]>();

        this.http.get(this.settings.chatAPI)
            .subscribe(
                response => {
                    const data = response.json();
                    if (data) {
                        const conversations = data as Conversation[];
                        conversations.forEach(value => this.setConversationTitle(value));
                        subject.next(conversations);
                    }
                },
                error => subject.error(error));

        return subject.asObservable();
    }

    private setConversationTitle(conversation: Conversation) {
      let title = '';
      conversation.attendees.forEach(attendee => {
          if (attendee && attendee.userId && attendee.userId !== this.settings.userName) {
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
