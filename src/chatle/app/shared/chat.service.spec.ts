import { Injector } from '@angular/core';
import { TestBed, inject } from '@angular/core/testing';
import { HttpModule, Http, XHRBackend } from '@angular/http';
import { MockBackend } from '@angular/http/testing';

import { ChatService, ConnectionState } from "./chat.service";

import { Settings } from './settings';
import { User } from './user';
import { Message } from './message';
import { Conversation } from './conversation';

describe('ChatService', () => {

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [
                { provide: XHRBackend, useClass: MockBackend }
            ]
        });
    });

    it("start should return connectionState observable", inject([Settings, Http], (settings: Settings, http: Http) => {
        let service = new ChatService(settings, http);
        let connectionState: ConnectionState;

        service.start(true)
            .subscribe((response: ConnectionState) => {
                connectionState = response;
            });          
        
        expect(connectionState).toBe(ConnectionState.Connected);
    }));
});