import { HttpClient } from 'aurelia-http-client';
import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

import { Settings } from '../config/settings';

@autoinject
export class Login {
    userName: string;
    errorMessage: string;

    constructor(private settings: Settings, private http: HttpClient, private router: Router) {
        http.configure(
            builder => builder
                .withBaseUrl(settings.apiBaseUrl)
                .withCredentials(true));
     }

    login() {
        this.http.get('xhrf') 
            .then(response => 
                this.http
                    .configure(
                        builder => builder.withHeader("X-XSRF-TOKEN", response.response)
                    )
                    .post(this.settings.loginAPI, JSON.stringify(this.userName))
                    .then(response => { 
                        this.settings.userName = this.userName;
                        sessionStorage.setItem('userName', this.userName);
                        this.router.navigateToRoute('home');
                    })
                    .catch(error => this.errorMessage = error))
            .catch(error => this.errorMessage = error);
    }
}