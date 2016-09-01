import { Component, Input } from '@angular/core';

import { Conversation } from '../shared/conversation';

@Component({
    selector: 'conversation',
    templateUrl: '/app/conversations/conversation.component.html'
})

export class ConversationComponent {
    @Input()
    conversation: Conversation;

    onClick() {

    }
}
