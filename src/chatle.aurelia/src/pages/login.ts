import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

import { ChatService, ConnectionState } from '../services/chat.service';

@autoinject
export class Login {
    errorMessage: string;
    constructor(private service: ChatService, private router: Router) { }

    login(userName: string) {
        this.service.login(userName, null)
            .then(() => {
                this.router.navigateToRoute('home');
            })
            .catch(error => this.errorMessage = error);
    }
}