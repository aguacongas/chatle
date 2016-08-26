export class Message {
    conversationId: number;
    from: number;
    text: string;

    constructor(data: any) {
        this.conversationId = data.conversationId;
        this.from = data.from;
        this.text = data.text;
    }
}