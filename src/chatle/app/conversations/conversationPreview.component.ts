import { Component, Input } from '@angular/core';

import { Conversation } from '../shared/conversation';

@Component({
    selector: 'conversation-preview',
    templateUrl: '/app/conversations/conversationPreview.component.html'
})

export class ConversationComponent {
    @Input()
    conversation: Conversation;

    onClick() {

    }
}
