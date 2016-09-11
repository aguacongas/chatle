import { bindable, autoinject } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

import { ChatService } from '../../services/chat.service';
import { Conversation } from '../../model/conversation';
import { ConversationSelected } from '../../events/conversationSelected';

export class ConversationPreview {
  @bindable conversation: Conversation;
  isSelected: boolean;
  private conversationSelectecSubscription: Subscription;

  constructor(private service: ChatService, private ea: EventAggregator) { }

  select() {
    this.isSelected = true;
    this.service.showConversation(this.conversation);
  }

  attached() {
    this.ea.subscribe(ConversationSelected, c => {
      if (c.id === this.conversation.id) {
        this.isSelected = true;
      } else {
        this.isSelected = false;
      }
    });
  }

  detached() {
    this.conversationSelectecSubscription.dispose();
  }
}

