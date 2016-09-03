import { Injectable, Inject } from '@angular/core';
import { Http } from '@angular/http';

import 'rxjs/add/operator/toPromise';
import {Observable} from "rxjs/Observable";
import {Subject} from "rxjs/Subject";  

import { Settings } from './settings';
import { User } from './user';
import { Message } from './message';
import { Conversation } from './conversation';

interface ChatSignalR extends SignalR {
    chat: ChatProxy
}

interface ChatProxy {
    client: ChatClient
}

interface ChatClient {
    userConnected: (user: User) => void;
    userDisconnected: (id: string) => void;
    messageReceived: (message: Message) => void;
    joinConversation: (conversation: Conversation) => void;
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
    userDiscconnected: Observable<string>;
    messageReceived: Observable<Message>;
    joinConversation: Observable<Conversation>;    
    openConversation: Observable<Conversation>;

    private connectionStateSubject = new Subject<ConnectionState>();
    private userConnectedSubject = new Subject<User>();
    private userDisconnectedSubject = new Subject<string>();
    private messageReceivedSubject = new Subject<Message>();
    private joinConversationSubject = new Subject<Conversation>();
    private openConversationSubject = new Subject<Conversation>();

    constructor(private http: Http, private settings: Settings) {
        this.connectionState = this.connectionStateSubject.asObservable();
        this.messageReceived = this.messageReceivedSubject.asObservable();
        this.userConnected = this.userConnectedSubject.asObservable();
        this.userDiscconnected = this.userDisconnectedSubject.asObservable();
        this.joinConversation = this.joinConversationSubject.asObservable();
        this.openConversation = this.openConversationSubject.asObservable();
    }
    
    start(debug: boolean):Observable<ConnectionState> {
        // only for debug
        $.connection.hub.logging = debug;
        // get the signalR hub named 'chat'
        let connection = <ChatSignalR>$.connection;
        let chatHub = connection.chat;
        
        /**
          * @desc callback when a new user connect to the chat
          * @param User user, the connected user
        */
        chatHub.client.userConnected = user => this.onUserConnected(user);
        /**
          * @desc callback when a new user disconnect the chat
          * @param id, the disconnected user id
        */
        chatHub.client.userDisconnected = id => this.onUserDisconnected(id);
        /**
          * @desc callback when a message is received
          * @param String to, the conversation id
          * @param Message data, the message
        */
        chatHub.client.messageReceived = message => this.onMessageReceived(message);
        /**
          * @desc callback when a new conversation is create on server
          * @param Conv data, the conversation model
        */
        chatHub.client.joinConversation = conversation => this.onJoinConversation(conversation);

        if (debug) {
            // for debug only, callback on connection state change
            $.connection.hub.stateChanged(change => {
                let oldState: string,
                    newState: string;

                for (var state in $.signalR.connectionState) {
                    if ($.signalR.connectionState[state] === change.oldState) {
                        oldState = state;
                    }
                    if ($.signalR.connectionState[state] === change.newState) {
                        newState = state;
                    }
                }

                console.log("Chat Hub state changed from " + oldState + " to " + newState);
            });                        
        }

        // callback on connection reconnect
        $.connection.hub.reconnected(() => this.onReconnected());
        // callback on connection error
        $.connection.hub.error(error => this.onError(error) );
        // callback on connection disconnect
        $.connection.hub.disconnected(() => this.onDisconnected());
    
        // start the connection
        $.connection.hub.start()
            .done(response => this.setConnectionState(ConnectionState.Connected))
            .fail(error => this.connectionStateSubject.error(error));

        return this.connectionState;
    }

    showConversation(conversation: Conversation) {
        this.openConversationSubject.next(conversation);        
    }

    sendMessage(conversation: Conversation, message: string): Observable<Message> {
        let messageSubject = new Subject<Message>();

        let m = new Message();
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
            let attendee = conversation.attendees.find(a => a.userId !== this.settings.userName); 
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
        let subject = new Subject<User[]>();

        this.http.get(this.settings.userAPI)
            .subscribe(
                response => {
                    var data = response.json();
                    if (data && data.users) {
                        subject.next(data.users as User[]);
                    }
                },
                error => subject.error(error));
        
        return subject.asObservable();
    }

    getConversations(): Observable<Conversation[]> {
        let subject = new Subject<Conversation[]>();

        this.http.get(this.settings.chatAPI)
            .subscribe(
                response => {
                    var data = response.json();
                    if (data) {
                        subject.next(data as Conversation[]);
                    }
                },
                error => subject.error(error));
        
        return subject.asObservable();
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
        console.log("Chat Hub new user connected: " + user.id);
        this.userConnectedSubject.next(user);
    }

    private onUserDisconnected(id: string) {
        console.log("Chat Hub user disconnected: " + id);
        if (id !== this.settings.userName) {
            this.userDisconnectedSubject.next(id);
        }
    }   

    private onMessageReceived(message: Message) {
        this.messageReceivedSubject.next(message);
    }

    private onJoinConversation(conversation: Conversation) {
        this.joinConversationSubject.next(conversation);
    }
}