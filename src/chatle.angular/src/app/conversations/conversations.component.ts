import { Component, OnInit } from '@angular/core';

import { ChatService, ConnectionState } from '../shared/chat.service';
import { Settings } from '../shared/settings';
import { Conversation } from '../shared/Conversation';

@Component({
    selector: 'chatle-conversations',
    templateUrl: './conversations.component.html'
})
export class ConversationsComponent implements OnInit {
    conversations: Conversation[] = [];
    error: any;

    constructor(private service: ChatService, private settings: Settings) { }

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
                conversation => {
                    this.conversations.unshift(conversation);
                },
                error => this.error = error);

        this.service.userDiscconnected
            .subscribe(
                user => {
                    if (user.isRemoved) {
                        const index = this.conversations.findIndex(c => c.attendees.length < 3
                            && c.attendees.some(a => a.userId === user.id));
                        if (index > -1) {
                            this.conversations.splice(index, 1);
                        }
                    }
                });

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
