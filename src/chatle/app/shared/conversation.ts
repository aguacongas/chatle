import { User } from './user';
import { Message } from './message';
import { Settings } from './settings';

export class Conversation {
    id: number;
    attendees: User[];
    messages: Message[]

    constructor(private conv: any) {
        this.id = conv.id;
        this.attendees = conv.attendees;
        this.messages = conv.messages;
    }
}