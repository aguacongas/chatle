import { Attendee } from './attendee';
import { Message } from './message';
import { Settings } from './settings';

export class Conversation {
    id: string;
    title: string;
    attendees: Attendee[];
    messages: Message[]
}