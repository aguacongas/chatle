import { HttpClient } from 'aurelia-http-client';
import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

import { ChatService } from '../services/chat.service';

@autoinject
export class Login {
    userName: string;
    errorMessage: string;

    constructor(private service: ChatService, private http: HttpClient, private router: Router) { }

    login() {
        this.service.login(this.userName)
            .then(() => {
                this.router.navigateToRoute('home');
            })
            .catch(error => this.errorMessage = error);
    }
}