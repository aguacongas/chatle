import { autoinject } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

import { ChatService } from '../services/chat.service';
import { Conversation } from '../model/conversation';
import { ConversationJoined } from '../events/conversationJoined'
import { UserDisconnected } from '../events/userDisconnected'

@autoinject
export class ConversationList {
  conversations: Conversation[];
  private conversationJoinedSubscription: Subscription;
  private userDisconnectedSubscription: Subscription;

  constructor(private service: ChatService, private ea: EventAggregator) { }

  attached() {
    this.conversations = new Array<Conversation>();

    this.service.getConversations()
      .then(conversations => {
        this.conversations = conversations;
        this.conversations.forEach(c => this.setConversationTitle(c));
        
        this.userDisconnectedSubscription = this.ea.subscribe(UserDisconnected, e => {
          this.conversations.forEach(c => {
            let attendees = c.attendees;
            if (attendees.length === 2) {
              attendees.forEach(a => {
                if (a.userId === e.id) {
                  let index = this.conversations.indexOf(c);
                  this.conversations.splice(index, 1);
                }
              });
            }
          });          
        });

        this.conversationJoinedSubscription = this.ea.subscribe(ConversationJoined, e => {
          let conversation = (<ConversationJoined>e).conversation;          
          this.conversations.unshift(e.conversation);
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

  private setConversationTitle(conversation: Conversation) {
      let title = '';
      conversation.attendees.forEach(attendee => {
          if (attendee && attendee.userId && attendee.userId !== this.service.userName) {
              title += attendee.userId + ' ';
          }                
      });
      conversation.title = title.trim();
  }
}