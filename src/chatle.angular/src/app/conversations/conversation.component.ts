import { Component, Input, OnInit } from '@angular/core';

import { Conversation } from '../shared/conversation';
import { ChatService } from '../shared/chat.service';

@Component({
    selector: 'chatle-conversation',
    templateUrl: './conversation.component.html',
    styleUrls: ['./conversation.component.css']
})
export class ConversationComponent implements OnInit {
    @Input()
    conversation: Conversation;
    message: string;
    error: any;
    userName: string;

    constructor(private service: ChatService) { }

    ngOnInit() {
        this.userName = this.service.settings.userName;
        this.service.openConversation
            .subscribe(
                conversation => this.conversation = conversation);
        this.service.userDiscconnected
            .subscribe(
                user => {
                  if (user.isRemoved && this.conversation
                        && this.conversation.attendees.some(a => a.userId === user.id)
                        && this.conversation.attendees.length < 3) {
                            delete this.conversation;
                    }
                }
            );
    }

    send() {
        this.service.sendMessage(this.conversation, this.message)
            .subscribe(
                m => this.conversation.messages.unshift(m),
                error => this.error = error);
        this.message = null;
    }

    displayFrom(i: number) {
      const messages = this.conversation.messages;
      return i === 0 || messages[i].from !== messages[i - 1].from;
    }
}
