import { EventAggregator } from 'aurelia-event-aggregator';
import { HttpClient } from 'aurelia-http-client';

import { ChatService } from '../../../src/services/chat.service';
import { Settings } from '../../../src/config/settings';
import { ConnectionState } from '../../../src/services/chat.service';

describe('chat service test', () => {
    let service = new ChatService(new Settings(), new EventAggregator(), new HttpClient());

    it('chat service default state is disconnected', () => {
        expect(service.currentState).toBe(ConnectionState.Disconnected);
    });
    
});