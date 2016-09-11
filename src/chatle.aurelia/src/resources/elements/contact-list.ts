import { autoinject } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

import { ChatService } from '../../services/chat.service';
import { User } from '../../model/user';
import { UserConnected } from '../../events/userConnected'
import { UserDisconnected } from '../../events/userDisconnected'

@autoinject
export class ContactList {
  users: User[];
  loadingMessage = "loading...";
  private userConnectedSubscription: Subscription;
  private userDisconnectedSubscription: Subscription;

  constructor(private service: ChatService, private ea: EventAggregator) { }

  attached() {
    this.service.getUsers()
      .then(users => {
        this.users = users;

        this.userConnectedSubscription = this.ea.subscribe(UserConnected, user => {
          this.users.unshift(user);
        });

        this.userDisconnectedSubscription = this.ea.subscribe(UserDisconnected, id => {
          let user: User;
          this.users.forEach(u => {
            if(u.id === id) {
              user = u;
            }
          });
          let index = this.users.indexOf(user);
          this.users.splice(index, 1);
        });
      })
      .catch(error => this.loadingMessage = error);
    }

  detached() {
    if (this.userConnectedSubscription) {
      this.userConnectedSubscription.dispose();
    }
    if (this.userDisconnectedSubscription) {
      this.userDisconnectedSubscription.dispose();
    }
  }
}