import { Component, Input, OnInit } from '@angular/core';

import { User } from '../shared/user';
import { Conversation } from '../shared/conversation';
import { Message } from '../shared/message';
import { Attendee } from '../shared/attendee';
import { ChatService } from '../shared/chat.service'
import { Settings } from '../shared/settings'

@Component({
  selector: 'contact',
  templateUrl: '/app/contacts/contact.component.html'
})
export class ContactComponent implements OnInit {
    @Input()
    user: User;
    isCurrentUser: boolean;

    constructor(private service: ChatService, private settings: Settings) {  }

    ngOnInit() {
        this.service.joinConversation
            .subscribe(conversion => {
                if (!this.user.conversation
                    && conversion.attendees.length < 3
                    && conversion.attendees.some(a => a.userId === this.user.id)) {
                    this.user.conversation = conversion
                }
            });
        
        this.isCurrentUser = this.settings.userName === this.user.id;
    }

    onClick() {
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

        this.service.showConversation(this.user.conversation);
    }
}
