import { Component, OnInit } from '@angular/core';

import { ChatService, ConnectionState } from '../shared/chat.service'
import { User } from '../shared/user'

@Component({
  selector: 'contacts',
  template: `<h1>Contacts</h1>
    <ul>
      <li *ngFor="let user of users">
        <span>{{user.id}}</span> {{user.name}}
      </li>
    </ul>`
})
export class ContactsComponent implements OnInit 
{
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
