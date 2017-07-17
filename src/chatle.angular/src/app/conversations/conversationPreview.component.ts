import { Component, Input, OnInit } from '@angular/core';

import { Conversation } from '../shared/conversation';
import { ChatService } from '../shared/chat.service';

@Component({
    selector: 'chatle-conversation-preview',
    templateUrl: './conversationPreview.component.html'
})
export class ConversationPreviewComponent implements OnInit {
    @Input()
    conversation: Conversation;

    constructor(private service: ChatService) { }

    ngOnInit() {
        this.service.messageReceived
            .subscribe(
                message => {
                        if (message.conversationId === this.conversation.id) {
                            this.conversation.messages.unshift(message);
                        }
                    });
    }

    onClick() {
        this.service.showConversation(this.conversation);
    }

    getLastMessage(): string {
        const messages = this.conversation.messages;
        if (messages && messages[0]) {
            return messages[0].text;
        }
    }
}
