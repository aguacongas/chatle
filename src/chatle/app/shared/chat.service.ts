import { Injectable } from '@angular/core';
import { Headers, Http, Response } from '@angular/http';

import 'rxjs/add/operator/toPromise';

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
    userConnected(user: User): void;
    userDisconnected(id: string): void;
    messageReceived(data: any): void;
    joinConversation(data: any): void;
}

@Injectable()
export class ChatService {

    userName: string;
    users: User[];
    conversations: Conversation[];

    constructor(private settings: Settings, private http: Http) { }
    
    init(debug: boolean) {
        // only for debug
        $.connection.hub.logging = debug;
        // get the signalR hub named 'chat'
        let connection = <ChatSignalR>$.connection;
        let chatHub = connection.chat;
        
        /**
          * @desc callback when a new user connect to the chat
          * @param User user, the connected user
        */
        chatHub.client.userConnected = this.userConnected;
        /**
          * @desc callback when a new user disconnect the chat
          * @param id, the disconnected user id
        */
        chatHub.client.userDisconnected = this.removeUser;
        /**
          * @desc callback when a message is received
          * @param String to, the conversation id
          * @param Message data, the message
        */
        chatHub.client.messageReceived = this.messageReceived;
        /**
          * @desc callback when a new conversation is create on server
          * @param Conv data, the conversation model
        */
        chatHub.client.joinConversation = this.joinConversation

        if (debug) {
            // for debug only, callback on connection state change
            $.connection.hub.stateChanged(function (change) {
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
            // for debug only, callback on connection reconnect
            $.connection.hub.reconnected(function () {
                console.log("Chat Hub reconnect");
            });
        }
        // callback on connection error
        $.connection.hub.error(function (err) {
            console.log("Chat Hub error");
            this.logout(err);
        });
        // callback on connection disconnect
        $.connection.hub.disconnected(function () {
            console.log("Chat Hub disconnected");
            this.logout('disconnected');
        });
        // start the connection
        $.connection.hub.start()
            .done(this.start);
    };
    
    private start() {
        console.log("Chat Hub started");
        
        // get connected users
        this.http.get(this.settings.userAPI)
            .toPromise()
            .then(response => {
                let data = response.json();
                if (data && data.users) {
                    this.users = data.users as User[];
                }
            })
            .catch(this.logout);

        // get user conversations
        $.getJSON(this.settings.chatAPI)
            .done(function (data) {
                if (!data) {
                    return;
                }
                $.each(data, function (index, data) {
                    this.conversations.unshift(new Conversation(data));
                })
            }).fail(function (data) {
                this.logout(data);
            });
    }

    private messageReceived(data: any) {
        if (!data) {
            return;
        }

        let conv: Conversation;
        $.each(this.conversations, function (index, c) {
            if (c.id === data.conversationId) {
                conv = c;
                return false;
            }
        });

        if (!conv) {
            return;
        }

        conv.messages.unshift(data);
    }

    private removeUser(id: string) {
        for (let i = this.users.length - 1; i >= 0; i--) {
            if (this.users[i].id === id) {
                this.users.slice(i, 1);
            }
        }
    }

    private joinConversation(data: any) {

    }

    private userConnected(user: any) {
        console.log("Chat Hub newUserConnected " + user.id);
        this.removeUser(user.id);
        this.users.unshift(user);                      
    }   

    private logout(message: string) {
        // TODO implement
    };
}