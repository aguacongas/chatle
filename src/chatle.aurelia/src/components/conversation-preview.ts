import { bindable, autoinject } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';
import { Router, RouterConfiguration } from 'aurelia-router';

import { ChatService } from '../services/chat.service';
import { Conversation } from '../model/conversation';
import { ConversationSelected } from '../events/conversationSelected';

@autoinject
export class ConversationPreview {
    @bindable conversation: Conversation;
    isSelected: boolean;
    private conversationSelectecSubscription: Subscription;

    constructor(private service: ChatService, private ea: EventAggregator, private router: Router) { }

    select() {
        this.service.showConversation(this.conversation, this.router);
    }

    attached() {
        this.conversationSelectecSubscription = this.ea.subscribe(ConversationSelected, e => {
            if (e.conversation.id === this.conversation.id) {
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