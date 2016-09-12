import { autoinject } from 'aurelia-framework';
import { Router, Redirect, NavigationInstruction, RouterConfiguration, Next, RouteConfig } from 'aurelia-router';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';


import { Settings } from './config/settings';
import { ChatService, ConnectionState } from './services/chat.service';
import { ConnectionStateChanged } from './events/connectionStateChanged';

interface CustomRouteConfig extends RouteConfig {
    anomymous: boolean;
}

@autoinject
export class App {
    router: Router;
    isConnected: boolean;
    userName: string;

    constructor(private settings: Settings, private service: ChatService, private ea: EventAggregator) {
        this.setIsConnected();
    }

    configureRouter(config: RouterConfiguration, router: Router) {
        config.title = 'Chatle';
        config.addPipelineStep('authorize', AuthorizeStep);
        config.map([
            { route: ['', 'home'], name: 'home', moduleId: 'pages/home', title: 'Home' },
            { route: 'account', name: 'account', moduleId: 'pages/account', title: 'Account' },
            { route: 'login', name: 'login', moduleId: 'pages/login', title: 'Login', anomymous: true }
        ]);

        this.router = router;
    }

    private setIsConnected() {
        this.isConnected = this.settings.userName !== undefined && this.settings.userName != null;
        this.userName = this.settings.userName;
    }

    created() {
        this.ea.subscribe(ConnectionStateChanged, state => {
            this.setIsConnected();
        });
    }

    logoff() {
        this.service.logoff();
        this.router.navigateToRoute('login');
    }

    manage() {
        this.router.navigateToRoute('account');
    }
}

@autoinject
class AuthorizeStep {

    constructor(private settings: Settings) { }

    run(navigationInstruction: NavigationInstruction, next: Next): Promise<any> {
        if (navigationInstruction.getAllInstructions().some(i => {
            let route = i.config as CustomRouteConfig;
            return !route.anomymous
        })) {
            var isLoggedIn = this.settings.userName;
            if (!isLoggedIn) {
                return next.cancel(new Redirect('login'));
            }
        }

        return next();
    }
}