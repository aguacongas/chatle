import { Injectable } from '@angular/core';
import { HubSettings } from '../shared/signalr-client';

@Injectable()
export class Settings {
    userName = 'test';
    userAPI =  '/api/users';
    convAPI = '/api/chat/conv';
    chatAPI =  '/api/chat';
    hubSettings: HubSettings;
    debug = false;
}
