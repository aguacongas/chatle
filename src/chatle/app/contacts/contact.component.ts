import { Component, Input, OnInit } from '@angular/core';

import { User } from '../shared/user';
import { Conversation } from '../shared/conversation';
import { ChatService } from '../shared/chat.service'

@Component({
  selector: 'contact',
  templateUrl: '/app/contacts/contact.component.html'
})
export class ContactComponent implements OnInit {
    @Input()
    user: User;

    constructor(private service: ChatService) { }

    ngOnInit() {
        this.service.joinConversation
            .subscribe(conversion => {
                if (!this.user.conversation
                    && conversion.attendees.length < 3
                    && conversion.attendees.some(user => user.id === this.user.id)) {
                    this.user.conversation = conversion
                }
            });
    }

    onClick() {
        if (!this.user.conversation) {
            let conversation = new Conversation();
            conversation.attendees.push(this.user);
        }

        this.service.showConversation(this.user.conversation);
    }
}
