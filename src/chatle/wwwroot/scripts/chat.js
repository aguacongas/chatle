$(function() {
    "use strict";
    $.connection.hub.logging = !0;
    var chatHub = $.connection.chat, VM = function() {
        var showMessages = function(user) {
            var conv;
            $.each(this.conversations(), function(index, c) {
                var attendees = c.attendees();
                if (2 === attendees.length) for (var i = 0; i < attendees.length; i++) {
                    var a = attendees[i];
                    if (a.UserId === user.Id) return conv = c, !1;
                }
            }), conv ? conv.getMessages() : (conv = new ConversationVM({
                Id: null,
                Attendees: [ {
                    ConversattionId: null,
                    UserId: user.Id
                }, {
                    ConversattionId: null,
                    UserId: UserName
                } ],
                Messages: null
            }), this.conversations.unshift(conv)), this.currentConv(conv);
        }, messageReceived = function(data) {
            if (data) {
                var conv;
                $.each(this.conversations(), function(index, c) {
                    return c.id === data.ConversationId ? (conv = c, !1) : void 0;
                }), conv && conv.messages.unshift(data);
            }
        }, joinConversation = function(data) {
            if (data) {
                this.conversations.remove(function(conv) {
                    return conv.id === data.Id;
                }), this.conversations.unshift(new ConversationVM(data));
            }
        }, showConversation = function(conv) {
            this.currentConv(conv);
        };
        return {
            users: ko.observableArray(),
            conversations: ko.observableArray(),
            currentConv: ko.observable(),
            showMessage: showMessages,
            messageReceived: messageReceived,
            joinConversation: joinConversation,
            showConversation: showConversation
        };
    }, viewModel = new VM(), ConversationVM = function(conv) {
        for (var sendMessage = function(textBoxId) {
            var textBox = $(textBoxId), message = textBox.val();
            textBox.val("");
            var self = this;
            self.id ? $.ajax("api/chat", {
                data: {
                    to: self.id,
                    text: message
                },
                type: "POST"
            }).done(function() {
                self.messages.unshift({
                    From: UserName,
                    Text: message
                });
            }) : $.ajax("api/chat/conv", {
                data: {
                    to: self.attendees()[0].UserId,
                    text: message
                },
                type: "POST"
            }).done(function(data) {
                self.id = data, self.messages.unshift({
                    From: UserName,
                    Text: message
                });
            });
        }, getMessages = function() {
            var self = this;
            $.getJSON("api/chat/" + this.id).done(function(data) {
                self.messages(data);
            });
        }, title = "", attendees = conv.Attendees, i = 0; i < attendees.length; i++) {
            var attendee = attendees[i];
            attendee.UserId !== UserName && (title += attendee.UserId + " ");
        }
        return {
            id: conv.Id,
            title: ko.observable(title),
            attendees: ko.observableArray(conv.Attendees),
            messages: ko.observableArray(conv.Messages),
            sendMessage: sendMessage,
            getMessages: getMessages
        };
    };
    ko.applyBindings(viewModel), chatHub.client.userConnected = function(user) {
        console.log("Chat Hub newUserConnected " + user.Id);
        var users = viewModel.users;
        users.remove(function(u) {
            return u.Id === user.Id;
        }), users.unshift(user);
    }, chatHub.client.userDisconnected = function(id) {
        console.log("Chat Hub userDisconnected " + id), viewModel.users.remove(function(u) {
            return u.Id === id;
        });
    }, chatHub.client.messageReceived = function(data) {
        viewModel.messageReceived(data);
    }, chatHub.client.joinConversation = function(data) {
        viewModel.joinConversation(data);
    }, $.connection.hub.stateChanged(function(change) {
        var oldState = null, newState = null;
        for (var state in $.signalR.connectionState) $.signalR.connectionState[state] === change.oldState && (oldState = state), 
        $.signalR.connectionState[state] === change.newState && (newState = state);
        console.log("Chat Hub state changed from " + oldState + " to " + newState);
    }), $.connection.hub.reconnected(function() {
        console.log("Chat Hub reconnect");
    }), $.connection.hub.error(function() {
        console.log("Chat Hub error");
    }), $.connection.hub.disconnected(function() {
        console.log("Chat Hub disconnected");
    }), $.connection.hub.start().done(function() {
        console.log("Chat Hub started"), $.getJSON("api/users").done(function(data) {
            viewModel.users(data.Users);
        }), $.getJSON("api/chat").done(function(data) {
            data && $.each(data, function(index, conv) {
                viewModel.conversations.unshift(new ConversationVM(conv));
            });
        });
    });
});
//# sourceMappingURL=chat.js.map