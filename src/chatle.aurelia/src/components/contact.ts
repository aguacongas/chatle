import { bindable, autoinject } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';
import { Router } from 'aurelia-router';

import { ChatService } from '../services/chat.service';
import { User } from '../model/user';
import { Conversation } from '../model/conversation';
import { Attendee } from '../model/attendee';
import { Message } from '../model/message';
import { ConversationSelected } from '../events/conversationSelected';

@autoinject
export class Contact {
    @bindable user: User;
    isSelected: boolean;
    private conversationSelectedSubscription: Subscription;

    constructor(private service: ChatService, private ea: EventAggregator, private router: Router) { }

    select() {
        if (!this.user.conversation) {
            let conversation = new Conversation();
            let attendees = new Array<Attendee>();
            let attendee = new Attendee();

            attendee.userId = this.user.id;
            attendees.push(attendee);
            conversation.attendees = attendees;
            conversation.messages = new Array<Message>();

            this.user.conversation = conversation;
        }

        this.service.showConversation(this.user.conversation, this.router);        
    }

    attached() {
        this.conversationSelectedSubscription = this.ea.subscribe(ConversationSelected, e => {
            let conv = e.conversation as Conversation;
            let attendees = conv.attendees;

            this.isSelected = false;
            if (attendees.length == 2) {
                attendees.forEach(a => {
                    if (a.userId !== this.service.userName && a.userId === this.user.id) {
                        this.isSelected = true;
                    }
                })
            }
        });
    }

    detached() {
        this.conversationSelectedSubscription.dispose();
    }
}

