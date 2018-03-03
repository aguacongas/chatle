webpackJsonp(["main"],{

/***/ "../../../../../src/$$_lazy_route_resource lazy recursive":
/***/ (function(module, exports) {

function webpackEmptyAsyncContext(req) {
	// Here Promise.resolve().then() is used instead of new Promise() to prevent
	// uncatched exception popping up in devtools
	return Promise.resolve().then(function() {
		throw new Error("Cannot find module '" + req + "'.");
	});
}
webpackEmptyAsyncContext.keys = function() { return []; };
webpackEmptyAsyncContext.resolve = webpackEmptyAsyncContext;
module.exports = webpackEmptyAsyncContext;
webpackEmptyAsyncContext.id = "../../../../../src/$$_lazy_route_resource lazy recursive";

/***/ }),

/***/ "../../../../../src/app/app.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"row vertical-container\">\r\n    <chatle-contacts>Loading ...</chatle-contacts>\r\n    <chatle-conversation>Loading ...</chatle-conversation>\r\n    <chatle-conversations>Loading ...</chatle-conversations>\r\n</div>\r\n"

/***/ }),

/***/ "../../../../../src/app/app.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var AppComponent = (function () {
    function AppComponent() {
    }
    AppComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["m" /* Component */])({
            // tslint:disable-next-line:component-selector
            selector: 'chatle',
            template: __webpack_require__("../../../../../src/app/app.component.html")
        })
    ], AppComponent);
    return AppComponent;
}());



/***/ }),

/***/ "../../../../../src/app/app.module.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppModule; });
/* unused harmony export getSettings */
/* unused harmony export boot */
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_platform_browser__ = __webpack_require__("../../../platform-browser/esm5/platform-browser.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_forms__ = __webpack_require__("../../../forms/esm5/forms.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_common_http__ = __webpack_require__("../../../common/esm5/http.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__shared_signalr_client__ = __webpack_require__("../../../../../src/app/shared/signalr-client/index.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__environments_environment__ = __webpack_require__("../../../../../src/environments/environment.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__app_component__ = __webpack_require__("../../../../../src/app/app.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__contacts_contact_component__ = __webpack_require__("../../../../../src/app/contacts/contact.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__contacts_contacts_component__ = __webpack_require__("../../../../../src/app/contacts/contacts.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__conversations_conversation_component__ = __webpack_require__("../../../../../src/app/conversations/conversation.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__conversations_conversationPreview_component__ = __webpack_require__("../../../../../src/app/conversations/conversationPreview.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11__conversations_conversations_component__ = __webpack_require__("../../../../../src/app/conversations/conversations.component.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12__shared_settings__ = __webpack_require__("../../../../../src/app/shared/settings.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_13__shared_chat_service__ = __webpack_require__("../../../../../src/app/shared/chat.service.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};














var hubSettings = {
    url: __WEBPACK_IMPORTED_MODULE_5__environments_environment__["a" /* environment */].production ? '/chat' : 'http://localhost:5000/chat'
};
var AppModule = (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["E" /* NgModule */])({
            imports: [__WEBPACK_IMPORTED_MODULE_1__angular_platform_browser__["a" /* BrowserModule */], __WEBPACK_IMPORTED_MODULE_2__angular_forms__["a" /* FormsModule */], __WEBPACK_IMPORTED_MODULE_3__angular_common_http__["b" /* HttpClientModule */], __WEBPACK_IMPORTED_MODULE_4__shared_signalr_client__["b" /* SignalrModule */]],
            declarations: [
                __WEBPACK_IMPORTED_MODULE_6__app_component__["a" /* AppComponent */],
                __WEBPACK_IMPORTED_MODULE_7__contacts_contact_component__["a" /* ContactComponent */],
                __WEBPACK_IMPORTED_MODULE_8__contacts_contacts_component__["a" /* ContactsComponent */],
                __WEBPACK_IMPORTED_MODULE_10__conversations_conversationPreview_component__["a" /* ConversationPreviewComponent */],
                __WEBPACK_IMPORTED_MODULE_9__conversations_conversation_component__["a" /* ConversationComponent */],
                __WEBPACK_IMPORTED_MODULE_11__conversations_conversations_component__["a" /* ConversationsComponent */]
            ],
            bootstrap: [__WEBPACK_IMPORTED_MODULE_6__app_component__["a" /* AppComponent */]],
            providers: [
                __WEBPACK_IMPORTED_MODULE_13__shared_chat_service__["a" /* ChatService */],
                { provide: __WEBPACK_IMPORTED_MODULE_12__shared_settings__["a" /* Settings */], useFactory: getSettings },
                { provide: __WEBPACK_IMPORTED_MODULE_4__shared_signalr_client__["a" /* HubSettings */], useValue: hubSettings },
                {
                    provide: __WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* APP_INITIALIZER */],
                    useFactory: boot,
                    deps: [__WEBPACK_IMPORTED_MODULE_13__shared_chat_service__["a" /* ChatService */]],
                    multi: true
                },
            ]
        })
    ], AppModule);
    return AppModule;
}());

function getSettings() {
    return window['chatleSetting'];
}
function boot(service) {
    return function () {
        return new Promise(function (resolve, reject) {
            service.start(true).subscribe(function () { return resolve(); }, function (error) {
                location.assign('/Account?reason=disconnected');
                reject();
            });
        });
    };
}


/***/ }),

/***/ "../../../../../src/app/contacts/contact.component.html":
/***/ (function(module, exports) {

module.exports = "<a href=\"#\" *ngIf=\"!isCurrentUser\" (click)=\"onClick()\">{{user.id}}</a>\r\n<a href=\"/Account/Manage\" *ngIf=\"isCurrentUser\">{{user.id}}</a>"

/***/ }),

/***/ "../../../../../src/app/contacts/contact.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ContactComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__shared_user__ = __webpack_require__("../../../../../src/app/shared/user.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__shared_conversation__ = __webpack_require__("../../../../../src/app/shared/conversation.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__shared_attendee__ = __webpack_require__("../../../../../src/app/shared/attendee.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__shared_chat_service__ = __webpack_require__("../../../../../src/app/shared/chat.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__shared_settings__ = __webpack_require__("../../../../../src/app/shared/settings.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};






var ContactComponent = (function () {
    function ContactComponent(service, settings) {
        this.service = service;
        this.settings = settings;
    }
    ContactComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.service.joinConversation
            .subscribe(function (conversion) {
            if (!_this.user.conversation
                && conversion.attendees.length < 3
                && conversion.attendees.some(function (a) { return a.userId === _this.user.id; })) {
                _this.user.conversation = conversion;
            }
        });
        this.isCurrentUser = this.settings.userName === this.user.id;
    };
    ContactComponent.prototype.onClick = function () {
        if (!this.user.conversation) {
            var conversation = new __WEBPACK_IMPORTED_MODULE_2__shared_conversation__["a" /* Conversation */]();
            var attendees = new Array();
            var attendee = new __WEBPACK_IMPORTED_MODULE_3__shared_attendee__["a" /* Attendee */]();
            attendee.userId = this.user.id;
            attendees.push(attendee);
            conversation.attendees = attendees;
            conversation.messages = new Array();
            this.user.conversation = conversation;
        }
        this.service.showConversation(this.user.conversation);
    };
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["z" /* Input */])(),
        __metadata("design:type", __WEBPACK_IMPORTED_MODULE_1__shared_user__["a" /* User */])
    ], ContactComponent.prototype, "user", void 0);
    ContactComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["m" /* Component */])({
            selector: 'chatle-contact',
            template: __webpack_require__("../../../../../src/app/contacts/contact.component.html")
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_4__shared_chat_service__["a" /* ChatService */], __WEBPACK_IMPORTED_MODULE_5__shared_settings__["a" /* Settings */]])
    ], ContactComponent);
    return ContactComponent;
}());



/***/ }),

/***/ "../../../../../src/app/contacts/contacts.component.css":
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__("../../../../css-loader/lib/css-base.js")(false);
// imports


// module
exports.push([module.i, ".contacts-list {\r\n  height: calc(100% - 23.2px);\r\n}\r\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ "../../../../../src/app/contacts/contacts.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"col-xs-3 vertical-container\">\r\n  <h6>CONNECTED</h6>\r\n  <div class=\"vertical-list vertical-container contacts-list\">\r\n    <span *ngIf=\"!users\">loading...</span>\r\n    <ul>\r\n      <li *ngFor=\"let user of users\">\r\n        <chatle-contact [user]=\"user\"></chatle-contact>\r\n      </li>\r\n    </ul>\r\n  </div>\r\n</div>\r\n"

/***/ }),

/***/ "../../../../../src/app/contacts/contacts.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ContactsComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__shared_chat_service__ = __webpack_require__("../../../../../src/app/shared/chat.service.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var ContactsComponent = (function () {
    function ContactsComponent(service, detector) {
        this.service = service;
        this.detector = detector;
        this.users = [];
    }
    ContactsComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.service.connectionState
            .subscribe(function (connectionState) {
            if (connectionState === __WEBPACK_IMPORTED_MODULE_1__shared_chat_service__["b" /* ConnectionState */].Connected) {
                _this.getUsers();
            }
        }, function (error) { return _this.error = error; });
        this.service.userConnected
            .subscribe(function (user) {
            _this.removeUser(user.id);
            _this.users.unshift(user);
            _this.detector.detectChanges();
        }, function (error) { return _this.error = error; });
        this.service.userDiscconnected
            .subscribe(function (user) {
            _this.removeUser(user.id);
            _this.detector.detectChanges();
        }, function (error) { return _this.error = error; });
        if (this.service.currentState === __WEBPACK_IMPORTED_MODULE_1__shared_chat_service__["b" /* ConnectionState */].Connected) {
            this.getUsers();
        }
    };
    ContactsComponent.prototype.removeUser = function (id) {
        var index = this.users.findIndex(function (user) { return user.id === id; });
        if (index !== -1) {
            this.users.splice(index, 1);
        }
    };
    ContactsComponent.prototype.getUsers = function () {
        var _this = this;
        this.service.getUsers()
            .subscribe(function (users) {
            users.forEach(function (user) {
                if (!_this.users.some(function (u) { return u.id === user.id; })) {
                    _this.users.push(user);
                }
            });
            _this.detector.detectChanges();
        }, function (error) { return _this.error = error; });
    };
    ContactsComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["m" /* Component */])({
            selector: 'chatle-contacts',
            template: __webpack_require__("../../../../../src/app/contacts/contacts.component.html"),
            styles: [__webpack_require__("../../../../../src/app/contacts/contacts.component.css")]
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_1__shared_chat_service__["a" /* ChatService */], __WEBPACK_IMPORTED_MODULE_0__angular_core__["j" /* ChangeDetectorRef */]])
    ], ContactsComponent);
    return ContactsComponent;
}());



/***/ }),

/***/ "../../../../../src/app/conversations/conversation.component.css":
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__("../../../../css-loader/lib/css-base.js")(false);
// imports


// module
exports.push([module.i, "ul {\r\n  margin-top: 10px;\r\n}\r\n\r\n.from-me {\r\n  background-color: #fafafa\r\n}\r\n\r\n.messages-list {\r\n  height: calc(100% - 71px);\r\n}\r\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ "../../../../../src/app/conversations/conversation.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"col-xs-6 vertical-container\">\r\n  <div *ngIf=\"conversation\" class=\"vertical-container\">\r\n    <h6>{{conversation.title}}</h6>\r\n    <div class=\"vertical-container\">\r\n      <form class=\"form-inline\">\r\n        <div class=\"input-group\">\r\n          <input class=\"form-control\" name=\"message\" [(ngModel)]=\"message\" placeholder=\"message...\">\r\n          <span class=\"input-group-btn\">\r\n            <button type=\"submit\" class=\"btn btn-default\" (click)=\"send()\">send</button>\r\n          </span>\r\n        </div>\r\n      </form>\r\n      <ul class=\"vertical-list vertical-container messages-list\">\r\n        <li [class.from-me]=\"message.from == userName\" *ngFor=\"let message of conversation.messages; let i=index\">\r\n          <div *ngIf=\"displayFrom(i)\">\r\n            <small>{{message.from}}</small>\r\n          </div>\r\n          <span>{{message.text}}</span>\r\n        </li>\r\n      </ul>\r\n    </div>\r\n  </div>\r\n  <div class=\"vertical-list vertical-container\" *ngIf=\"!conversation\">\r\n    <h1>WELCOME TO THIS REALLY SIMPLE CHAT</h1>\r\n  </div>\r\n</div>\r\n"

/***/ }),

/***/ "../../../../../src/app/conversations/conversation.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ConversationComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__shared_conversation__ = __webpack_require__("../../../../../src/app/shared/conversation.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__shared_chat_service__ = __webpack_require__("../../../../../src/app/shared/chat.service.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var ConversationComponent = (function () {
    function ConversationComponent(service) {
        this.service = service;
    }
    ConversationComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.userName = this.service.settings.userName;
        this.service.openConversation
            .subscribe(function (conversation) { return _this.conversation = conversation; });
        this.service.userDiscconnected
            .subscribe(function (user) {
            if (user.isRemoved && _this.conversation
                && _this.conversation.attendees.some(function (a) { return a.userId === user.id; })
                && _this.conversation.attendees.length < 3) {
                delete _this.conversation;
            }
        });
    };
    ConversationComponent.prototype.send = function () {
        var _this = this;
        this.service.sendMessage(this.conversation, this.message)
            .subscribe(function (m) { return _this.conversation.messages.unshift(m); }, function (error) { return _this.error = error; });
        this.message = null;
    };
    ConversationComponent.prototype.displayFrom = function (i) {
        var messages = this.conversation.messages;
        return i === 0 || messages[i].from !== messages[i - 1].from;
    };
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["z" /* Input */])(),
        __metadata("design:type", __WEBPACK_IMPORTED_MODULE_1__shared_conversation__["a" /* Conversation */])
    ], ConversationComponent.prototype, "conversation", void 0);
    ConversationComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["m" /* Component */])({
            selector: 'chatle-conversation',
            template: __webpack_require__("../../../../../src/app/conversations/conversation.component.html"),
            styles: [__webpack_require__("../../../../../src/app/conversations/conversation.component.css")]
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_2__shared_chat_service__["a" /* ChatService */]])
    ], ConversationComponent);
    return ConversationComponent;
}());



/***/ }),

/***/ "../../../../../src/app/conversations/conversationPreview.component.html":
/***/ (function(module, exports) {

module.exports = "<a href=\"#\" (click)=\"onClick()\">{{conversation.title}}</a><br />\r\n<span>{{getLastMessage()}}</span>"

/***/ }),

/***/ "../../../../../src/app/conversations/conversationPreview.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ConversationPreviewComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__shared_conversation__ = __webpack_require__("../../../../../src/app/shared/conversation.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__shared_chat_service__ = __webpack_require__("../../../../../src/app/shared/chat.service.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var ConversationPreviewComponent = (function () {
    function ConversationPreviewComponent(service) {
        this.service = service;
    }
    ConversationPreviewComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.service.messageReceived
            .subscribe(function (message) {
            if (message.conversationId === _this.conversation.id) {
                _this.conversation.messages.unshift(message);
            }
        });
    };
    ConversationPreviewComponent.prototype.onClick = function () {
        this.service.showConversation(this.conversation);
    };
    ConversationPreviewComponent.prototype.getLastMessage = function () {
        var messages = this.conversation.messages;
        if (messages && messages[0]) {
            return messages[0].text;
        }
    };
    __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["z" /* Input */])(),
        __metadata("design:type", __WEBPACK_IMPORTED_MODULE_1__shared_conversation__["a" /* Conversation */])
    ], ConversationPreviewComponent.prototype, "conversation", void 0);
    ConversationPreviewComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["m" /* Component */])({
            selector: 'chatle-conversation-preview',
            template: __webpack_require__("../../../../../src/app/conversations/conversationPreview.component.html")
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_2__shared_chat_service__["a" /* ChatService */]])
    ], ConversationPreviewComponent);
    return ConversationPreviewComponent;
}());



/***/ }),

/***/ "../../../../../src/app/conversations/conversations.component.css":
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__("../../../../css-loader/lib/css-base.js")(false);
// imports


// module
exports.push([module.i, ".conversations-list {\r\n  height: calc(100% - 23.2px);\r\n}\r\n", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ "../../../../../src/app/conversations/conversations.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"col-xs-3 vertical-container\">\r\n  <h6>CONV</h6>\r\n  <div class=\"vertical-list vertical-container conversations-list\">\r\n    <ul *ngFor=\"let conv of conversations\">\r\n      <li>\r\n        <chatle-conversation-preview [conversation]=\"conv\"></chatle-conversation-preview>\r\n      </li>\r\n    </ul>\r\n  </div>\r\n</div>\r\n"

/***/ }),

/***/ "../../../../../src/app/conversations/conversations.component.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ConversationsComponent; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__shared_chat_service__ = __webpack_require__("../../../../../src/app/shared/chat.service.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__shared_settings__ = __webpack_require__("../../../../../src/app/shared/settings.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var ConversationsComponent = (function () {
    function ConversationsComponent(service, settings) {
        this.service = service;
        this.settings = settings;
        this.conversations = [];
    }
    ConversationsComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.service.connectionState
            .subscribe(function (connectionState) {
            if (connectionState === __WEBPACK_IMPORTED_MODULE_1__shared_chat_service__["b" /* ConnectionState */].Connected) {
                _this.getConversations();
            }
        }, function (error) { return _this.error = error; });
        this.service.joinConversation
            .subscribe(function (conversation) {
            _this.conversations.unshift(conversation);
        }, function (error) { return _this.error = error; });
        this.service.userDiscconnected
            .subscribe(function (user) {
            if (user.isRemoved) {
                var index = _this.conversations.findIndex(function (c) { return c.attendees.length < 3
                    && c.attendees.some(function (a) { return a.userId === user.id; }); });
                if (index > -1) {
                    _this.conversations.splice(index, 1);
                }
            }
        });
        if (this.service.currentState === __WEBPACK_IMPORTED_MODULE_1__shared_chat_service__["b" /* ConnectionState */].Connected) {
            this.getConversations();
        }
    };
    ConversationsComponent.prototype.getConversations = function () {
        var _this = this;
        this.service.getConversations()
            .subscribe(function (conversations) { return _this.conversations = conversations; }, function (error) { return _this.error = error; });
    };
    ConversationsComponent = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["m" /* Component */])({
            selector: 'chatle-conversations',
            template: __webpack_require__("../../../../../src/app/conversations/conversations.component.html"),
            styles: [__webpack_require__("../../../../../src/app/conversations/conversations.component.css")]
        }),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_1__shared_chat_service__["a" /* ChatService */], __WEBPACK_IMPORTED_MODULE_2__shared_settings__["a" /* Settings */]])
    ], ConversationsComponent);
    return ConversationsComponent;
}());



/***/ }),

/***/ "../../../../../src/app/shared/attendee.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return Attendee; });
var Attendee = (function () {
    function Attendee() {
    }
    return Attendee;
}());



/***/ }),

/***/ "../../../../../src/app/shared/chat.service.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "b", function() { return ConnectionState; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ChatService; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_common_http__ = __webpack_require__("../../../common/esm5/http.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__signalr_client__ = __webpack_require__("../../../../../src/app/shared/signalr-client/index.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__ = __webpack_require__("../../../../rxjs/_esm5/Subject.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_rxjs_add_operator_map__ = __webpack_require__("../../../../rxjs/_esm5/add/operator/map.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_rxjs_add_operator_catch__ = __webpack_require__("../../../../rxjs/_esm5/add/operator/catch.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__settings__ = __webpack_require__("../../../../../src/app/shared/settings.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__message__ = __webpack_require__("../../../../../src/app/shared/message.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};








var ConnectionState;
(function (ConnectionState) {
    ConnectionState[ConnectionState["Connected"] = 1] = "Connected";
    ConnectionState[ConnectionState["Disconnected"] = 2] = "Disconnected";
    ConnectionState[ConnectionState["Error"] = 3] = "Error";
})(ConnectionState || (ConnectionState = {}));
var ChatService = (function () {
    function ChatService(settings, http, signalrService) {
        var _this = this;
        this.settings = settings;
        this.http = http;
        this.signalrService = signalrService;
        this.currentState = ConnectionState.Disconnected;
        this.connectionStateSubject = new __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__["a" /* Subject */]();
        this.userConnectedSubject = new __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__["a" /* Subject */]();
        this.userDisconnectedSubject = new __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__["a" /* Subject */]();
        this.messageReceivedSubject = new __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__["a" /* Subject */]();
        this.joinConversationSubject = new __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__["a" /* Subject */]();
        this.openConversationSubject = new __WEBPACK_IMPORTED_MODULE_3_rxjs_Subject__["a" /* Subject */]();
        this.connectionState = this.connectionStateSubject.asObservable();
        this.messageReceived = this.messageReceivedSubject.asObservable();
        this.userConnected = this.userConnectedSubject.asObservable();
        this.userDiscconnected = this.userDisconnectedSubject.asObservable();
        this.joinConversation = this.joinConversationSubject.asObservable();
        this.openConversation = this.openConversationSubject.asObservable();
        signalrService.on('userConnected', function (user) { return _this.onUserConnected(user); });
        signalrService.on('userDisconnected', function (user) {
            return _this.onUserDisconnected(user);
        });
        signalrService.on('messageReceived', function (message) {
            return _this.onMessageReceived(message);
        });
        signalrService.on('joinConversation', function (conv) {
            return _this.onJoinConversation(conv);
        });
        signalrService.closed.subscribe(function (error) {
            if (location) {
                location.href = 'Account';
            }
        }, function (error) {
            _this.currentState = ConnectionState.Error;
            _this.connectionStateSubject.next(_this.currentState);
        });
    }
    ChatService.prototype.start = function (debug) {
        var _this = this;
        this.signalrService.connect().subscribe(function () {
            _this.currentState = ConnectionState.Connected;
            _this.connectionStateSubject.next(_this.currentState);
        }, function (err) {
            _this.currentState = ConnectionState.Error;
            _this.connectionStateSubject.next(_this.currentState);
        });
        return this.connectionState;
    };
    ChatService.prototype.showConversation = function (conversation) {
        this.openConversationSubject.next(conversation);
    };
    ChatService.prototype.sendMessage = function (conversation, message) {
        var _this = this;
        var m = new __WEBPACK_IMPORTED_MODULE_7__message__["a" /* Message */]();
        m.conversationId = conversation.id;
        m.from = this.settings.userName;
        m.text = message;
        if (conversation.id) {
            return this.http
                .post(this.settings.chatAPI, {
                to: conversation.id,
                text: message
            }).map(function (value) { return m; });
        }
        else {
            var attendee = conversation.attendees.find(function (a) { return a.userId !== _this.settings.userName; });
            return this.http
                .post(this.settings.convAPI, {
                to: attendee.userId,
                text: message
            })
                .map(function (response) {
                conversation.id = response;
                _this.setConversationTitle(conversation);
                _this.joinConversationSubject.next(conversation);
                return m;
            });
        }
    };
    ChatService.prototype.getUsers = function () {
        return this.http.get(this.settings.userAPI).map(function (response) {
            var data = response;
            if (data && data.users) {
                return data.users;
            }
        });
    };
    ChatService.prototype.getConversations = function () {
        var _this = this;
        return this.http.get(this.settings.chatAPI).map(function (conversations) {
            conversations.forEach(function (value) { return _this.setConversationTitle(value); });
            return conversations;
        });
    };
    ChatService.prototype.setConversationTitle = function (conversation) {
        var _this = this;
        var title = '';
        conversation.attendees.forEach(function (attendee) {
            if (attendee &&
                attendee.userId &&
                attendee.userId !== _this.settings.userName) {
                title += attendee.userId + ' ';
            }
        });
        conversation.title = title.trim();
    };
    ChatService.prototype.setConnectionState = function (connectionState) {
        console.log('connection state changed to: ' + connectionState);
        this.currentState = connectionState;
        this.connectionStateSubject.next(connectionState);
    };
    ChatService.prototype.onReconnected = function () {
        this.setConnectionState(ConnectionState.Connected);
    };
    ChatService.prototype.onDisconnected = function () {
        this.setConnectionState(ConnectionState.Disconnected);
    };
    ChatService.prototype.onError = function (error) {
        this.connectionStateSubject.error(error);
    };
    ChatService.prototype.onUserConnected = function (user) {
        console.log('Chat Hub new user connected: ' + user.id);
        this.userConnectedSubject.next(user);
    };
    ChatService.prototype.onUserDisconnected = function (user) {
        console.log('Chat Hub user disconnected: ' + user.id);
        if (user.id !== this.settings.userName) {
            this.userDisconnectedSubject.next(user);
        }
    };
    ChatService.prototype.onMessageReceived = function (message) {
        console.log('Chat Hub message received: ' + message.conversationId);
        this.messageReceivedSubject.next(message);
    };
    ChatService.prototype.onJoinConversation = function (conversation) {
        console.log('Chat Hub join conversation: ' + conversation.id);
        this.setConversationTitle(conversation);
        this.joinConversationSubject.next(conversation);
    };
    ChatService = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["w" /* Injectable */])(),
        __metadata("design:paramtypes", [__WEBPACK_IMPORTED_MODULE_6__settings__["a" /* Settings */],
            __WEBPACK_IMPORTED_MODULE_1__angular_common_http__["a" /* HttpClient */],
            __WEBPACK_IMPORTED_MODULE_2__signalr_client__["c" /* SignalrService */]])
    ], ChatService);
    return ChatService;
}());



/***/ }),

/***/ "../../../../../src/app/shared/conversation.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return Conversation; });
var Conversation = (function () {
    function Conversation() {
        this.attendees = [];
        this.messages = [];
    }
    return Conversation;
}());



/***/ }),

/***/ "../../../../../src/app/shared/message.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return Message; });
var Message = (function () {
    function Message() {
    }
    return Message;
}());



/***/ }),

/***/ "../../../../../src/app/shared/settings.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return Settings; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};

var Settings = (function () {
    function Settings() {
        this.userName = 'test';
        this.userAPI = '/api/users';
        this.convAPI = '/api/chat/conv';
        this.chatAPI = '/api/chat';
    }
    Settings = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["w" /* Injectable */])()
    ], Settings);
    return Settings;
}());



/***/ }),

/***/ "../../../../../src/app/shared/signalr-client/hub-settings.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return HubSettings; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__aspnet_signalr__ = __webpack_require__("../../../../@aspnet/signalr/dist/browser/signalr.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__aspnet_signalr___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__aspnet_signalr__);

var HubSettings = (function () {
    function HubSettings() {
        this.logLevel = __WEBPACK_IMPORTED_MODULE_0__aspnet_signalr__["LogLevel"].Trace;
    }
    return HubSettings;
}());



/***/ }),

/***/ "../../../../../src/app/shared/signalr-client/index.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__signalr_module__ = __webpack_require__("../../../../../src/app/shared/signalr-client/signalr.module.ts");
/* harmony namespace reexport (by used) */ __webpack_require__.d(__webpack_exports__, "b", function() { return __WEBPACK_IMPORTED_MODULE_0__signalr_module__["a"]; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__signalr_service__ = __webpack_require__("../../../../../src/app/shared/signalr-client/signalr.service.ts");
/* harmony namespace reexport (by used) */ __webpack_require__.d(__webpack_exports__, "c", function() { return __WEBPACK_IMPORTED_MODULE_1__signalr_service__["b"]; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__hub_settings__ = __webpack_require__("../../../../../src/app/shared/signalr-client/hub-settings.ts");
/* harmony namespace reexport (by used) */ __webpack_require__.d(__webpack_exports__, "a", function() { return __WEBPACK_IMPORTED_MODULE_2__hub_settings__["a"]; });





/***/ }),

/***/ "../../../../../src/app/shared/signalr-client/signalr.module.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return SignalrModule; });
/* unused harmony export createHubConnection */
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_common__ = __webpack_require__("../../../common/esm5/common.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__aspnet_signalr__ = __webpack_require__("../../../../@aspnet/signalr/dist/browser/signalr.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__aspnet_signalr___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2__aspnet_signalr__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__hub_settings__ = __webpack_require__("../../../../../src/app/shared/signalr-client/hub-settings.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__signalr_service__ = __webpack_require__("../../../../../src/app/shared/signalr-client/signalr.service.ts");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};





var SignalrModule = (function () {
    function SignalrModule() {
    }
    SignalrModule = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["E" /* NgModule */])({
            imports: [
                __WEBPACK_IMPORTED_MODULE_1__angular_common__["a" /* CommonModule */]
            ],
            providers: [
                { provide: __WEBPACK_IMPORTED_MODULE_4__signalr_service__["a" /* HUB_CONNECTION_FACTORY */], useFactory: createHubConnection, deps: [__WEBPACK_IMPORTED_MODULE_3__hub_settings__["a" /* HubSettings */]] },
                __WEBPACK_IMPORTED_MODULE_4__signalr_service__["b" /* SignalrService */]
            ]
        })
    ], SignalrModule);
    return SignalrModule;
}());

function createHubConnection(hubSettings) {
    return function () { return new __WEBPACK_IMPORTED_MODULE_2__aspnet_signalr__["HubConnection"](new __WEBPACK_IMPORTED_MODULE_2__aspnet_signalr__["HttpConnection"](hubSettings.url, {
        transport: hubSettings.transportType,
        logger: hubSettings.logLevel
    })); };
}


/***/ }),

/***/ "../../../../../src/app/shared/signalr-client/signalr.service.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return HUB_CONNECTION_FACTORY; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "b", function() { return SignalrService; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_rxjs_Observable__ = __webpack_require__("../../../../rxjs/_esm5/Observable.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_Subject__ = __webpack_require__("../../../../rxjs/_esm5/Subject.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_add_observable_fromPromise__ = __webpack_require__("../../../../rxjs/_esm5/add/observable/fromPromise.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_rxjs_add_operator_map__ = __webpack_require__("../../../../rxjs/_esm5/add/operator/map.js");
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};





/**
 * {@link HubConnectionFactory} injection token
 */
var HUB_CONNECTION_FACTORY = new __WEBPACK_IMPORTED_MODULE_0__angular_core__["x" /* InjectionToken */]('HUB_CONNECTION_FACTORY');
/**
 * SignalR core service
 */
var SignalrService = (function () {
    /**
     * Initialise a new instance of {@link SignalrService}
     * @param factory the hub connection factorx
     */
    function SignalrService(factory) {
        this.factory = factory;
        /**
         * True if the hub connection is connected
         */
        this.isConnected = false;
        this.connectionCloseSubject = new __WEBPACK_IMPORTED_MODULE_2_rxjs_Subject__["a" /* Subject */]();
    }
    Object.defineProperty(SignalrService.prototype, "closed", {
        /**
         * Hub connection closed event
         */
        get: function () {
            return this.connectionCloseSubject;
        },
        enumerable: true,
        configurable: true
    });
    /**
     * Connects to the hub
     */
    SignalrService.prototype.connect = function () {
        var _this = this;
        this.connection = this.factory();
        this.serverMethods.forEach(function (callback) {
            _this.connection.on(callback.methodName, callback.method);
        });
        this.connection.onclose(function (e) {
            _this.isConnected = false;
            _this.connectionCloseSubject.next(e);
        });
        return __WEBPACK_IMPORTED_MODULE_1_rxjs_Observable__["a" /* Observable */].fromPromise(this.connection.start())
            .map(function () {
            _this.isConnected = true;
        });
    };
    /**
     * Disconnect from the hub
     */
    SignalrService.prototype.disconnect = function () {
        this.connection.stop();
    };
    /**
     * Adds a server method callback
     * @param methodName the server method name
     * @param method the callback
     */
    SignalrService.prototype.on = function (methodName, method) {
        this.serverMethods = this.serverMethods || [];
        this.serverMethods.push({ methodName: methodName, method: method });
    };
    /**
     * Invokes a server methods
     * @param methodName the method name
     * @param args the method arguments
     */
    SignalrService.prototype.invoke = function (methodName) {
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
        if (!this.isConnected) {
            return;
        }
        var argsArray = Array.prototype.slice.call(arguments);
        var subject = new __WEBPACK_IMPORTED_MODULE_2_rxjs_Subject__["a" /* Subject */]();
        var promise = this.connection.invoke.apply(this.connection, argsArray)
            .then(function (result) {
            subject.next(result);
        })
            .catch(function (err) {
            subject.error(err);
        });
        return subject;
    };
    SignalrService = __decorate([
        Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["w" /* Injectable */])(),
        __param(0, Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["v" /* Inject */])(HUB_CONNECTION_FACTORY)),
        __metadata("design:paramtypes", [Function])
    ], SignalrService);
    return SignalrService;
}());



/***/ }),

/***/ "../../../../../src/app/shared/user.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return User; });
var User = (function () {
    function User() {
    }
    return User;
}());



/***/ }),

/***/ "../../../../../src/environments/environment.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return environment; });
// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.
var environment = {
    production: false
};


/***/ }),

/***/ "../../../../../src/main.ts":
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__("../../../core/esm5/core.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__ = __webpack_require__("../../../platform-browser-dynamic/esm5/platform-browser-dynamic.js");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__app_app_module__ = __webpack_require__("../../../../../src/app/app.module.ts");
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__environments_environment__ = __webpack_require__("../../../../../src/environments/environment.ts");




if (__WEBPACK_IMPORTED_MODULE_3__environments_environment__["a" /* environment */].production) {
    Object(__WEBPACK_IMPORTED_MODULE_0__angular_core__["_7" /* enableProdMode */])();
}
Object(__WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__["a" /* platformBrowserDynamic */])().bootstrapModule(__WEBPACK_IMPORTED_MODULE_2__app_app_module__["a" /* AppModule */]);


/***/ }),

/***/ 0:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__("../../../../../src/main.ts");


/***/ })

},[0]);
//# sourceMappingURL=main.bundle.js.map