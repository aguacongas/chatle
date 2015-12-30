/// <reference path="../../../../src/chatle/wwwroot/lib/knockout/dist/knockout.js" />
/// <reference path="../../../../src/chatle/wwwroot/js/chat.js" />

describe("chat test suite", function () {
	it("ko must be defined", function () {
		expect(ko).toBeDefined();
	});

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
});