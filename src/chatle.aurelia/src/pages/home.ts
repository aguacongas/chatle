import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

import { ChatService, ConnectionState } from '../services/chat.service';

@autoinject
export class Home {

    constructor(public service: ChatService, private router: Router) { }

    attached() {
        if (this.service.currentState !== ConnectionState.Connected) {
            this.service.start();
        }        
    }

    logoff() {
        this.service.logoff();
        this.router.navigateToRoute('login');
    }

    manage() {
        this.router.navigateToRoute('account');
    }
}