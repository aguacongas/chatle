import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';
import { ValidationRules } from 'aurelia-validation';

import { ChatService, ConnectionState } from '../services/chat.service';
import { ChangePassword } from '../model/changePassword';
import { ConnectionStateChanged } from '../events/connectionStateChanged';

@autoinject
export class Account {
    userName: string;
    model = new ChangePassword();
    errorMessage: string;
    isGuess: boolean;

    private connectionStateSubscription: Subscription;

    constructor(private service: ChatService, private router: Router, private ea: EventAggregator) { 
        this.userName = service.userName;
        this.isGuess = service.isGuess;

        ValidationRules.ensure((m: ChangePassword) => m.confirmPassword)
            .equals(this.model.newPassword)
            .on(this.model);
    }

    attached() {
        this.connectionStateSubscription = this.ea.subscribe(ConnectionStateChanged, e => {
            if ((<ConnectionStateChanged>e).state === ConnectionState.Connected) {
                this.isGuess = this.service.isGuess;
            }
        });
    }

    detached() {
        this.connectionStateSubscription.dispose();
    }

    changePassword() {
        this.service.changePassword(this.model)
            .then(() => {
                this.isGuess = false;
                this.router.navigateToRoute('home');
            })
            .catch(error => this.errorMessage = error);
    }
}