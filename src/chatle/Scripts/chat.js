var chatLe = chatLe || {};
/**
  * @desc chat management class
  * @author Olivier Lefebvre
*/
(function () {
	"use strict";

	chatLe = function () {
		// settings
		var settings = {
			userName: null,
			chatAPI: null,
			convAPI: null,
			userAPI: null,
			init: null
		};

		/**
		  * @desc logout on error
		  * @param String reason, the error
		*/
		var logout = function (reason) {
			$('#reasonHidden').val(reason);
			$('#logoutForm').submit();
		};

		/**
		  * @desc main view model constructor
		*/
		var VM = function () {		
			/**
			  * @desc get or create a one to one conversation with the user and retrieve messages
			  * @param User user
			*/
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
					conv.getMessages();
				} else {
					conv = new ConversationVM(
						{
							Id: null,
							Attendees: [{ ConversattionId: null, UserId: user.Id }, { ConversattionId: null, UserId: settings.userName }],
							Messages: null
						})
					this.conversations.unshift(conv);
				}
				this.currentConv(conv);
			};
			/**
			  * @desc add the message received in the conversation having id = to
			  * @param Message data, the message
			*/
			var messageReceived = function (data) {
				if (!data) {
					return;
				}
				var conv;
				$.each(this.conversations(), function (index, c) {
					if (c.id === data.ConversationId) {
						conv = c;
						return false;
					}
				});
				if (!conv) {
					return;
				}
				conv.messages.unshift(data);
			};
			/**
			  * @desc add the current user in a new conversation
			  * @param Conv data, the conversation model
			*/
			var joinConversation = function (data) {
				if (!data) {
					return;
				}
				this.conversations.remove(function (conv) {
					return conv.id === data.Id;
				});
				this.conversations.unshift(new ConversationVM(data));
			};
			/**
			  * @desc set the conv as the current conversation
			  * @param ConversationVM conv, the conversation model
			*/
			var showConversation = function (conv) {
				this.currentConv(conv);
			};
			return {
				userName: null,
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
		/**
		  * @desc conversation view model constructor
		*/
		var ConversationVM = function (conv) {

			/**
			  * @desc send a new message in the conversation
			  * @param String textBoxId, the id of the textbox containing the message to send
			*/
			var sendMessage = function (textBoxId) {
				var textBox = $(textBoxId);
				var message = textBox.val();
				textBox.val('');
				var self = this;
				if (!self.id) {
					$.ajax(settings.convAPI, {
						data: { to: self.attendees()[0].UserId, text: message },
						type: "POST"
					}).done(function (data) {
						self.id = data;
						self.messages.unshift({ From: settings.userName, Text: message });
					}).fail(function (data) {
						logout(data);
					});;
				} else {
					$.ajax(settings.chatAPI, {
						data: { to: self.id, text: message },
						type: "POST"
					}).done(function () {
						self.messages.unshift({ From: settings.userName, Text: message });
					}).fail(function (data) {
						logout(data);
					});;
				}
			};
			/**
			  * @desc get messages for the conversation
			*/
			var getMessages = function () {
				var self = this;
				$.getJSON(settings.chatAPI + '/' + this.id)
					.done(function (data) {
						self.messages(data);
					}).fail(function (data) {
						logout(data);
					});
			};

			var title = '';
			var attendees = conv.Attendees;

			for (var i = 0; i < attendees.length; i++) {
				var attendee = attendees[i];
				if (attendee.UserId !== settings.userName) {
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

		var init = function (debug) {
			// only for debug
			$.connection.hub.logging = debug;
			// get the signalR hub named 'chat'
			var chatHub = $.connection.chat;

			viewModel.userName = settings.userName;
			// apply the view model binding
			ko.applyBindings(viewModel);

			/**
			  * @desc callback when a new user connect to the chat
			  * @param User user, the connected user
			*/
			chatHub.client.userConnected = function (user) {
				console.log("Chat Hub newUserConnected " + user.Id);
				var users = viewModel.users;
				users.remove(function (u) {
					return u.Id === user.Id;
				});
				users.unshift(user);
			};
			/**
			  * @desc callback when a new user disconnect the chat
			  * @param id, the disconnected user id
			*/
			chatHub.client.userDisconnected = function (id) {
				console.log("Chat Hub userDisconnected " + id);
				viewModel.users.remove(function (u) {
					return u.Id === id;
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
							viewModel.users(data.Users);
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

		settings.init = init;
		return settings;
	};

})();
chatLe = chatLe();
