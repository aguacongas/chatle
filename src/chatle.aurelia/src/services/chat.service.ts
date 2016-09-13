import { EventAggregator } from 'aurelia-event-aggregator';
import { HttpClient } from 'aurelia-http-client';
import { autoinject } from 'aurelia-framework';
import environment from '../environment';

import { Settings } from '../config/settings';
import { User } from '../model/user';
import { Message } from '../model/message';
import { Conversation } from '../model/conversation';
import { Attendee } from '../model/attendee';

import { ConnectionStateChanged } from '../events/connectionStateChanged';
import { ConversationJoined } from '../events/conversationJoined';
import { ConversationSelected } from '../events/conversationSelected';
import { MessageReceived } from '../events/messageReceived';
import { UserConnected } from '../events/userConnected';
import { UserDisconnected } from '../events/userDisconnected';

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

@autoinject
export class ChatService {
    currentState = ConnectionState.Disconnected;
    userName: string;

    constructor(private settings: Settings, private ea: EventAggregator, private http: HttpClient) {
        http.configure(
            builder => builder
                .withBaseUrl(settings.apiBaseUrl)
                .withCredentials(true));

        settings.userName = sessionStorage.getItem('userName');
    }
    
    start() {
        let debug = environment.debug;
        // only for debug
        jQuery.connection.hub.logging = debug;
        // get the signalR hub named 'chat'
        let connection = <ChatSignalR>jQuery.connection;
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
            jQuery.connection.hub.stateChanged(change => {
                let oldState: string,
                    newState: string;

                for (var state in jQuery.signalR.connectionState) {
                    if (jQuery.signalR.connectionState[state] === change.oldState) {
                        oldState = state;
                    }
                    if (jQuery.signalR.connectionState[state] === change.newState) {
                        newState = state;
                    }
                }

                console.log("Chat Hub state changed from " + oldState + " to " + newState);
            });                        
        }

        // callback on connection reconnect
        jQuery.connection.hub.reconnected(() => this.onReconnected());
        // callback on connection error
        jQuery.connection.hub.error(error => this.onError(error) );
        // callback on connection disconnect
        jQuery.connection.hub.disconnected(() => this.onDisconnected());
    
        // start the connection
        jQuery.connection.hub.start()
            .done(response => this.setConnectionState(ConnectionState.Connected))
            .fail(error => this.setConnectionState(ConnectionState.Error));
    }

    showConversation(conversation: Conversation) {
        this.ea.publish(new ConversationJoined(conversation));        
    }

    sendMessage(conversation: Conversation, message: string): Promise<Message> {
        let m = new Message();
        m.conversationId = conversation.id;
        m.from = this.userName;
        m.text = message;

        if (conversation.id) {
            return new Promise<Message>((resolve, reject) => {
                this.http.post(this.settings.chatAPI, {
                    to: conversation.id,
                    text: message
                })
                .then(response => resolve(m))
                .catch(error => reject(error));
            });
        } else {
            let attendee: Attendee;
             conversation.attendees.forEach(a => {
                 if (a.userId !== this.userName) {
                     attendee = a;
                 }
             });

             return new Promise<Message>((resolve, reject) => {
                this.http.post(this.settings.convAPI, {
                    to: attendee.userId,
                    text: message
                })
                .then(
                    response => {
                        conversation.id = response.content;
                        this.ea.publish(new ConversationJoined(conversation));
                        resolve(m);
                    })
                .catch(error => reject(error));
            });
            
        }
    }
    
    login(userName: string): Promise<any> {
        return new Promise<any>((resolve, reject) => {
            this.http.get('xhrf')
                .then(response => {
                    this.http.createRequest(this.settings.loginAPI)
                        .asPost()
                        .withHeader("X-XSRF-TOKEN", response.response)
                        .withContent({ userName: userName })
                        .send()
                        .then(response => {
                            this.userName = userName;
                            sessionStorage.setItem('userName', userName);
                            resolve();
                            this.start();
                        })
                        .catch(error => reject(error))
                })
                .catch(error => reject(error));
        });
    }

    logoff() {
        delete this.userName;
        sessionStorage.removeItem('userName');
        jQuery.connection.hub.stop();
        this.http.createRequest(this.settings.logoffAPI)
            .asPost()
            .send();
    }
    
    getUsers(): Promise<User[]> {
        return new Promise<User[]>((resolve, reject) => {
            this.http.get(this.settings.userAPI)
                .then(response => {
                        var data = response.content;
                        if (data && data.users) {
                            resolve(data.users as User[]);
                        }
                    })
                .catch(error => reject(error));
        });
    }

    getConversations(): Promise<Conversation[]> {
        return new Promise<Conversation[]>((resolve, reject) => {
            this.http.get(this.settings.chatAPI)
                .then(response => {
                    if (response.response) {
                        var data = response.content;
                        if (data) {
                            resolve(data as Conversation[]);
                        }
                    } else {
                        resolve(null);
                    }
                })
                .catch(error => reject(error));
        });
    }

    private setConnectionState(connectionState: ConnectionState) {
        console.log('connection state changed to: ' + connectionState);
        this.currentState = connectionState;
        this.ea.publish(new ConnectionStateChanged(connectionState));
    }
       
    private onReconnected() {
        this.setConnectionState(ConnectionState.Connected);
    }

    private onDisconnected() {
        this.setConnectionState(ConnectionState.Disconnected);
    }

    private onError(error: any) {
        this.setConnectionState(ConnectionState.Error);
    }

    private onUserConnected(user: User) {
        console.log("Chat Hub new user connected: " + user.id);
        this.ea.publish(new UserConnected(user));
    }

    private onUserDisconnected(id: string) {
        console.log("Chat Hub user disconnected: " + id);
        if (id !== this.userName) {
            this.ea.publish(new UserDisconnected(id));
        }
    }   

    private onMessageReceived(message: Message) {
        this.ea.publish(new MessageReceived(message));
    }

    private onJoinConversation(conversation: Conversation) {
        this.ea.publish(new ConversationJoined(conversation));
    }
}