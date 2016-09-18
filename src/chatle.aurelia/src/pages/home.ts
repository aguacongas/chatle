import { autoinject } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

import { ChatService, ConnectionState } from '../services/chat.service';
import { ConnectionStateChanged } from '../events/connectionStateChanged';

@autoinject
export class Home {
    router: Router;
    isDisconnected: boolean;

    private connectionStateSubscription: Subscription;
    private showConversationSubscription: Subscription;

    constructor(public service: ChatService, private config: RouterConfiguration, private ea: EventAggregator) { }

    configureRouter(config: RouterConfiguration, router: Router) {
        config.map([
            { route: ['', 'conversation/:id'], name: 'conversation', moduleId: '../components/conversation-component' }
        ]);

        this.router = router;
    }

    attached() {
        this.connectionStateSubscription = this.ea.subscribe(ConnectionStateChanged, e => {
            this.setIsDisconnected((<ConnectionStateChanged>e).state);
        });

        this.setIsDisconnected(this.service.currentState);

        if (this.service.currentState !== ConnectionState.Connected) {
            this.service.start();
        }
    }

    detached() {
        this.connectionStateSubscription.dispose();
    }

    private setIsDisconnected(state: ConnectionState) {
        if (state === ConnectionState.Error) {
            this.service.logoff();
        } if (state === ConnectionState.Disconnected) {
            this.isDisconnected = true;
        } else {
            this.isDisconnected = false;
        }
    }
}