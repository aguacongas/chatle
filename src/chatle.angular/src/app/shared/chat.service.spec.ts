import { Injector } from '@angular/core';
import { TestBed, inject } from '@angular/core/testing';
import { HttpModule, Http, XHRBackend } from '@angular/http';
import { MockBackend } from '@angular/http/testing';
import { SignalrModule, SignalrService } from './signalr-client';

import { ChatService, ConnectionState } from './chat.service';

import { Settings } from './settings';
import { User } from './user';
import { Message } from './message';
import { Conversation } from './conversation';

describe('ChatService', () => {
    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpModule, SignalrModule],
            providers: [
                { provide: XHRBackend, useClass: MockBackend },
                { provide: Settings, useClass: Settings }
            ]
        });
    });

    it('start should return connectionState observable',
        inject([Http, Settings, SignalrService],
            (http: Http, settings: Settings, signalrService: SignalrService) => {
        const service = new ChatService(http, settings, signalrService);
        let connectionState: ConnectionState;

        service.start(true)
            .subscribe((response: ConnectionState) => {
                connectionState = response;
            });

        expect(connectionState).toBe(ConnectionState.Connected);
    }));
});
