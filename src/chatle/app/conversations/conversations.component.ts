import { Component, OnInit } from '@angular/core';

import { ChatService, ConnectionState } from '../shared/chat.service'
import { Conversation } from '../shared/Conversation'

@Component({
    selector: 'conversations',
    templateUrl: '/app/conversations/conversations.component.html'
})
export class ConversationsComponent implements OnInit {
    conversations: Conversation[];
    error: any;

    constructor(private service: ChatService) { }

    ngOnInit() {
        this.service.connectionState
            .subscribe(
                connectionState => {
                    if (connectionState === ConnectionState.Connected) {
                        this.getConversations();
                    }
                },
                error => this.error = error);

        this.service.joinConversation
            .subscribe(
                conversation => { this.conversations.unshift(conversation) },
                error => this.error = error);

        if (this.service.currentState === ConnectionState.Connected) {
            this.getConversations();
        }
    }

    private getConversations() {
        this.service.getConversations()
            .subscribe(
            conversations => this.conversations = conversations,
            error => this.error = error);
    }
}
