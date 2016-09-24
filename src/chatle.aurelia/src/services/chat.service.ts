import { EventAggregator } from 'aurelia-event-aggregator';
import { HttpClient } from 'aurelia-http-client';
import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import environment from '../environment';

import { Settings } from '../config/settings';
import { User } from '../model/user';
import { Message } from '../model/message';
import { Conversation } from '../model/conversation';
import { Attendee } from '../model/attendee';
import { ChangePassword } from '../model/changePassword';

import { ConnectionStateChanged } from '../events/connectionStateChanged';
import { ConversationJoined } from '../events/conversationJoined';
import { ConversationSelected } from '../events/conversationSelected';
import { MessageReceived } from '../events/messageReceived';
import { UserConnected } from '../events/userConnected';
import { UserDisconnected } from '../events/userDisconnected';
import { Error } from '../model/error';

interface ChatSignalR extends SignalR {
    chat: ChatProxy,
    hub: any
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
    currentConversation: Conversation;
    isGuess: boolean;

    constructor(private settings: Settings, private ea: EventAggregator, private http: HttpClient) {
        settings.apiBaseUrl = environment.apiBaseUrl;
        http.configure(
            builder => builder
                .withBaseUrl(settings.apiBaseUrl)
                .withCredentials(true));

        this.userName = sessionStorage.getItem('userName');
    }
    
    start(): Promise<ConnectionState> {
        
        let debug = environment.debug;
        // only for debug
        let hub = jQuery.connection.hub; 
        hub.logging = debug;
        hub.url = this.settings.apiBaseUrl + '/signalr';
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
            hub.stateChanged(change => {
                let oldState: string,
                    newState: string;
                
                let signalR = jQuery.signalR;
                for (var state in signalR.connectionState) {
                    if (signalR.connectionState[state] === change.oldState) {
                        oldState = state;
                    }
                    if (signalR.connectionState[state] === change.newState) {
                        newState = state;
                    }
                }

                console.log("Chat Hub state changed from " + oldState + " to " + newState);
            });                        
        }

        // callback on connection reconnect
        hub.reconnected(() => this.onReconnected());
        // callback on connection error
        hub.error(error => this.onError(error) );
        // callback on connection disconnect
        hub.disconnected(() => this.onDisconnected());
    
        // start the connection
        return new Promise<ConnectionState>((resolve, reject) => {
            hub.start()
                .done(response => { 
                    this.setConnectionState(ConnectionState.Connected);
                    resolve(ConnectionState.Connected);
                })
                .fail(error => {
                    this.setConnectionState(ConnectionState.Error)
                    reject(ConnectionState.Error);
                });
        });
    }

    showConversation(conversation: Conversation, router: Router) {
        this.currentConversation = conversation;
        this.setConverationTitle(conversation);
        this.ea.publish(new ConversationSelected(conversation));
        router.navigateToRoute('conversation', { id: conversation.title });
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
                .then(response => {
                    conversation.messages.unshift(m);
                    resolve(m);
                })
                .catch(error => reject('Error when sending the message'));
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
                        conversation.messages.unshift(m);
                        resolve(m);
                    })
                    .catch(error => reject('Error when creating the conversation'));
            });
            
        }
    }
    
    login(userName: string, password: string): Promise<any> {
        this.isGuess = !password;

        return new Promise<any>((resolve, reject) => {
            if (this.isGuess) {
                this.http.post(this.settings.accountdAPI + '/spaguess', { userName: userName })
                    .then(response => {
                        this.userName = userName;
                        this.start();
                        // get a new token for the session lifecycle
                        this.setXhrf(resolve, reject);
                    })
                    .catch(error => {
                        if (error.statusCode === 409) {
                            reject("This user name already exists, please chose a different name");
                        } else {
                            reject(this.getErrorMessage(error));
                        }   
                })
            } else {
                this.http.post(this.settings.accountdAPI + '/spalogin', { userName: userName, password: password })
                    .then(response => {
                        this.userName = userName;
                        sessionStorage.setItem('userName', userName);
                        this.start();
                        // get a new token for the session lifecycle
                        this.setXhrf(resolve, reject);
                    })
                    .catch(error => {
                        if (error.statusCode === 409) {
                            reject("This user name already exists, please chose a different name");
                        } else {
                            reject(this.getErrorMessage(error));
                        }
                    })
            }
        });
    }

    logoff() {
        delete this.userName;
        sessionStorage.removeItem('userName');
        jQuery.connection.hub.stop();
        this.http.post(this.settings.accountdAPI + '/spalogoff', null);
    }
    
    getUsers(): Promise<User[]> {
        return new Promise<User[]>((resolve, reject) => {
            this.http.get(this.settings.userAPI)
                .then(response => {
                        var data = response.content;
                        if (data && data.users) {
                            resolve(<User[]>data.users);
                        }
                    })
                .catch(error => reject('The service is down'));
        });
    }

    getConversations(): Promise<Conversation[]> {
        return new Promise<Conversation[]>((resolve, reject) => {
            this.http.get(this.settings.chatAPI)
                .then(response => {
                    if (response.response) {
                        var data = response.content;
                        if (data) {
                            resolve(<Conversation[]>data);
                        }
                    } else {
                        resolve(null);
                    }
                })
                .catch(error => reject('The service is down'));
        });
    }

    changePassword(model: ChangePassword): Promise<any> {
        if (this.isGuess) {
            return new Promise<any>((resolve, reject) => {
                this.http.post(this.settings.accountdAPI + '/setpassword', model)
                    .then(response => {
                        this.isGuess = false;
                        sessionStorage.setItem('userName', this.userName)
                        resolve();
                    })
                    .catch(error => reject(this.getErrorMessage(error)));
            });
        } else {
            return new Promise<any>((resolve, reject) => {
                this.http.put(this.settings.accountdAPI + '/changepassword', model)
                    .then(response => resolve())
                    .catch(error => reject(this.getErrorMessage(error)));
            });
        }
    }

    setXhrf(resolve: Function, reject: Function) {
        this.http.get('xhrf')
            .then(r => {
                this.http.configure(builder => {
                    builder.withHeader('X-XSRF-TOKEN', r.response);
                });
                resolve();
            })
            .catch(error => reject('the service is down'));
    }

    private getErrorMessage(error: any) {
        return (<Error[]>error.content)[0].errors[0].errorMessage
    }

    private setConverationTitle(conversation: Conversation) {
        if (conversation.title) {
            return;
        }

        let title = '';
        conversation.attendees.forEach(attendee => {
            if (attendee && attendee.userId && attendee.userId !== this.userName) {
                title += attendee.userId + ' ';
            }
        });
        conversation.title = title.trim();
    }

    private setConnectionState(connectionState: ConnectionState) {
        if (this.currentState === connectionState) {
            return;
        }
        
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
        this.setConverationTitle(conversation);
        this.ea.publish(new ConversationJoined(conversation));
    }
}