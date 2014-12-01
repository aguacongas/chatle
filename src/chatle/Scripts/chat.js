$(function () {
    "use strict";
    $.connection.hub.logging = true;

    var chatHub = $.connection.chat;

    var VM = function () {
        var vm = {
            users: ko.observableArray(),
            conversations: ko.observableArray(),
            unreadMessages: ko.observableArray(),
            currentConv: ko.observable(),
            showMessage: function (user) {
                var conv;
                $.each(vm.conversations(), function (index, c) {
                    var attendees = c.attendees();
                    if (attendees.length === 1) {
                        for (var a in attendees) {
                            if (a.Id === user.Id) {
                                conv = c;
                                return false;
                            }
                        }
                    }
                });
                if (conv) {
                    vm.currentConv(conv);
                    $.getJSON('api/chat/' + conv.Id)
                        .done(function (data) {
                            conv.messages(data);
                        });
                } else {
                    conv = new ConversationVM(
                        {
                            Id: null,
                            Attendees: [{ ConversattionId: null, UserId: user.Id }, { ConversattionId: null, UserId: null }],
                            Messages: null
                        })
                    vm.conversations.unshift(conv);
                }
                vm.currentConv(conv);                
                vm.unreadMessages.remove(function (item) {
                    return item.userId == user.Id;
                });
            },
            messageReceived: function (data) {
                if (!data) {
                    return;
                }
                var conv;
                $.each(vm.conversations, function (index, c) {
                    if (c === data.ConversationId) {
                        conv = c;
                        return false;
                    }
                });
                if (!conv) {
                    return;
                }
                conv.messages.unshift(data);
                if (vm.currentUser().Id !== data.UserId) {
                    vm.unreadMessages.unshift(data);
                }
            },
            joinConversation: function (data) {
                if (!data) {
                    return;
                }
                var exist = false;
                vm.conversations.remove(function (conv) {
                    return conv.Id === data.Id;
                });
                vm.conversations.unshift(new ConversationVM(data));
            }
        };

        return vm;
    };

    var viewModel = new VM();
    var ConversationVM = function (conv) {
        var vm = {
            id: conv.Id,
            title: ko.observable(),
            attendees: conv.Attendees,
            messages: ko.observableArray(conv.Messages),
            sendMessage: function (message) {
                if (!vm.id) {
                    $.ajax('api/chat/conv', {
                        data: { to: attendees()[0].UserId, text: message },
                        type: "POST"
                    }).done(function (data) {
                        vm.id = data;
                        viewModel.messages.unshift(model);
                    });
                } else {
                    $.ajax('api/chat', {
                        data: { to: vm.id, text: message },
                        type: "POST"
                    }).done(function () {
                        viewModel.messages.unshift({ from: "Me", text: message });
                    });
                }
            }
        };

        for (var attendee in conv.Attendees) {
            if (attendee.Id !== UserName) {
                vm.title += attendee.Id + ' ';
            }
        }
    
        return vm;
    };

    chatHub.client.userConnected = function (userName) {
        console.log("Chat Hub newUserConnected " + userName);
        var users = viewModel.users;
        if (users.indexOf(userName) == -1) {
            viewModel.users.push(userName);
        }
    };

    chatHub.client.userDisconnected = function (userName) {
        console.log("Chat Hub userDisconnected " + userName);
        viewModel.users.remove(userName);
    };

    chatHub.client.messageReceived = function (data) {
        viewModel.messageReceived(data);
    }

    chatHub.client.joinConversation = function (data) {
        viewModel.joinConversation(data);
    }

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
