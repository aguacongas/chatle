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

	// mock connection
	$.connection.chat = {
		client: {}
	};
	$.connection.hub = {
		stateChanged: function (onStateChange) { },
		reconnected: function (onReconnect) { },
		error: function (onError) { },
		disconnected: function (onDisconnected) { },
		start: function () {
			return {
				done: function () {
				}
			}
		}
	};

	describe("initialize chat", function () {
		beforeEach(function () {
			spyOn(ko, 'applyBindings').and.callFake(function (vm) { });

			spyOn(chatLe, 'init').and.callThrough();
			spyOn($.connection.hub, 'stateChanged').and.callThrough();
			spyOn($.connection.hub, 'reconnected').and.callThrough();
			spyOn($.connection.hub, 'error').and.callThrough();
			spyOn($.connection.hub, 'disconnected').and.callThrough();
			spyOn($.connection.hub, 'start').and.callThrough();
			chatLe.init(true);
		});

		it("chatLe.init must called", function () {
			expect(chatLe.init).toHaveBeenCalledWith(true);
		});

		it("$.connection.chat.client.userConnected must be defined", function () {
			expect($.connection.chat.client.userConnected).toBeDefined();;
		});

		it("$.connection.chat.client.messageReceived must be defined", function () {
			expect($.connection.chat.client.messageReceived).toBeDefined();
		});

		it("$.connection.chat.client.joinConversation must be defined", function () {
			expect($.connection.chat.client.joinConversation).toBeDefined();
		});

		it("$.connection.hub.stateChanged must have been called in debug mode", function () {
			expect($.connection.hub.stateChanged).toHaveBeenCalled();
		});

		it("$.connection.hub.reconnected must have been called  in debug mode", function () {
			expect($.connection.hub.reconnected).toHaveBeenCalled();
		});

		it("$.connection.hub.error must have been called", function () {
			expect($.connection.hub.error).toHaveBeenCalled();
		});

		it("$.connection.hub.disconnected must have been called", function () {
			expect($.connection.hub.disconnected).toHaveBeenCalled();
		});

		it("$.connection.hub.start must have been called", function () {
			expect($.connection.hub.start).toHaveBeenCalled();
		});
	});

	describe("start chat", function () {
		var startCallback;
		var currenVM;

		beforeEach(function () {
			spyOn(ko, 'applyBindings').and.callFake(function (vm) {
				currenVM = vm;
			});

			spyOn($.connection.hub, 'start').and.returnValue({
				done: function (callback) {
					startCallback = callback;
				}
			});

			spyOn($.connection.hub.start(), 'done').and.callThrough();
			chatLe.init(true);
		});

		it("$.connection.hub.start().done must have been called", function () {
			expect($.connection.hub.start, 'done').toHaveBeenCalled();
		});

		it("start call back must be defined", function () {
			expect(startCallback).toBeDefined();
		});

		describe("on started", function () {
			var doneCallback = [];
			var failCallback = [];

			chatLe.userAPI = "userAPI";
			chatLe.chatAPI = "chatAPI";

			beforeEach(function () {
				spyOn($, 'getJSON').and.callFake(function (url) {
					return {
						done: function (callback) {
							if (callback) {
								doneCallback.push({ u: url, c: callback });
							}
							return {
								fail: function (callback) {
									if (callback) {
										failCallback.push({ u: url, c: callback });
									}
								}
							}
						}
					}
				});

				spyOn($.getJSON(), 'done').and.callThrough();
				spyOn($.getJSON().done(), 'fail').and.callThrough();

				startCallback();
			});

			var getCallback = function (array, key) {
				var result = $.grep(array, function (item) { return item.u == key });
				if (result.length == 0) {
					return null;
				}
				return result[0].c;
			}

			it("on started must get users", function () {
				var callback = getCallback(doneCallback, chatLe.userAPI);
				expect(callback).toBeDefined();
			});

			it("on started must define a fail function on get users", function () {
				var callback = getCallback(failCallback, chatLe.userAPI);
				expect(callback).toBeDefined();
			});

			it("on started must get conversations", function () {
				var callback = getCallback(doneCallback, chatLe.chatAPI);
				expect(callback).toBeDefined();
			});

			it("on started must define a fail function on get conversations", function () {
				var callback = getCallback(failCallback, chatLe.chatAPI);
				expect(callback).toBeDefined();
			});

			it("on started must set the users observable array", function () {
				var data = {
					Users: [
						"test"
					]
				};
				var callback = getCallback(doneCallback, chatLe.userAPI);
				spyOn(currenVM, 'users').and.callThrough();
				callback(data);
				expect(currenVM.users).toHaveBeenCalledWith(data.Users);
			});

			it("on started must set the conversations observable array", function () {
				var data = [{ 
					Id: 'test',
					Attendees: ['test1', 'test'],
					Messages: ['test']
				}];
				var callback = getCallback(doneCallback, chatLe.chatAPI);
				spyOn(currenVM.conversations, 'unshift').and.callThrough();
				callback(data);
				expect(currenVM.conversations.unshift).toHaveBeenCalled();
			});

			describe("logout on error", function () {

				beforeEach(function () {
					spyOn(window, '$').and.returnValue({
						val: function () { },
						submit: function () { }
					});
				});

				it("on started must call logout on get users failed", function () {
					var callback = getCallback(failCallback, chatLe.userAPI);
					callback('error');
					expect(window.$).toHaveBeenCalledWith('#reasonHidden');
					expect(window.$).toHaveBeenCalledWith('#logoutForm');
				});

				it("on started must call logout on get users failed", function () {
					var callback = getCallback(failCallback, chatLe.chatAPI);
					callback('error');
					expect(window.$).toHaveBeenCalledWith('#reasonHidden');
					expect(window.$).toHaveBeenCalledWith('#logoutForm');
				});

			});
		});
	});
});