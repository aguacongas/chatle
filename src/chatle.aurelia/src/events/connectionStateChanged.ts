import { ConnectionState } from '../services/chat.service';

export class ConnectionStateChanged {
    constructor(state: ConnectionState) { }
}