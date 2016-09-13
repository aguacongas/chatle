import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

import { ChatService, ConnectionState } from '../services/chat.service';
import { ConnectionStateChanged } from '../events/connectionStateChanged';

@autoinject
export class Home {
    isDisconnected: boolean;

    private connectionStateSubscription: Subscription;

    constructor(public service: ChatService, private router: Router, private ea: EventAggregator) { }

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
        if (state === ConnectionState.Disconnected || state === ConnectionState.Error) {
            this.isDisconnected = true;
        } else {
            this.isDisconnected = false;
        }
    }
}