import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';
import { ValidationRules } from 'aurelia-validation';
import { ValidationControllerFactory, ValidationController} from 'aurelia-validation';

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
        this.controller.validate();

        if (this.controller.errors.length === 0) {
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
        }
    }
}

ValidationRules.customRule(
    'matchesProperty',
    (value, obj, otherPropertyName) => 
        value === null
        || value === undefined
        || value === ''
        || obj[otherPropertyName] === null
        || obj[otherPropertyName] === undefined
        || obj[otherPropertyName] === ''
        || value === obj[otherPropertyName],
    '${$displayName} must match ${$config.otherPropertyName}',
    otherPropertyName => ({ otherPropertyName }));

        
ValidationRules
    .ensure((a: Account) => a.confirmPassword)
        .displayName('Confirm new password')
        .required()
        .satisfiesRule('matchesProperty', 'newPassword')
            .withMessage('Confirm new password must match New password')
    .ensure((a: Account) => a.newPassword)
        .displayName("New password")
        .required()
        .matches(/(?=.*[A-Z])(?=.*[!@#$&\.\*\-\+\=\?£€])(?=.*[0-9])/)
            .withMessage('${$displayName} must contains at least one uppercase letter, one digit and one special charactere.')
        .minLength(6)
    .on(Account);