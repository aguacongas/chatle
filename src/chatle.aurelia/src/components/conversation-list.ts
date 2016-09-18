import { autoinject } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

import { ChatService,ConnectionState } from '../services/chat.service';
import { Conversation } from '../model/conversation';
import { ConversationJoined } from '../events/conversationJoined';
import { UserDisconnected } from '../events/userDisconnected';
import { ConnectionStateChanged } from '../events/connectionStateChanged';

@autoinject
export class ConversationList {
  conversations: Conversation[];
  private conversationJoinedSubscription: Subscription;
  private userDisconnectedSubscription: Subscription;
  private connectionStateSubscription: Subscription;

  constructor(private service: ChatService, private ea: EventAggregator) { }

  attached() {
    this.conversations = new Array<Conversation>();

    this.getConversations();

    this.connectionStateSubscription = this.ea.subscribe(ConnectionStateChanged, e => {
      let state = (<ConnectionStateChanged>e).state;
      if (state === ConnectionState.Disconnected) { 
        // remove conversation on log off, disconnection
        this.conversations.splice(this.conversations.length);
      } else if (state === ConnectionState.Connected) {
        // get conversation for reconnect
        this.getConversations();
      }
    });
  }

  detached() {
    this.Unsubscribe();
    this.connectionStateSubscription.dispose();
  }

  private Unsubscribe() {
    if (this.conversationJoinedSubscription) {
      this.conversationJoinedSubscription.dispose();
    }
    if (this.userDisconnectedSubscription) {
      this.userDisconnectedSubscription.dispose();
    }
  }

  private getConversations() {
    this.service.getConversations()
      .then(conversations => {
        // Unsubscribe before in case of connection state changed to connected
        this.Unsubscribe();

        if (!conversations) {
          return;
        }

        conversations.forEach(c => this.setConversationTitle(c));
        this.conversations = conversations;      
        
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