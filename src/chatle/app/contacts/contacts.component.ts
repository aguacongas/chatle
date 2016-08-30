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
    </ul>`,
  providers: [ChatService]
})
export class ContactsComponent implements OnInit 
{
  users: User[];
  error: any;

  constructor(private service: ChatService) { }

  ngOnInit() {
    if (this.service.currentState == ConnectionState.Connected) {
      this.getUsers();
    }

    this.service.connectionState
      .toPromise()
      .then(connectionState => {
        if (connectionState === ConnectionState.Connected) {
          this.getUsers();
        }
      })
      .catch(error => this.error = error);
  }
  
  private getUsers() {
    this.service.getUsers()
      .toPromise()
      .then(users => this.users = users)
      .catch(error => this.error = error);
  }
}
