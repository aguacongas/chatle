import { Message } from '../model/message';

export class MessageReceived {
    constructor(public message: Message) { }
}