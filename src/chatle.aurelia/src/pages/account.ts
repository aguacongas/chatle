import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

import { ChatService, ConnectionState } from '../services/chat.service';
import { ChangePassword } from '../model/changePassword';

@autoinject
export class Account {
    userName: string;
    model = new ChangePassword();
    errorMessage: string;

    constructor(private service: ChatService, private router: Router) { 
        this.userName = service.userName;
    }

    changePassword() {
        this.service.changePassword(this.model)
            .then(() => {
                this.router.navigateToRoute('home');
            })
            .catch(error => this.errorMessage = error);
    }
}