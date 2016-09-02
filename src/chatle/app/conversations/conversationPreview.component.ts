import { Component, Input, OnInit } from '@angular/core';

import { Conversation } from '../shared/conversation';
import { ChatService } from '../shared/chat.service';

@Component({
    selector: 'conversation-preview',
    templateUrl: '/app/conversations/conversationPreview.component.html'
})
export class ConversationPreviewComponent implements OnInit {
    @Input()
    conversation: Conversation;

    constructor(private service: ChatService) { }

    ngOnInit() {
        this.service.openConversation
            .subscribe(
            conversation => this.conversation = conversation);
    }

    onClick() {
        this.service.showConversation(this.conversation);
    }
}
