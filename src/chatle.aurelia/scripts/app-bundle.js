define('config/settings',["require", "exports"], function (require, exports) {
    "use strict";
    var Settings = (function () {
        function Settings() {
            this.apiBaseUrl = 'http://localhost:5000';
            this.userAPI = '/api/users';
            this.convAPI = '/api/chat/conv';
            this.chatAPI = '/api/chat';
        }
        return Settings;
    }());
    exports.Settings = Settings;
});

define('model/attendee',["require", "exports"], function (require, exports) {
    "use strict";
    var Attendee = (function () {
        function Attendee() {
        }
        return Attendee;
    }());
    exports.Attendee = Attendee;
});

define('model/message',["require", "exports"], function (require, exports) {
    "use strict";
    var Message = (function () {
        function Message() {
        }
        return Message;
    }());
    exports.Message = Message;
});

define('model/conversation',["require", "exports"], function (require, exports) {
    "use strict";
    var Conversation = (function () {
        function Conversation() {
        }
        return Conversation;
    }());
    exports.Conversation = Conversation;
});

define('model/user',["require", "exports"], function (require, exports) {
    "use strict";
    var User = (function () {
        function User() {
        }
        return User;
    }());
    exports.User = User;
});

define('events/connectionStateChanged',["require", "exports"], function (require, exports) {
    "use strict";
    var ConnectionStateChanged = (function () {
        function ConnectionStateChanged(state) {
        }
        return ConnectionStateChanged;
    }());
    exports.ConnectionStateChanged = ConnectionStateChanged;
});

define('events/conversationJoined',["require", "exports"], function (require, exports) {
    "use strict";
    var ConversationJoined = (function () {
        function ConversationJoined(conversation) {
        }
        return ConversationJoined;
    }());
    exports.ConversationJoined = ConversationJoined;
});

define('events/conversationSelected',["require", "exports"], function (require, exports) {
    "use strict";
    var ConversationSelected = (function () {
        function ConversationSelected(conversation) {
        }
        return ConversationSelected;
    }());
    exports.ConversationSelected = ConversationSelected;
});

define('events/messageReceived',["require", "exports"], function (require, exports) {
    "use strict";
    var MessageReceived = (function () {
        function MessageReceived(message) {
        }
        return MessageReceived;
    }());
    exports.MessageReceived = MessageReceived;
});

define('events/userConnected',["require", "exports"], function (require, exports) {
    "use strict";
    var UserConnected = (function () {
        function UserConnected(user) {
        }
        return UserConnected;
    }());
    exports.UserConnected = UserConnected;
});

define('events/userDisconnected',["require", "exports"], function (require, exports) {
    "use strict";
    var UserDisconnected = (function () {
        function UserDisconnected(id) {
        }
        return UserDisconnected;
    }());
    exports.UserDisconnected = UserDisconnected;
});

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('services/chat.service',["require", "exports", 'aurelia-event-aggregator', 'aurelia-http-client', 'aurelia-framework', '../environment', '../config/settings', '../model/message', '../events/connectionStateChanged', '../events/conversationJoined', '../events/messageReceived', '../events/userConnected', '../events/userDisconnected'], function (require, exports, aurelia_event_aggregator_1, aurelia_http_client_1, aurelia_framework_1, environment_1, settings_1, message_1, connectionStateChanged_1, conversationJoined_1, messageReceived_1, userConnected_1, userDisconnected_1) {
    "use strict";
    (function (ConnectionState) {
        ConnectionState[ConnectionState["Connected"] = 1] = "Connected";
        ConnectionState[ConnectionState["Disconnected"] = 2] = "Disconnected";
        ConnectionState[ConnectionState["Error"] = 3] = "Error";
    })(exports.ConnectionState || (exports.ConnectionState = {}));
    var ConnectionState = exports.ConnectionState;
    var ChatService = (function () {
        function ChatService(settings, ea, http) {
            this.settings = settings;
            this.ea = ea;
            this.http = http;
            this.currentState = ConnectionState.Disconnected;
            http.configure(function (builder) { return builder
                .withBaseUrl(settings.apiBaseUrl)
                .withCredentials(true); });
        }
        ChatService.prototype.start = function () {
            var _this = this;
            var debug = environment_1.default.debug;
            jQuery.connection.hub.logging = debug;
            var connection = jQuery.connection;
            var chatHub = connection.chat;
            chatHub.client.userConnected = function (user) { return _this.onUserConnected(user); };
            chatHub.client.userDisconnected = function (id) { return _this.onUserDisconnected(id); };
            chatHub.client.messageReceived = function (message) { return _this.onMessageReceived(message); };
            chatHub.client.joinConversation = function (conversation) { return _this.onJoinConversation(conversation); };
            if (debug) {
                jQuery.connection.hub.stateChanged(function (change) {
                    var oldState, newState;
                    for (var state in jQuery.signalR.connectionState) {
                        if (jQuery.signalR.connectionState[state] === change.oldState) {
                            oldState = state;
                        }
                        if (jQuery.signalR.connectionState[state] === change.newState) {
                            newState = state;
                        }
                    }
                    console.log("Chat Hub state changed from " + oldState + " to " + newState);
                });
            }
            jQuery.connection.hub.reconnected(function () { return _this.onReconnected(); });
            jQuery.connection.hub.error(function (error) { return _this.onError(error); });
            jQuery.connection.hub.disconnected(function () { return _this.onDisconnected(); });
            jQuery.connection.hub.start()
                .done(function (response) { return _this.setConnectionState(ConnectionState.Connected); })
                .fail(function (error) { return _this.setConnectionState(ConnectionState.Error); });
        };
        ChatService.prototype.showConversation = function (conversation) {
            this.ea.publish(new conversationJoined_1.ConversationJoined(conversation));
        };
        ChatService.prototype.sendMessage = function (conversation, message) {
            var _this = this;
            var m = new message_1.Message();
            m.conversationId = conversation.id;
            m.from = this.settings.userName;
            m.text = message;
            if (conversation.id) {
                return new Promise(function (resolve, reject) {
                    _this.http.post(_this.settings.chatAPI, {
                        to: conversation.id,
                        text: message
                    })
                        .then(function (response) { return resolve(m); })
                        .catch(function (error) { return reject(error); });
                });
            }
            else {
                var attendee_1;
                conversation.attendees.forEach(function (a) {
                    if (a.userId !== _this.settings.userName) {
                        attendee_1 = a;
                    }
                });
                return new Promise(function (resolve, reject) {
                    _this.http.post(_this.settings.convAPI, {
                        to: attendee_1.userId,
                        text: message
                    })
                        .then(function (response) {
                        conversation.id = response.content;
                        _this.ea.publish(new conversationJoined_1.ConversationJoined(conversation));
                        resolve(m);
                    })
                        .catch(function (error) { return reject(error); });
                });
            }
        };
        ChatService.prototype.getUsers = function () {
            var _this = this;
            return new Promise(function (resolve, reject) {
                _this.http.get(_this.settings.userAPI)
                    .then(function (response) {
                    var data = response.content;
                    if (data && data.users) {
                        resolve(data.users);
                    }
                })
                    .catch(function (error) { return reject(error); });
            });
        };
        ChatService.prototype.getConversations = function () {
            var _this = this;
            return new Promise(function (resolve, reject) {
                _this.http.get(_this.settings.chatAPI)
                    .then(function (response) {
                    var data = response.content;
                    if (data) {
                        resolve(data);
                    }
                })
                    .catch(function (error) { return reject(error); });
            });
        };
        ChatService.prototype.setConnectionState = function (connectionState) {
            console.log('connection state changed to: ' + connectionState);
            this.currentState = connectionState;
            this.ea.publish(new connectionStateChanged_1.ConnectionStateChanged(connectionState));
        };
        ChatService.prototype.onReconnected = function () {
            this.setConnectionState(ConnectionState.Connected);
        };
        ChatService.prototype.onDisconnected = function () {
            this.setConnectionState(ConnectionState.Disconnected);
        };
        ChatService.prototype.onError = function (error) {
            this.setConnectionState(ConnectionState.Error);
        };
        ChatService.prototype.onUserConnected = function (user) {
            console.log("Chat Hub new user connected: " + user.id);
            this.ea.publish(new userConnected_1.UserConnected(user));
        };
        ChatService.prototype.onUserDisconnected = function (id) {
            console.log("Chat Hub user disconnected: " + id);
            if (id !== this.settings.userName) {
                this.ea.publish(new userDisconnected_1.UserDisconnected(id));
            }
        };
        ChatService.prototype.onMessageReceived = function (message) {
            this.ea.publish(new messageReceived_1.MessageReceived(message));
        };
        ChatService.prototype.onJoinConversation = function (conversation) {
            this.ea.publish(new conversationJoined_1.ConversationJoined(conversation));
        };
        ChatService = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [settings_1.Settings, aurelia_event_aggregator_1.EventAggregator, aurelia_http_client_1.HttpClient])
        ], ChatService);
        return ChatService;
    }());
    exports.ChatService = ChatService;
});

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('app',["require", "exports", 'aurelia-framework', './services/chat.service'], function (require, exports, aurelia_framework_1, chat_service_1) {
    "use strict";
    var App = (function () {
        function App(service) {
            this.service = service;
            this.message = 'Hello World!';
            service.start();
        }
        App.prototype.configureRouter = function (config, router) {
            config.title = 'Chatle';
            config.map([
                { route: '', moduleId: 'home', title: 'Home' },
                { route: 'account', moduleId: 'account', title: 'Account' },
                { route: 'login', moduleId: 'login', title: 'Login' }
            ]);
            this.router = router;
        };
        App = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [chat_service_1.ChatService])
        ], App);
        return App;
    }());
    exports.App = App;
});

define('environment',["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = {
        debug: true,
        testing: true
    };
});

define('main',["require", "exports", './environment'], function (require, exports, environment_1) {
    "use strict";
    Promise.config({
        warnings: {
            wForgottenReturn: false
        }
    });
    function configure(aurelia) {
        aurelia.use
            .standardConfiguration()
            .feature('resources');
        if (environment_1.default.debug) {
            aurelia.use.developmentLogging();
        }
        if (environment_1.default.testing) {
            aurelia.use.plugin('aurelia-testing');
        }
        aurelia.start().then(function () { return aurelia.setRoot(); });
    }
    exports.configure = configure;
});

define('resources/index',["require", "exports"], function (require, exports) {
    "use strict";
    function configure(config) {
    }
    exports.configure = configure;
});

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('resources/elements/contact',["require", "exports", 'aurelia-framework'], function (require, exports, aurelia_framework_1) {
    "use strict";
    var Contact = (function () {
        function Contact() {
        }
        Contact.prototype.valueChanged = function (newValue, oldValue) {
        };
        __decorate([
            aurelia_framework_1.bindable, 
            __metadata('design:type', Object)
        ], Contact.prototype, "value", void 0);
        return Contact;
    }());
    exports.Contact = Contact;
});

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('resources/elements/contacts',["require", "exports", 'aurelia-framework'], function (require, exports, aurelia_framework_1) {
    "use strict";
    var Contacts = (function () {
        function Contacts() {
        }
        Contacts.prototype.valueChanged = function (newValue, oldValue) {
        };
        __decorate([
            aurelia_framework_1.bindable, 
            __metadata('design:type', Object)
        ], Contacts.prototype, "value", void 0);
        return Contacts;
    }());
    exports.Contacts = Contacts;
});

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('resources/elements/conversation',["require", "exports", 'aurelia-framework'], function (require, exports, aurelia_framework_1) {
    "use strict";
    var Conversation = (function () {
        function Conversation() {
        }
        Conversation.prototype.valueChanged = function (newValue, oldValue) {
        };
        __decorate([
            aurelia_framework_1.bindable, 
            __metadata('design:type', Object)
        ], Conversation.prototype, "value", void 0);
        return Conversation;
    }());
    exports.Conversation = Conversation;
});

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('resources/elements/conversationpreview',["require", "exports", 'aurelia-framework'], function (require, exports, aurelia_framework_1) {
    "use strict";
    var ConversationPreview = (function () {
        function ConversationPreview() {
        }
        ConversationPreview.prototype.valueChanged = function (newValue, oldValue) {
        };
        __decorate([
            aurelia_framework_1.bindable, 
            __metadata('design:type', Object)
        ], ConversationPreview.prototype, "value", void 0);
        return ConversationPreview;
    }());
    exports.ConversationPreview = ConversationPreview;
});

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('resources/elements/conversations',["require", "exports", 'aurelia-framework'], function (require, exports, aurelia_framework_1) {
    "use strict";
    var Conversations = (function () {
        function Conversations() {
        }
        Conversations.prototype.valueChanged = function (newValue, oldValue) {
        };
        __decorate([
            aurelia_framework_1.bindable, 
            __metadata('design:type', Object)
        ], Conversations.prototype, "value", void 0);
        return Conversations;
    }());
    exports.Conversations = Conversations;
});

define('text!app.html', ['module'], function(module) { module.exports = "<template>\n  <h1>${message}</h1>\n</template>\n"; });
define('text!resources/elements/contact.html', ['module'], function(module) { module.exports = "<template>\n  <h1>${value}</h1>\n</template>"; });
define('text!resources/elements/contacts.html', ['module'], function(module) { module.exports = "<template>\n  <h1>${value}</h1>\n</template>"; });
define('text!resources/elements/conversation.html', ['module'], function(module) { module.exports = "<template>\n  <h1>${value}</h1>\n</template>"; });
define('text!resources/elements/conversationpreview.html', ['module'], function(module) { module.exports = "<template>\n  <h1>${value}</h1>\n</template>"; });
define('text!resources/elements/conversations.html', ['module'], function(module) { module.exports = "<template>\n  <h1>${value}</h1>\n</template>"; });
//# sourceMappingURL=app-bundle.js.map