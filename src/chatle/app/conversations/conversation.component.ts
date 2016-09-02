import { Component, Input, OnInit } from '@angular/core';

import { Conversation } from '../shared/conversation';
import { ChatService } from '../shared/chat.service';

@Component({
    selector: 'conversation',
    templateUrl: '/app/conversations/conversation.component.html'
})
export class ConversationComponent implements OnInit {
    @Input()
    conversation: Conversation;
    message: string;
    error: any;

    constructor(private service: ChatService) { }

    ngOnInit() {
        this.service.openConversation
            .subscribe(
                conversation => this.conversation = conversation);
    }

    send() {        
        this.service.sendMessage(this.conversation, this.message)
            .subscribe(
                m => this.conversation.messages.unshift(m),
                error => this.error = error);
    }
}
