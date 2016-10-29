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
    TestBed.resetTestEnvironment();

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [
                { provide: XHRBackend, useClass: MockBackend },
                { provide: Settings, useClass: Settings }
            ]
        });
    });

    it("start should return connectionState observable", inject([Http, Settings], (http: Http, settings: Settings) => {
        let service = new ChatService(http, settings);
        let connectionState: ConnectionState;

        service.start(true)
            .subscribe((response: ConnectionState) => {
                connectionState = response;
            });          
        
        expect(connectionState).toBe(ConnectionState.Connected);
    }));
});