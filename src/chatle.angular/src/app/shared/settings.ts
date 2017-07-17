import { Injectable } from '@angular/core';

@Injectable()
export class Settings {
    userName = 'test';
    userAPI =  '/api/users';
    convAPI = '/api/chat/conv';
    chatAPI =  '/api/chat';
}
