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
                user => this.users.unshift(user),
                error => this.error = error);

        this.service.userDiscconnected
            .subscribe(
                id => {
                    let index = this.users.findIndex(value => value.id === id);
                    this.users.slice(index);
                },
                error => this.error = error);

        if (this.service.currentState === ConnectionState.Connected) {
            this.getUsers();
        }
    }

    private getUsers() {
        this.service.getUsers()
            .subscribe(
            users => this.users = users,
            error => this.error = error);
    }
}
