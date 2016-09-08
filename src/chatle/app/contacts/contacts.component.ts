import { Component, OnInit } from '@angular/core';

import { ChatService, ConnectionState } from '../shared/chat.service'
import { User } from '../shared/user'

@Component({
    selector: 'contacts',
    templateUrl: '/app/contacts/contacts.component.html'
})
export class ContactsComponent implements OnInit {
    users: User[];
    error: any;

    constructor(private service: ChatService) { }

    ngOnInit() {
        this.service.connectionState
            .subscribe(
                connectionState => {
                    if (connectionState === ConnectionState.Connected) {
                        this.getUsers();
                    }
                },
                error => this.error = error);

        this.service.userConnected
            .subscribe(
                user => {
                    this.removeUser(user.id);
                    this.users.unshift(user)
                },
                error => this.error = error);

        this.service.userDiscconnected
            .subscribe(
                id => {
                    this.removeUser(id);
                },
                error => this.error = error);

        if (this.service.currentState === ConnectionState.Connected) {
            this.getUsers();
        }
    }

    private removeUser(id: string) {
        let index = this.users.findIndex(user => user.id === id);
        if (index !== -1) {
            this.users.splice(index, 1);
        }
    }

    private getUsers() {
        this.service.getUsers()
            .subscribe(
            users => this.users = users,
            error => this.error = error);
    }
}
