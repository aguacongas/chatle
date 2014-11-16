$(function () {
    "use strict";
    $.connection.hub.logging = true;
   
    var chatHub = $.connection.chat;

    var viewModel = {        
        users: ko.observableArray(),
        messages: ko.observableArray(),
        unreadMessages: ko.observableArray(),
        currentUser: ko.observable(),
        sendMessage: function (message) {
            var model = { to: viewModel.currentUser(), text: message };
            chatHub.server.postMessage({ to: viewModel.currentUser(), text: message })
                .done(function (data) {
                    mode.from = "Me";
                    messages.unshift(model);
                });
        },
        showMessage: function (user) {
            viewModel.currentUser(user);
            $.getJSON('/chat/get/' + user)
                .done(function (data) {
                    viewModel.messages(data);
                });
            viewModel.unreadMessages.remove(function (item) {
                return item.from == user;
            });
        },
        messageReceived: function (data) {
            if (viewModel.currentUser() == data.from) {
                viewModel.messages.unshift(data);
            } else {
                viewModel.unreadMessages.unshift(data);
            }
        }
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
            $.getJSON('/user/get')
                .done(function (data) {
                    viewModel.users(data);
                });
        });

    ko.applyBindings(viewModel);
});