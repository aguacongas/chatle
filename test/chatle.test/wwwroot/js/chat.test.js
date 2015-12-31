/// <reference path="../../../../src/chatle/wwwroot//lib/jquery/dist/jquery.js" />
/// <reference path="../../../../src/chatle/wwwroot//lib/signalr/jquery.signalR.js" />
/// <reference path="../../../../src/chatle/wwwroot/lib/knockout/dist/knockout.js" />
/// <reference path="../../../../src/chatle/wwwroot/js/chat.js" />

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

	describe("initialize chat", function () {

		beforeEach(function () {
			// mock connection
			$.connection.chat = {
				client: {}
			};
			$.connection.hub = {
				stateChanged: function (onStateChange) { },
				reconnected: function (onReconnect) { },
				error: function (onError) { },
				disconnected: function (onDisconnected) { },
				start: function () { return { done: function () { } }}
			};

			spyOn(chatLe, 'init').and.callThrough();
			spyOn($.connection.hub, 'stateChanged').and.callThrough();
			spyOn($.connection.hub, 'reconnected').and.callThrough();
			spyOn($.connection.hub, 'error').and.callThrough();
			spyOn($.connection.hub, 'disconnected').and.callThrough();
			// mock ko
			ko.applyBindings = function (vm) { };
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
	});
});