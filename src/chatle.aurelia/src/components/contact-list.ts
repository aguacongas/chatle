import { autoinject } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

import { ChatService, ConnectionState } from '../services/chat.service';
import { User } from '../model/user';
import { UserConnected } from '../events/userConnected'
import { UserDisconnected } from '../events/userDisconnected'
import { ConnectionStateChanged } from '../events/connectionStateChanged';

@autoinject
export class ContactList {
    users: User[];
    loadingMessage = "loading...";
    private userConnectedSubscription: Subscription;
    private userDisconnectedSubscription: Subscription;
    private connectionStateChangeSubscription: Subscription;

    constructor(private service: ChatService, private ea: EventAggregator) { }

    attached() {
        this.connectionStateChangeSubscription = this.ea.subscribe(ConnectionStateChanged, e => {
            if ((<ConnectionStateChanged>e).state === ConnectionState.Connected) {
                this.getUser();
            }            
        });

        if (this.service.currentState === ConnectionState.Connected) {
            this.getUser();
        }        
    }

    detached() {
        this.connectionStateChangeSubscription.dispose();
        if (this.userConnectedSubscription) {
            this.userConnectedSubscription.dispose();
        }
        if (this.userDisconnectedSubscription) {
            this.userDisconnectedSubscription.dispose();
        }
    }

    private getUser() {
        this.service.getUsers()
            .then(users => {
                this.users = users;

                this.userConnectedSubscription = this.ea.subscribe(UserConnected, e => {
                    this.removeUser((<UserConnected>e).user.id);
                    this.users.unshift(e.user);
                });

                this.userDisconnectedSubscription = this.ea.subscribe(UserDisconnected, e => {
                    this.removeUser((<UserDisconnected>e).id);
                });
            })
            .catch(error => this.loadingMessage = error);
    }

    private removeUser(id: string) {
        let user: User;
        this.users.forEach(u => {
            if (u.id === id) {
                user = u;
            }
        });

        if (user) {
            let index = this.users.indexOf(user);
            this.users.splice(index, 1);
        }        
    }
}