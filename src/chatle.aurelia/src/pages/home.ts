import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

import { Settings } from '../config/settings';

@autoinject
export class Home {

    constructor(public settings: Settings, private router: Router) { }

    logoff() {
        delete this.settings.userName;
        sessionStorage.removeItem('userName');
        this.router.navigateToRoute('login');
    }

    manage() {
        this.router.navigateToRoute('account');
    }
}