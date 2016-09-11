import { autoinject } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

import { ChatService } from '../../services/chat.service';
import { Conversation } from '../../model/conversation';
import { ConversationJoined } from '../../events/conversationJoined'
import { UserDisconnected } from '../../events/userDisconnected'

@autoinject
export class ConversationList {
  conversations: Conversation[];
  private conversationJoinedSubscription: Subscription;
  private userDisconnectedSubscription: Subscription;

  constructor(private service: ChatService, private ea: EventAggregator) { }

  attached() {
    this.service.getConversations()
      .then(conversations => {
        this.conversations = conversations;

        this.userDisconnectedSubscription = this.ea.subscribe(UserDisconnected, id => {
          this.conversations.forEach(c => {
            let attendees = c.attendees;
            if (attendees.length === 2) {
              attendees.forEach(a => {
                if (a.userId === id) {
                  let index = this.conversations.indexOf(c);
                  this.conversations.splice(index, 1);
                }
              });
            }
          });          
        });

        this.conversationJoinedSubscription = this.ea.subscribe(ConversationJoined, c => {
          this.conversations.unshift(c);
        });
      });
    }

  detached() {
    if (this.conversationJoinedSubscription) {
      this.conversationJoinedSubscription.dispose();
    }
    if (this.userDisconnectedSubscription) {
      this.userDisconnectedSubscription.dispose();
    }
  }
}