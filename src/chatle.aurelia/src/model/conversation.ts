import { Attendee } from './attendee';
import { Message } from './message';

export class Conversation {
    id: string;
    title: string;
    attendees: Attendee[];
    messages: Message[]
}