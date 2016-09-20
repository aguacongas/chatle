import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';
import { ValidationRules } from 'aurelia-validation';
import {ValidationControllerFactory, ValidationController} from 'aurelia-validation';


import { ChatService, ConnectionState } from '../services/chat.service';
import { ChangePassword } from '../model/changePassword';
import { ConnectionStateChanged } from '../events/connectionStateChanged';

@autoinject
export class Account {
    userName: string;
    oldPassword: string;
    newPassword: string;
    confirmPassword: string;    
    errorMessage: string;
    isGuess: boolean;
    controller: ValidationController;

    private connectionStateSubscription: Subscription;

    constructor(private service: ChatService, 
            private router: Router, 
            private ea: EventAggregator,
            controllerFactory: ValidationControllerFactory) { 
        this.userName = service.userName;
        this.isGuess = service.isGuess;

        ValidationRules
            .ensure((a: Account) => a.confirmPassword)
                .equals(this.newPassword).withMessage("The new password and confirmation password do not match.")
            .on(this);

        ValidationRules            
            .ensure((a: Account) => a.newPassword)
                .matches(new RegExp('^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])$')).withMessage('Your password must contains at least one uppercase letter, one digit and one special charactere.')
                .minLength(6).withMessage('Your password must be 6 characteres long')
            .on(this);

        this.controller = controllerFactory.createForCurrentScope();
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
        this.controller.validate()
            .then(() => {
                let model = <ChangePassword> {
                    oldPassword: this.oldPassword,
                    newPassword: this.newPassword, 
                    confirmPassword: this.confirmPassword
                };

                this.service.changePassword(model)
                    .then(() => {
                        this.isGuess = false;
                        this.router.navigateToRoute('home');
                    })
                    .catch(error => this.errorMessage = error);
            })
            .catch(error => console.error(error));
    }
}