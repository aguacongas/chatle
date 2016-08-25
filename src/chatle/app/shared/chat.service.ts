import { Injectable } from '@angular/core';
import { User } from './user';

interface SignalR {
    chat: any
}

@Injectable()
export class ChatService {

    userName: string;
    users: User[];

    init(debug: boolean) {
        // only for debug
        $.connection.hub.logging = debug;
        // get the signalR hub named 'chat'
        var chatHub = $.connection.chat;
        
        /**
          * @desc callback when a new user connect to the chat
          * @param User user, the connected user
        */
        chatHub.client.userConnected = function (user) {
            console.log("Chat Hub newUserConnected " + user.id);
            users.remove(function (u) {
                return u.id === user.id;
            });
            users.unshift(user);
        };
        /**
          * @desc callback when a new user disconnect the chat
          * @param id, the disconnected user id
        */
        chatHub.client.userDisconnected = function (id) {
            console.log("Chat Hub userDisconnected " + id);
            users.remove(function (u) {
                return u.id === id;
            });
        };
        /**
          * @desc callback when a message is received
          * @param String to, the conversation id
          * @param Message data, the message
        */
        chatHub.client.messageReceived = function (data) {
            viewModel.messageReceived(data);
        };
        /**
          * @desc callback when a new conversation is create on server
          * @param Conv data, the conversation model
        */
        chatHub.client.joinConversation = function (data) {
            console.log("join conversation " + JSON.stringify(data));
            viewModel.joinConversation(data);
        };

        if (debug) {
            // for debug only, callback on connection state change
            $.connection.hub.stateChanged(function (change) {
                var oldState = null,
                    newState = null;

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
            logout(err);
        });
        // callback on connection disconnect
        $.connection.hub.disconnected(function () {
            console.log("Chat Hub disconnected");
            logout('disconnected');
        });
        // start the connection
        $.connection.hub.start()
            .done(function () {
                console.log("Chat Hub started");
                // get connected users
                $.getJSON(settings.userAPI)
                    .done(function (data) {
                        if (data && data.users) {
                            viewModel.users(data.users);
                        }
                    }).fail(function (data) {
                        logout(data);
                    });

                // get user conversations
                $.getJSON(settings.chatAPI)
                    .done(function (data) {
                        if (!data) {
                            return;
                        }
                        $.each(data, function (index, conv) {
                            viewModel.conversations.unshift(new ConversationVM(conv));
                        })
                    }).fail(function (data) {
                        logout(data);
                    });
            });
    };

    private logout(message: string) {

    };
}