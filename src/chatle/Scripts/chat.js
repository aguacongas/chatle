$(function () {
    "use strict";
    $.connection.hub.logging = true;

    var chatHub = $.connection.chat;

    var VM = function () {
        var showMessages = function (user) {
            var conv;
            $.each(this.conversations(), function (index, c) {
                var attendees = c.attendees();
                if (attendees.length === 2) {
                    for (var i = 0; i < attendees.length; i++) {
                        var a = attendees[i];
                        if (a.UserId === user.Id) {
                            conv = c;
                            return false;
                        }
                    }
                }
            });
            if (conv) {
                this.currentConv(conv);
                conv.getMessages();
            } else {
                conv = new ConversationVM(
                    {
                        Id: null,
                        Attendees: [{ ConversattionId: null, UserId: user.Id }, { ConversattionId: null, UserId: UserName }],
                        Messages: null
                    })
                this.conversations.unshift(conv);
            }
            this.currentConv(conv);
        };

        var messageReceived = function (to, data) {
            if (!data || !to) {
                return;
            }
            var conv;
            $.each(this.conversations(), function (index, c) {
                if (c.id === to) {
                    conv = c;
                    return false;
                }
            });
            if (!conv) {
                return;
            }
            conv.messages.unshift(data);
        };

        var joinConversation = function (data) {
            if (!data) {
                return;
            }
            var exist = false;
            this.conversations.remove(function (conv) {
                return conv.Id === data.Id;
            });
            this.conversations.unshift(new ConversationVM(data));
        };

        var showConversation = function (conv) {
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
    };

    var viewModel = new VM();
    var ConversationVM = function (conv) {
        var sendMessage = function (textBoxId) {
            var textBox = $(textBoxId);
            var message = textBox.val();
            textBox.val('');
            var self = this;
            if (!self.id) {
                $.ajax('api/chat/conv', {
                    data: { to: self.attendees()[0].UserId, text: message },
                    type: "POST"
                }).done(function (data) {
                    self.id = data;
                    self.messages.unshift({ From: "Me", Text: message });
                });
            } else {
                $.ajax('api/chat', {
                    data: { to: self.id, text: message },
                    type: "POST"
                }).done(function () {
                    self.messages.unshift({ From: "Me", Text: message });
                });
            }
        };

        var getMessages = function () {
            var self = this;
            $.getJSON('api/chat/' + this.id)
                    .done(function (data) {
                        self.messages(data);
                    });
        };

        var title = '';
        var attendees = conv.Attendees;

        for (var i = 0; i < attendees.length; i++) {
            var attendee = attendees[i];
            if (attendee.UserId !== UserName) {
                title += attendee.UserId + ' ';
            }
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

    chatHub.client.userConnected = function (user) {
        console.log("Chat Hub newUserConnected " + user.Id);
        var users = viewModel.users;
        users.remove(function (u) {
            return u.Id === user.Id;
        });
        users.unshift(user);
    };

    chatHub.client.userDisconnected = function (user) {
        console.log("Chat Hub userDisconnected " + user.Id);
        viewModel.users.remove(function (u) {
            return u.Id === user.Id;
        });
    };

    chatHub.client.messageReceived = function (to, data) {
        viewModel.messageReceived(to, data);
    };

    chatHub.client.joinConversation = function (data) {
        viewModel.joinConversation(data);
    };

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

    $.connection.hub.reconnected(function () {
        console.log("Chat Hub reconnect");
    });

    $.connection.hub.error(function (err) {
        console.log("Chat Hub error");
    });

    $.connection.hub.disconnected(function () {
        console.log("Chat Hub disconnected");
    });

    $.connection.hub.start()
        .done(function () {
            console.log("Chat Hub started");
            $.getJSON("api/users")
                .done(function (data) {
                    viewModel.users(data);
                });
        });

    ko.applyBindings(viewModel);
});
