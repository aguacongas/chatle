define('environment',["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = {
        debug: true,
        testing: true
    };
});

define('config/settings',["require", "exports"], function (require, exports) {
    "use strict";
    var Settings = (function () {
        function Settings() {
            this.apiBaseUrl = 'http://localhost:5000';
            this.userAPI = '/api/users';
            this.convAPI = '/api/chat/conv';
            this.chatAPI = '/api/chat';
            this.loginAPI = "/account/spaguess";
            this.logoffAPI = "/account/spalogoff";
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
            this.state = state;
        }
        return ConnectionStateChanged;
    }());
    exports.ConnectionStateChanged = ConnectionStateChanged;
});

define('events/conversationJoined',["require", "exports"], function (require, exports) {
    "use strict";
    var ConversationJoined = (function () {
        function ConversationJoined(conversation) {
            this.conversation = conversation;
        }
        return ConversationJoined;
    }());
    exports.ConversationJoined = ConversationJoined;
});

define('events/conversationSelected',["require", "exports"], function (require, exports) {
    "use strict";
    var ConversationSelected = (function () {
        function ConversationSelected(conversation) {
            this.conversation = conversation;
        }
        return ConversationSelected;
    }());
    exports.ConversationSelected = ConversationSelected;
});

define('events/messageReceived',["require", "exports"], function (require, exports) {
    "use strict";
    var MessageReceived = (function () {
        function MessageReceived(message) {
            this.message = message;
        }
        return MessageReceived;
    }());
    exports.MessageReceived = MessageReceived;
});

define('events/userConnected',["require", "exports"], function (require, exports) {
    "use strict";
    var UserConnected = (function () {
        function UserConnected(user) {
            this.user = user;
        }
        return UserConnected;
    }());
    exports.UserConnected = UserConnected;
});

define('events/userDisconnected',["require", "exports"], function (require, exports) {
    "use strict";
    var UserDisconnected = (function () {
        function UserDisconnected(id) {
            this.id = id;
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
define('services/chat.service',["require", "exports", 'aurelia-event-aggregator', 'aurelia-http-client', 'aurelia-framework', '../environment', '../config/settings', '../model/message', '../events/connectionStateChanged', '../events/conversationJoined', '../events/conversationSelected', '../events/messageReceived', '../events/userConnected', '../events/userDisconnected'], function (require, exports, aurelia_event_aggregator_1, aurelia_http_client_1, aurelia_framework_1, environment_1, settings_1, message_1, connectionStateChanged_1, conversationJoined_1, conversationSelected_1, messageReceived_1, userConnected_1, userDisconnected_1) {
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
            this.userName = sessionStorage.getItem('userName');
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
        ChatService.prototype.showConversation = function (conversation, router) {
            this.currentConversation = conversation;
            this.setConverationTitle(conversation);
            this.ea.publish(new conversationSelected_1.ConversationSelected(conversation));
            router.navigateToRoute('conversation', { id: conversation.title });
        };
        ChatService.prototype.sendMessage = function (conversation, message) {
            var _this = this;
            var m = new message_1.Message();
            m.conversationId = conversation.id;
            m.from = this.userName;
            m.text = message;
            if (conversation.id) {
                return new Promise(function (resolve, reject) {
                    _this.http.post(_this.settings.chatAPI, {
                        to: conversation.id,
                        text: message
                    })
                        .then(function (response) { return resolve(m); })
                        .catch(function (error) { return reject('Error when sending the message'); });
                });
            }
            else {
                var attendee_1;
                conversation.attendees.forEach(function (a) {
                    if (a.userId !== _this.userName) {
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
                        .catch(function (error) { return reject('Error when creating the conversation'); });
                });
            }
        };
        ChatService.prototype.login = function (userName) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                _this.http.get('xhrf')
                    .then(function (response) {
                    _this.http.createRequest(_this.settings.loginAPI)
                        .asPost()
                        .withHeader("X-XSRF-TOKEN", response.response)
                        .withContent({ userName: userName })
                        .send()
                        .then(function (response) {
                        _this.userName = userName;
                        sessionStorage.setItem('userName', userName);
                        resolve();
                        _this.start();
                    })
                        .catch(function (error) {
                        if (error.statusCode === 409) {
                            reject("This user name already exists, please chose a different name");
                        }
                        else {
                            reject(error.content.errors[0].ErrorMessage);
                        }
                    });
                })
                    .catch(function (error) { return reject('The service is down'); });
            });
        };
        ChatService.prototype.logoff = function () {
            delete this.userName;
            sessionStorage.removeItem('userName');
            jQuery.connection.hub.stop();
            this.http.createRequest(this.settings.logoffAPI)
                .asPost()
                .send();
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
                    .catch(function (error) { return reject('The service is down'); });
            });
        };
        ChatService.prototype.getConversations = function () {
            var _this = this;
            return new Promise(function (resolve, reject) {
                _this.http.get(_this.settings.chatAPI)
                    .then(function (response) {
                    if (response.response) {
                        var data = response.content;
                        if (data) {
                            resolve(data);
                        }
                    }
                    else {
                        resolve(null);
                    }
                })
                    .catch(function (error) { return reject('The service is down'); });
            });
        };
        ChatService.prototype.setConverationTitle = function (conversation) {
            var _this = this;
            if (conversation.title) {
                return;
            }
            var title = '';
            conversation.attendees.forEach(function (attendee) {
                if (attendee && attendee.userId && attendee.userId !== _this.userName) {
                    title += attendee.userId + ' ';
                }
            });
            conversation.title = title.trim();
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
            if (id !== this.userName) {
                this.ea.publish(new userDisconnected_1.UserDisconnected(id));
            }
        };
        ChatService.prototype.onMessageReceived = function (message) {
            this.ea.publish(new messageReceived_1.MessageReceived(message));
        };
        ChatService.prototype.onJoinConversation = function (conversation) {
            this.setConverationTitle(conversation);
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
define('app',["require", "exports", 'aurelia-framework', 'aurelia-router', 'aurelia-event-aggregator', './services/chat.service', './events/connectionStateChanged'], function (require, exports, aurelia_framework_1, aurelia_router_1, aurelia_event_aggregator_1, chat_service_1, connectionStateChanged_1) {
    "use strict";
    var App = (function () {
        function App(service, ea) {
            this.service = service;
            this.ea = ea;
            this.setIsConnected();
        }
        App.prototype.configureRouter = function (config, router) {
            config.title = 'Chatle';
            config.addPipelineStep('authorize', AuthorizeStep);
            config.map([
                { route: ['', 'home'], name: 'home', moduleId: 'pages/home', title: 'Home' },
                { route: 'account', name: 'account', moduleId: 'pages/account', title: 'Account' },
                { route: 'login', name: 'login', moduleId: 'pages/login', title: 'Login', anomymous: true }
            ]);
            this.router = router;
        };
        App.prototype.created = function () {
            var _this = this;
            this.ea.subscribe(connectionStateChanged_1.ConnectionStateChanged, function (e) {
                _this.setIsConnected();
            });
        };
        App.prototype.logoff = function () {
            this.service.logoff();
            this.router.navigateToRoute('login');
        };
        App.prototype.manage = function () {
            this.router.navigateToRoute('account');
        };
        App.prototype.setIsConnected = function () {
            this.isConnected = this.service.userName !== undefined && this.service.userName != null;
            this.userName = this.service.userName;
        };
        App = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [chat_service_1.ChatService, aurelia_event_aggregator_1.EventAggregator])
        ], App);
        return App;
    }());
    exports.App = App;
    var AuthorizeStep = (function () {
        function AuthorizeStep(service) {
            this.service = service;
        }
        AuthorizeStep.prototype.run = function (navigationInstruction, next) {
            if (navigationInstruction.getAllInstructions().some(function (i) {
                var route = i.config;
                return !route.anomymous;
            })) {
                var isLoggedIn = this.service.userName;
                if (!isLoggedIn) {
                    return next.cancel(new aurelia_router_1.Redirect('login'));
                }
            }
            return next();
        };
        AuthorizeStep = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [chat_service_1.ChatService])
        ], AuthorizeStep);
        return AuthorizeStep;
    }());
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

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('components/contact-list',["require", "exports", 'aurelia-framework', 'aurelia-event-aggregator', '../services/chat.service', '../events/userConnected', '../events/userDisconnected', '../events/connectionStateChanged'], function (require, exports, aurelia_framework_1, aurelia_event_aggregator_1, chat_service_1, userConnected_1, userDisconnected_1, connectionStateChanged_1) {
    "use strict";
    var ContactList = (function () {
        function ContactList(service, ea) {
            this.service = service;
            this.ea = ea;
            this.loadingMessage = "loading...";
        }
        ContactList.prototype.attached = function () {
            var _this = this;
            this.connectionStateChangeSubscription = this.ea.subscribe(connectionStateChanged_1.ConnectionStateChanged, function (e) {
                if (e.state === chat_service_1.ConnectionState.Connected) {
                    _this.getUser();
                }
            });
            if (this.service.currentState === chat_service_1.ConnectionState.Connected) {
                this.getUser();
            }
        };
        ContactList.prototype.detached = function () {
            this.connectionStateChangeSubscription.dispose();
            if (this.userConnectedSubscription) {
                this.userConnectedSubscription.dispose();
            }
            if (this.userDisconnectedSubscription) {
                this.userDisconnectedSubscription.dispose();
            }
        };
        ContactList.prototype.getUser = function () {
            var _this = this;
            this.service.getUsers()
                .then(function (users) {
                _this.users = users;
                _this.userConnectedSubscription = _this.ea.subscribe(userConnected_1.UserConnected, function (e) {
                    _this.removeUser(e.user.id);
                    _this.users.unshift(e.user);
                });
                _this.userDisconnectedSubscription = _this.ea.subscribe(userDisconnected_1.UserDisconnected, function (e) {
                    _this.removeUser(e.id);
                });
            })
                .catch(function (error) { return _this.loadingMessage = error; });
        };
        ContactList.prototype.removeUser = function (id) {
            var user;
            this.users.forEach(function (u) {
                if (u.id === id) {
                    user = u;
                }
            });
            if (user) {
                var index = this.users.indexOf(user);
                this.users.splice(index, 1);
            }
        };
        ContactList = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [chat_service_1.ChatService, aurelia_event_aggregator_1.EventAggregator])
        ], ContactList);
        return ContactList;
    }());
    exports.ContactList = ContactList;
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
define('components/contact',["require", "exports", 'aurelia-framework', 'aurelia-event-aggregator', 'aurelia-router', '../services/chat.service', '../model/user', '../model/conversation', '../model/attendee', '../events/conversationSelected'], function (require, exports, aurelia_framework_1, aurelia_event_aggregator_1, aurelia_router_1, chat_service_1, user_1, conversation_1, attendee_1, conversationSelected_1) {
    "use strict";
    var Contact = (function () {
        function Contact(service, ea, router) {
            this.service = service;
            this.ea = ea;
            this.router = router;
        }
        Contact.prototype.select = function () {
            if (!this.user.conversation) {
                var conversation = new conversation_1.Conversation();
                var attendees = new Array();
                var attendee = new attendee_1.Attendee();
                attendee.userId = this.user.id;
                attendees.push(attendee);
                conversation.attendees = attendees;
                conversation.messages = new Array();
                this.user.conversation = conversation;
            }
            this.service.showConversation(this.user.conversation, this.router);
        };
        Contact.prototype.attached = function () {
            var _this = this;
            this.conversationSelectedSubscription = this.ea.subscribe(conversationSelected_1.ConversationSelected, function (e) {
                var conv = e.conversation;
                var attendees = conv.attendees;
                _this.isSelected = false;
                if (attendees.length == 2) {
                    attendees.forEach(function (a) {
                        if (a.userId !== _this.service.userName && a.userId === _this.user.id) {
                            _this.isSelected = true;
                        }
                    });
                }
            });
        };
        Contact.prototype.detached = function () {
            this.conversationSelectedSubscription.dispose();
        };
        __decorate([
            aurelia_framework_1.bindable, 
            __metadata('design:type', user_1.User)
        ], Contact.prototype, "user", void 0);
        Contact = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [chat_service_1.ChatService, aurelia_event_aggregator_1.EventAggregator, aurelia_router_1.Router])
        ], Contact);
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
define('components/conversation-component',["require", "exports", 'aurelia-framework', 'aurelia-router', '../services/chat.service'], function (require, exports, aurelia_framework_1, aurelia_router_1, chat_service_1) {
    "use strict";
    var ConversationComponent = (function () {
        function ConversationComponent(service, router) {
            this.service = service;
            this.router = router;
        }
        ConversationComponent.prototype.activate = function (params, routeConfig) {
            if (!params) {
                delete this.service.currentConversation;
            }
            this.conversation = this.service.currentConversation;
            if (!this.conversation) {
                this.router.navigateToRoute('home');
            }
            else {
                routeConfig.navModel.setTitle(this.conversation.title);
            }
        };
        ConversationComponent.prototype.sendMessage = function () {
            var _this = this;
            this.service.sendMessage(this.conversation, this.message)
                .then(function (message) { return _this.conversation.messages.unshift(message); });
            this.message = '';
        };
        ConversationComponent = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [chat_service_1.ChatService, aurelia_router_1.Router])
        ], ConversationComponent);
        return ConversationComponent;
    }());
    exports.ConversationComponent = ConversationComponent;
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
define('components/conversation-list',["require", "exports", 'aurelia-framework', 'aurelia-event-aggregator', '../services/chat.service', '../events/conversationJoined', '../events/userDisconnected'], function (require, exports, aurelia_framework_1, aurelia_event_aggregator_1, chat_service_1, conversationJoined_1, userDisconnected_1) {
    "use strict";
    var ConversationList = (function () {
        function ConversationList(service, ea) {
            this.service = service;
            this.ea = ea;
        }
        ConversationList.prototype.attached = function () {
            var _this = this;
            this.service.getConversations()
                .then(function (conversations) {
                _this.conversations = conversations;
                _this.conversations.forEach(function (c) { return _this.setConversationTitle(c); });
                _this.userDisconnectedSubscription = _this.ea.subscribe(userDisconnected_1.UserDisconnected, function (e) {
                    _this.conversations.forEach(function (c) {
                        var attendees = c.attendees;
                        if (attendees.length === 2) {
                            attendees.forEach(function (a) {
                                if (a.userId === e.id) {
                                    var index = _this.conversations.indexOf(c);
                                    _this.conversations.splice(index, 1);
                                }
                            });
                        }
                    });
                });
                _this.conversationJoinedSubscription = _this.ea.subscribe(conversationJoined_1.ConversationJoined, function (e) {
                    var conversation = e.conversation;
                    _this.conversations.unshift(e.conversation);
                });
            });
        };
        ConversationList.prototype.detached = function () {
            if (this.conversationJoinedSubscription) {
                this.conversationJoinedSubscription.dispose();
            }
            if (this.userDisconnectedSubscription) {
                this.userDisconnectedSubscription.dispose();
            }
        };
        ConversationList.prototype.setConversationTitle = function (conversation) {
            var _this = this;
            var title = '';
            conversation.attendees.forEach(function (attendee) {
                if (attendee && attendee.userId && attendee.userId !== _this.service.userName) {
                    title += attendee.userId + ' ';
                }
            });
            conversation.title = title.trim();
        };
        ConversationList = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [chat_service_1.ChatService, aurelia_event_aggregator_1.EventAggregator])
        ], ConversationList);
        return ConversationList;
    }());
    exports.ConversationList = ConversationList;
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
define('components/conversation-preview',["require", "exports", 'aurelia-framework', 'aurelia-event-aggregator', 'aurelia-router', '../services/chat.service', '../model/conversation', '../events/conversationSelected', '../events/messageReceived'], function (require, exports, aurelia_framework_1, aurelia_event_aggregator_1, aurelia_router_1, chat_service_1, conversation_1, conversationSelected_1, messageReceived_1) {
    "use strict";
    var ConversationPreview = (function () {
        function ConversationPreview(service, ea, router) {
            this.service = service;
            this.ea = ea;
            this.router = router;
        }
        ConversationPreview.prototype.select = function () {
            this.service.showConversation(this.conversation, this.router);
        };
        ConversationPreview.prototype.attached = function () {
            var _this = this;
            this.conversationSelectedSubscription = this.ea.subscribe(conversationSelected_1.ConversationSelected, function (e) {
                if (e.conversation.id === _this.conversation.id) {
                    _this.isSelected = true;
                }
                else {
                    _this.isSelected = false;
                }
            });
            this.messageReceivedSubscription = this.ea.subscribe(messageReceived_1.MessageReceived, function (e) {
                var message = e.messaqe;
                if (message.conversationId === _this.conversation.id) {
                    _this.conversation.messages.unshift(e.message);
                }
            });
        };
        ConversationPreview.prototype.detached = function () {
            this.conversationSelectedSubscription.dispose();
            this.messageReceivedSubscription.dispose();
        };
        __decorate([
            aurelia_framework_1.bindable, 
            __metadata('design:type', conversation_1.Conversation)
        ], ConversationPreview.prototype, "conversation", void 0);
        ConversationPreview = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [chat_service_1.ChatService, aurelia_event_aggregator_1.EventAggregator, aurelia_router_1.Router])
        ], ConversationPreview);
        return ConversationPreview;
    }());
    exports.ConversationPreview = ConversationPreview;
});



define("pages/account", [],function(){});

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('pages/home',["require", "exports", 'aurelia-framework', 'aurelia-router', 'aurelia-event-aggregator', '../services/chat.service', '../events/connectionStateChanged'], function (require, exports, aurelia_framework_1, aurelia_router_1, aurelia_event_aggregator_1, chat_service_1, connectionStateChanged_1) {
    "use strict";
    var Home = (function () {
        function Home(service, config, ea) {
            this.service = service;
            this.config = config;
            this.ea = ea;
        }
        Home.prototype.configureRouter = function (config, router) {
            config.map([
                { route: ['', 'conversation/:id'], name: 'conversation', moduleId: '../components/conversation-component' }
            ]);
            this.router = router;
        };
        Home.prototype.attached = function () {
            var _this = this;
            this.connectionStateSubscription = this.ea.subscribe(connectionStateChanged_1.ConnectionStateChanged, function (e) {
                _this.setIsDisconnected(e.state);
            });
            this.setIsDisconnected(this.service.currentState);
            if (this.service.currentState !== chat_service_1.ConnectionState.Connected) {
                this.service.start();
            }
        };
        Home.prototype.detached = function () {
            this.connectionStateSubscription.dispose();
        };
        Home.prototype.setIsDisconnected = function (state) {
            if (state === chat_service_1.ConnectionState.Disconnected || state === chat_service_1.ConnectionState.Error) {
                this.isDisconnected = true;
            }
            else {
                this.isDisconnected = false;
            }
        };
        Home = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [chat_service_1.ChatService, aurelia_router_1.RouterConfiguration, aurelia_event_aggregator_1.EventAggregator])
        ], Home);
        return Home;
    }());
    exports.Home = Home;
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
define('pages/login',["require", "exports", 'aurelia-framework', 'aurelia-router', '../services/chat.service'], function (require, exports, aurelia_framework_1, aurelia_router_1, chat_service_1) {
    "use strict";
    var Login = (function () {
        function Login(service, router) {
            this.service = service;
            this.router = router;
        }
        Login.prototype.login = function (userName) {
            var _this = this;
            this.service.login(userName)
                .then(function () {
                _this.router.navigateToRoute('home');
            })
                .catch(function (error) { return _this.errorMessage = error; });
        };
        Login = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [chat_service_1.ChatService, aurelia_router_1.Router])
        ], Login);
        return Login;
    }());
    exports.Login = Login;
});

define('resources/index',["require", "exports"], function (require, exports) {
    "use strict";
    function configure(config) {
    }
    exports.configure = configure;
});

define('text!app.html', ['module'], function(module) { module.exports = "<template>\n    <require from=\"./css/site.css\"></require>\n    <div class=\"navbar navbar-default navbar-fixed-top\">\n        <div class=\"container\">\n            <div class=\"navbar-header\">\n                <button type=\"button\" class=\"navbar-toggle\" data-toggle=\"collapse\" data-target=\".navbar-collapse\">\n                    <span class=\"icon-bar menu\"></span>\n                    <span class=\"icon-bar menu\"></span>\n                    <span class=\"icon-bar menu\"></span>\n                </button>\n                <a class=\"navbar-brand title\" href=\"/\">chatle</a>\n            </div>\n            <div class=\"navbar-collapse collapse\">\n                <ul class=\"nav navbar-nav\" if.bind=\"isConnected\">\n                    <li click.delegate=\"manage()\"><a>Welcom ${userName}!</a></li>\n                    <li click.delegate=\"logoff()\"><a>Log off</a></li>\n                </ul>\n            </div>\n        </div>\n    </div>\n    <div class=\"container body-content\">\n        <router-view></router-view>\n    </div>\n    <footer>\n        <p>� 2016 - chatle</p>\n    </footer>\n</template>\n"; });
define('text!css/site.css', ['module'], function(module) { module.exports = "html {\n    font-family: cursive\n}\n/* Move down content because we have a fixed navbar that is 50px tall */\nbody.chatle {\n    padding-top: 50px;\n    padding-bottom: 20px;\n}\n\n/* Wrapping element */\n/* Set some basic padding to keep content from hitting the edges */\n.body-content {\n    padding-left: 15px;\n    padding-right: 15px;\n}\n\n.navbar.navbar-default {\n    background-color: #000;\n}\n\n.title.navbar-brand:focus,\n.title.navbar-brand:hover,\n.nav.navbar-nav > li > a:hover {\n    color: #fff;    \n}\n\nli:hover {\n    cursor: pointer;\n}\n\n/* Set widths on the form inputs since otherwise they're 100% wide */\ninput,\nselect,\ntextarea {\n    max-width: 280px;\n}\n\nul\n{\n    padding-left: 0;\n    list-style-type: none;\n}\n\nsmall {\n    color: #666666;\n}\n\n/* Responsive: Portrait tablets and up */\n@media screen and (min-width: 768px) {\n    .jumbotron {\n        margin-top: 20px;\n    }\n\n    .body-content {\n        padding: 0;\n    }\n}\n\n.list-group-item {\n    border: 0;\n    padding: 0;\n}\n.list-group-item.active {\n    background-color: #eee;\n}"; });
define('text!components/contact-list.html', ['module'], function(module) { module.exports = "<template>\n  <require from=\"./contact\"></require>\n  <div class=\"contact-list\">\n    <span if.bind=\"!users\">${loadingMessage}</span>\n    <ul class=\"list-group\" if.bind=\"users\">\n      <contact repeat.for=\"user of users\" user.bind=\"user\"></contact>\n    </ul>\n  </div>\n</template>"; });
define('text!css/site.min.css', ['module'], function(module) { module.exports = "html{font-family:cursive}body{padding-top:50px;padding-bottom:20px}.console{font-family:'Lucida Console',Monaco,monospace}.body-content{padding-left:15px;padding-right:15px}.navbar.navbar-default{background-color:#fff}.nav.navbar-nav>li>a:hover,.title.navbar-brand:focus,.title.navbar-brand:hover{color:#222}input,select,textarea{max-width:280px}ul{padding-left:0;list-style-type:none}small{color:#666}@media screen and (min-width:768px){.jumbotron{margin-top:20px}.body-content{padding:0}}"; });
define('text!components/contact.html', ['module'], function(module) { module.exports = "<template>\n    <li class=\"list-group-item ${isSelected ? 'active' : ''}\" click.delegate=\"select()\"><a>${user.id}</a></li>\n</template>"; });
define('text!components/conversation-component.html', ['module'], function(module) { module.exports = "<template>\n    <div if.bind=\"conversation\">\n        <h6>${conversation.title}</h6>\n        <form class=\"form-inline\">\n            <input class=\"form-control\" value.bind=\"message\" placeholder=\"message...\">\n            <button type=\"submit\" class=\"btn btn-default\" click.delegate=\"sendMessage()\" disabled.bind=\"!message\">send</button>\n        </form>\n        <ul>\n            <li repeat.for=\"message of conversation.messages\">\n                <small>${message.from}</small><br />\n                <span>${message.text}</span>\n            </li>\n        </ul>\n    </div>\n    <h1 if.bind=\"!conversation\">WELCOME TO THIS REALLY SIMPLE CHAT</h1>\n</template>"; });
define('text!components/conversation-list.html', ['module'], function(module) { module.exports = "<template>\n  <require from=\"./conversation-preview\"></require>\n  <div class=\"conversation-list\">\n    <ul class=\"list-group\">\n      <conversation-preview repeat.for=\"conversation of conversations\" conversation.bind=\"conversation\"></conversation-preview>\n    </ul>\n  </div>\n</template>"; });
define('text!components/conversation-preview.html', ['module'], function(module) { module.exports = "<template>\n  <li class=\"list-group-item ${isSelected ? 'active' : ''}\" click.delegate=\"select()\">\n    <a>${conversation.title}</a><br/>\n    <span>${conversation.messages[0].text}</span>\n  </li>\n</template>"; });
define('text!pages/account.html', ['module'], function(module) { module.exports = ""; });
define('text!pages/home.html', ['module'], function(module) { module.exports = "<template>\n    <require from=\"../components/contact-list\"></require>\n    <require from=\"../components/conversation-list\"></require>\n\n    <div class=\"row\">\n        <ul if.bind=\"isDisconnected\">\n            <li><a class=\"text-danger\" href=\"/home\">You are disconnected</a></li>\n        </ul>\n        <div class=\"col-xs-3\">\n            <h6>CONVERSATION</h6>\n            <conversation-list></conversation-list>\n        </div>\n        <router-view class=\"col-xs-6\"></router-view>\n        <div class=\"col-xs-3\">\n            <h6>CONNECTED</h6>\n            <contact-list></contact-list>\n        </div>\n    </div>\n</template>"; });
define('text!pages/login.html', ['module'], function(module) { module.exports = "<template>\n    <h2>Loging</h2>\n    <hr />\n    <form class=\"form-horizontal\">\n        <div class=\"form-group\">\n            <label class=\"col-xs-3 control-label\" for=\"userName\"></label>\n            <div class=\"col-xs-9\">\n                <input class=\"form-control\" name=\"userName\" value.bind=\"userName\" />\n                <span class=\"text-danger\" if.bind=\"errorMessage\">${errorMessage}</span>\n            </div>\n        </div>\n        <div class=\"form-group\">\n            <div class=\"col-xs-offset-3 col-xs-9\">\n                <input type=\"submit\" value=\"Log in\" class=\"btn btn-default\" click.delegate=\"login(userName)\" />\n            </div>\n        </div>\n    </form>\n</template>"; });
//# sourceMappingURL=app-bundle.js.map