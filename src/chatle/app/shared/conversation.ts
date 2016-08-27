import { User } from './user';
import { Message } from './message';
import { Settings } from './settings';

export class Conversation {
    id: number;
    attendees: User[];
    messages: Message[]
}