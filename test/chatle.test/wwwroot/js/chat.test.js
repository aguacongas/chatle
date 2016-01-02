describe("chat test suite", function () {
	
	it("chatLe must be defined", function () {
		expect(chatLe).toBeDefined();
	});

	it("chatLe.userName must be defined", function () {
		expect(chatLe.userName).toBeDefined();
	});

	it("chatLe.chatAPI must be defined", function () {
		expect(chatLe.chatAPI).toBeDefined();
	});

	it("chatLe.chatAPI must be defined", function () {
		expect(chatLe.convAPI).toBeDefined();
	});

	it("chatLe.userAPI must be defined", function () {
		expect(chatLe.userAPI).toBeDefined();
	});

	it("chatLe.init must be defined", function () {
		expect(chatLe.init).toBeDefined();
	});

	describe("start chat", function () {
		var onStateChanged;
		var onReconnected;
		var onError;
		var onDisconnected;
		var startCallback;
		var currenVM;

		// mock connection
		$.connection.chat = {
			client: {}
		};
		$.connection.hub = {
			stateChanged: function (callback) {
				onStateChanged = callback
			},
			reconnected: function (callback) {
				onReconnected = callback
			},
			error: function (callback) {
				onError = callback
			},
			disconnected: function (callback) {
				onDisconnected = callback
			},
			start: function () { }
		};

		$.signalR = {
			connectionState: {
				olstate: 0,
				newstate: 1
			}
		};

		beforeEach(function () {
			spyOn(ko, 'applyBindings').and.callFake(function (vm) {
				currenVM = vm;
			});

			spyOn(chatLe, 'init').and.callThrough();

			spyOn($.connection.hub, 'start').and.returnValue({
				done: function (callback) {
					startCallback = callback;
				}
			});

			spyOn($.connection.hub.start(), 'done').and.callThrough();

			chatLe.init(true);

		});

		it("chatLe.init must called", function () {
			expect(chatLe.init).toHaveBeenCalledWith(true);
		});

		it("$.connection.chat.client.userConnected must be defined", function () {
			expect($.connection.chat.client.userConnected).toBeDefined();
		});

		it("$.connection.chat.client.messageReceived must be defined", function () {
			expect($.connection.chat.client.messageReceived).toBeDefined();
		});

		it("$.connection.chat.client.joinConversation must be defined", function () {
			expect($.connection.chat.client.joinConversation).toBeDefined();
		});

		it("$.connection.hub.stateChanged must have been called in debug mode", function () {
			expect(onStateChanged).toBeDefined();
			onStateChanged({ olstate: 0, newstate: 1 });
		});

		it("$.connection.hub.reconnected must have been called  in debug mode", function () {
			expect(onReconnected).toBeDefined();
			onReconnected();
		});

		it("$.connection.hub.error must have been called", function () {
			expect(onError).toBeDefined();
		});

		it("$.connection.hub.disconnected must have been called", function () {
			expect(onDisconnected).toBeDefined();
		});

		it("start call back must be defined", function () {
			expect(startCallback).toBeDefined();
		});

		describe("on started", function () {

			var users = {
				Users: [
					"test"
				]
			};

			var conversations = [{
				Id: 'test',
				Attendees: ['test1', 'test'],
				Messages: ['test']
			}];

			beforeEach(function () {

				chatLe.userAPI = "/userAPI";
				chatLe.chatAPI = "/chatAPI";

				jasmine.Ajax.install();

				spyOn(currenVM, 'users').and.callThrough();
				spyOn(currenVM.conversations, 'unshift').and.callThrough();

				startCallback();

				var usersResponse = {
					status: 200,
					statusText: 'HTTP/1.1 200 OK',
					contentType: 'text/json;charset=UTF-8',
					responseText: JSON.stringify(users)
				};

				jasmine.Ajax.requests.at(0).respondWith(usersResponse);

				var conversationsResponse = {
					status: 200,
					statusText: 'HTTP/1.1 200 OK',
					contentType: 'text/json;charset=UTF-8',
					responseText: JSON.stringify(conversations)
				};

				jasmine.Ajax.requests.at(1).respondWith(conversationsResponse);
			});

			afterEach(function () {
				jasmine.Ajax.uninstall();
			});

			it("on started must set the users observable array", function () {
				expect(currenVM.users).toHaveBeenCalledWith(users.Users);
			});

			it("on started must set the conversations observable array", function () {
				expect(currenVM.conversations.unshift).toHaveBeenCalled();
			});

		});

		describe("logout on error", function () {

			beforeEach(function () {
				spyOn(window, '$').and.returnValue({
					val: function () { },
					submit: function () { }
				});

				jasmine.Ajax.install();
				startCallback();

				var usersResponse = {
					status: 500,
					statusText: 'HTTP/1.1 500 internal error',
					contentType: 'text/json;charset=UTF-8',
					responseText: 'error'
				};

				jasmine.Ajax.requests.at(0).respondWith(usersResponse);
			});

			afterEach(function () {
				jasmine.Ajax.uninstall();
			});

			it("on started must call logout on get users failed", function () {
				expect(window.$).toHaveBeenCalledWith('#reasonHidden');
				expect(window.$).toHaveBeenCalledWith('#logoutForm');
			});

		});

		describe("logout on error", function () {

			beforeEach(function () {
				spyOn(window, '$').and.returnValue({
					val: function () { },
					submit: function () { }
				});

				jasmine.Ajax.install();
				startCallback();

				var usersResponse = {
					status: 500,
					statusText: 'HTTP/1.1 500 internal error',
					contentType: 'text/json;charset=UTF-8',
					responseText: 'error'
				};

				jasmine.Ajax.requests.at(1).respondWith(usersResponse);
			});

			it("on started must call logout on get conversation failed", function () {
				expect(window.$).toHaveBeenCalledWith('#reasonHidden');
				expect(window.$).toHaveBeenCalledWith('#logoutForm');
			});

			afterEach(function () {
				jasmine.Ajax.uninstall();
			});

		});

		describe("on signalR event", function () {

			beforeEach(function () {
				spyOn(window, '$').and.returnValue({
					val: function () { },
					submit: function () { }
				});

			});

			it("on hub disconnected must call logout", function () {
				onDisconnected();
				expect(window.$).toHaveBeenCalledWith('#reasonHidden');
				expect(window.$).toHaveBeenCalledWith('#logoutForm');
			});

			it("on hub error must call logout", function () {
				onError('error');
				expect(window.$).toHaveBeenCalledWith('#reasonHidden');
				expect(window.$).toHaveBeenCalledWith('#logoutForm');
			});

		})
	});
});