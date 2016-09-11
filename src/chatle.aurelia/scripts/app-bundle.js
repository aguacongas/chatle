define('config/settings',["require", "exports"], function (require, exports) {
    "use strict";
    var Settings = (function () {
        function Settings() {
            this.apiBaseUrl = 'http://localhost:5000';
            this.userAPI = '/api/users';
            this.convAPI = '/api/chat/conv';
            this.chatAPI = '/api/chat';
            this.loginAPI = "/account/spaguess";
        }
        return Settings;
    }());
    exports.Settings = Settings;
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
define('app',["require", "exports", 'aurelia-framework', 'aurelia-router', './config/settings'], function (require, exports, aurelia_framework_1, aurelia_router_1, settings_1) {
    "use strict";
    var App = (function () {
        function App(settings) {
            this.settings = settings;
            this.loadUser();
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
        App.prototype.loadUser = function () {
            this.settings.userName = sessionStorage.getItem('userName');
        };
        App = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [settings_1.Settings])
        ], App);
        return App;
    }());
    exports.App = App;
    var AuthorizeStep = (function () {
        function AuthorizeStep(settings) {
            this.settings = settings;
        }
        AuthorizeStep.prototype.run = function (navigationInstruction, next) {
            if (navigationInstruction.getAllInstructions().some(function (i) {
                var route = i.config;
                return !route.anomymous;
            })) {
                var isLoggedIn = this.settings.userName;
                if (!isLoggedIn) {
                    return next.cancel(new aurelia_router_1.Redirect('login'));
                }
            }
            return next();
        };
        AuthorizeStep = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [settings_1.Settings])
        ], AuthorizeStep);
        return AuthorizeStep;
    }());
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

define('events/connectionStateChanged',["require", "exports"], function (require, exports) {
    "use strict";
    var ConnectionStateChanged = (function () {
        function ConnectionStateChanged(state) {
        }
        return ConnectionStateChanged;
    }());
    exports.ConnectionStateChanged = ConnectionStateChanged;
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
define('pages/home',["require", "exports", 'aurelia-framework', 'aurelia-router', '../config/settings'], function (require, exports, aurelia_framework_1, aurelia_router_1, settings_1) {
    "use strict";
    var Home = (function () {
        function Home(settings, router) {
            this.settings = settings;
            this.router = router;
        }
        Home.prototype.logoff = function () {
            delete this.settings.userName;
            sessionStorage.removeItem('userName');
            this.router.navigateToRoute('login');
        };
        Home.prototype.manage = function () {
            this.router.navigateToRoute('account');
        };
        Home = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [settings_1.Settings, aurelia_router_1.Router])
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
define('pages/login',["require", "exports", 'aurelia-http-client', 'aurelia-framework', 'aurelia-router', '../config/settings'], function (require, exports, aurelia_http_client_1, aurelia_framework_1, aurelia_router_1, settings_1) {
    "use strict";
    var Login = (function () {
        function Login(settings, http, router) {
            this.settings = settings;
            this.http = http;
            this.router = router;
            http.configure(function (builder) { return builder
                .withBaseUrl(settings.apiBaseUrl)
                .withCredentials(true); });
        }
        Login.prototype.login = function () {
            var _this = this;
            this.http.get('xhrf')
                .then(function (response) {
                return _this.http
                    .configure(function (builder) { return builder.withHeader("X-XSRF-TOKEN", response.response); })
                    .post(_this.settings.loginAPI, JSON.stringify(_this.userName))
                    .then(function (response) {
                    _this.settings.userName = _this.userName;
                    sessionStorage.setItem('userName', _this.userName);
                    _this.router.navigateToRoute('home');
                })
                    .catch(function (error) { return _this.errorMessage = error; });
            })
                .catch(function (error) { return _this.errorMessage = error; });
        };
        Login = __decorate([
            aurelia_framework_1.autoinject, 
            __metadata('design:paramtypes', [settings_1.Settings, aurelia_http_client_1.HttpClient, aurelia_router_1.Router])
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

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define('resources/elements/contact-list',["require", "exports", 'aurelia-framework', 'aurelia-event-aggregator', '../../services/chat.service', '../../events/userConnected', '../../events/userDisconnected'], function (require, exports, aurelia_framework_1, aurelia_event_aggregator_1, chat_service_1, userConnected_1, userDisconnected_1) {
    "use strict";
    var ContactList = (function () {
        function ContactList(service, ea) {
            this.service = service;
            this.ea = ea;
            this.loadingMessage = "loading...";
        }
        ContactList.prototype.attached = function () {
            var _this = this;
            this.service.getUsers()
                .then(function (users) {
                _this.users = users;
                _this.userConnectedSubscription = _this.ea.subscribe(userConnected_1.UserConnected, function (user) {
                    _this.users.unshift(user);
                });
                _this.userDisconnectedSubscription = _this.ea.subscribe(userDisconnected_1.UserDisconnected, function (id) {
                    var user;
                    _this.users.forEach(function (u) {
                        if (u.id === id) {
                            user = u;
                        }
                    });
                    var index = _this.users.indexOf(user);
                    _this.users.splice(index, 1);
                });
            })
                .catch(function (error) { return _this.loadingMessage = error; });
        };
        ContactList.prototype.detached = function () {
            if (this.userConnectedSubscription) {
                this.userConnectedSubscription.dispose();
            }
            if (this.userDisconnectedSubscription) {
                this.userDisconnectedSubscription.dispose();
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
define('resources/elements/contact',["require", "exports", 'aurelia-framework', 'aurelia-event-aggregator', '../../services/chat.service', '../../model/user', '../../model/conversation', '../../model/attendee', '../../events/conversationSelected'], function (require, exports, aurelia_framework_1, aurelia_event_aggregator_1, chat_service_1, user_1, conversation_1, attendee_1, conversationSelected_1) {
    "use strict";
    var Contact = (function () {
        function Contact(service, ea) {
            this.service = service;
            this.ea = ea;
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
            this.service.showConversation(this.user.conversation);
        };
        Contact.prototype.attached = function () {
            var _this = this;
            this.conversationSelectedSubscription = this.ea.subscribe(conversationSelected_1.ConversationSelected, function (c) {
                var conv = c;
                var attendees = conv.attendees;
                _this.isSelected = false;
                if (attendees.length == 2) {
                    attendees.forEach(function (a) {
                        if (a.userId === _this.user.id) {
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
            __metadata('design:paramtypes', [chat_service_1.ChatService, aurelia_event_aggregator_1.EventAggregator])
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
define('resources/elements/conversation-list',["require", "exports", 'aurelia-framework', 'aurelia-event-aggregator', '../../services/chat.service', '../../events/conversationJoined', '../../events/userDisconnected'], function (require, exports, aurelia_framework_1, aurelia_event_aggregator_1, chat_service_1, conversationJoined_1, userDisconnected_1) {
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
                _this.userDisconnectedSubscription = _this.ea.subscribe(userDisconnected_1.UserDisconnected, function (id) {
                    _this.conversations.forEach(function (c) {
                        var attendees = c.attendees;
                        if (attendees.length === 2) {
                            attendees.forEach(function (a) {
                                if (a.userId === id) {
                                    var index = _this.conversations.indexOf(c);
                                    _this.conversations.splice(index, 1);
                                }
                            });
                        }
                    });
                });
                _this.conversationJoinedSubscription = _this.ea.subscribe(conversationJoined_1.ConversationJoined, function (c) {
                    _this.conversations.unshift(c);
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
define('resources/elements/conversation-preview',["require", "exports", 'aurelia-framework', '../../model/conversation', '../../events/conversationSelected'], function (require, exports, aurelia_framework_1, conversation_1, conversationSelected_1) {
    "use strict";
    var ConversationPreview = (function () {
        function ConversationPreview(service, ea) {
            this.service = service;
            this.ea = ea;
        }
        ConversationPreview.prototype.select = function () {
            this.isSelected = true;
            this.service.showConversation(this.conversation);
        };
        ConversationPreview.prototype.attached = function () {
            var _this = this;
            this.ea.subscribe(conversationSelected_1.ConversationSelected, function (c) {
                if (c.id === _this.conversation.id) {
                    _this.isSelected = true;
                }
                else {
                    _this.isSelected = false;
                }
            });
        };
        ConversationPreview.prototype.detached = function () {
            this.conversationSelectecSubscription.dispose();
        };
        __decorate([
            aurelia_framework_1.bindable, 
            __metadata('design:type', conversation_1.Conversation)
        ], ConversationPreview.prototype, "conversation", void 0);
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

define('text!app.html', ['module'], function(module) { module.exports = "<template>\n  <require from=\"bootstrap/css/bootstrap.css\"></require>\n\n  <div class=\"container\">\n    <div class=\"row\">\n      <router-view></router-view>\n    </div>\n  </div>\n</template>\n"; });
define('text!pages/account.html', ['module'], function(module) { module.exports = ""; });
define('text!pages/home.html', ['module'], function(module) { module.exports = "<template>\n    <require from=\"../resources/elements/contact-list\"></require>\n    <require from=\"../resources/elements/conversation\"></require>\n    <require from=\"../resources/elements/conversation-list\"></require>\n\n    <ul class=\"nav nav-bar\">\n        <li click.delegate=\"manage\">Welcom ${settings.userName}!</li>\n        <li click.delegate=\"logoff\">Log off</li>\n    </ul>\n    <div>\n        <contact-list class=\"col-xs-3\"></contact-list>\n        <conversation class=\"col-xs-6\"></conversation>\n        <conversation-list class=\"col-xs-3\"></conversation-list>\n    </div> \n</template>"; });
define('text!pages/login.html', ['module'], function(module) { module.exports = "<template>\n    <h2>Loging</h2>\n    <hr/>\n    <form class=\"form-horizontal\">\n        <div class=\"form-group\">\n            <label class=\"col-xs-3 control-label\" for=\"userName\"></label>\n            <div class=\"col-xs-9\">\n                <input class=\"form-control\" name=\"userName\" value.bind=\"userName\" />\n                <span class=\"text-danger\" if.bind=\"errorMessage\">${errorMessage}</span>\n            </div>\n        </div>\n        <div class=\"form-group\">\n            <div class=\"col-xs-offset-3 col-xs-9\">\n                <input type=\"submit\" value=\"Log in\" class=\"btn btn-default\" click.delegate=\"login()\" />\n            </div>\n        </div>\n    </form>\n</template>"; });
define('text!resources/elements/contact-list.html', ['module'], function(module) { module.exports = "<template>\n  <require from=\"./contact\"></require>\n  <div class=\"contact-list\">\n    <span if.bind=\"!users\">${loadingMessage}</span>\n    <ul class=\"list-group\" if.bind=\"users\">\n      <contact repeat.for=\"user of users\" user.bind=\"user\"></contact>\n    </ul>\n  </div>\n</template>"; });
define('text!resources/elements/contact.html', ['module'], function(module) { module.exports = "<template>\n  <li class=\"list-group-item ${isSelected ? 'active' : ''}\" click.delegate=\"select()\">${user.id}</li>\n</template>"; });
define('text!resources/elements/conversation-list.html', ['module'], function(module) { module.exports = "<template>\n  <require from=\"./conversation-preview\"></require>\n  <div class=\"conversation-list\">\n    <ul class=\"list-group\">\n      <conversation-preview repeat.for=\"conversation of conversations\" conversation.bind=\"conversation\"></conversation-preview>\n    </ul>\n  </div>\n</template>"; });
define('text!resources/elements/conversation-preview.html', ['module'], function(module) { module.exports = "<template>\n  <li class=\"list-group-item ${isSelected ? 'active' : ''}\" click.delegate=\"select()\">${conversation.title}</li>\n</template>"; });
define('text!resources/elements/conversation.html', ['module'], function(module) { module.exports = "<template>\n  <h1>${value}</h1>\n</template>"; });
//# sourceMappingURL=app-bundle.js.map