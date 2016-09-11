import { autoinject } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';

import { Settings } from './config/settings';

@autoinject
export class App {
  router: Router;

  constructor(private settings: Settings) { 
    this.loadUser();
  }

  configureRouter(config: RouterConfiguration, router: Router){
    config.title = 'Chatle';
    config.map([
      { route: ['', 'home'], moduleId: 'pages/home',    title: 'Home'},
      { route: 'account',    moduleId: 'pages/account', title:'Account' },
      { route: 'login',      moduleId: 'pages/login',   title:'Login' }
    ]);

    this.router = router;
  }

  private loadUser() {
    this.settings.userName = sessionStorage.getItem('userName');
  }
}
