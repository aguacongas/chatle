import { autoinject } from 'aurelia-framework';
import { Router, Redirect, NavigationInstruction, RouterConfiguration, Next, RouteConfig } from 'aurelia-router';

import { Settings } from './config/settings';

interface CustomRouteConfig extends RouteConfig {
  anomymous: boolean;
}

@autoinject
export class App {
  router: Router;

  constructor(private settings: Settings) { 
    this.loadUser();
  }

  configureRouter(config: RouterConfiguration, router: Router){
    config.title = 'Chatle';
    config.addPipelineStep('authorize', AuthorizeStep);
    config.map([
      { route: ['', 'home'], name: 'home',    moduleId: 'pages/home',    title: 'Home' },
      { route: 'account',    name: 'account', moduleId: 'pages/account', title:'Account' },
      { route: 'login',      name: 'login',   moduleId: 'pages/login',   title:'Login', anomymous: true }
    ]);

    this.router = router;
  }

  private loadUser() {
    this.settings.userName = sessionStorage.getItem('userName');
  }
}

@autoinject
class AuthorizeStep {

  constructor(private settings: Settings) { }

  run(navigationInstruction: NavigationInstruction, next: Next): Promise<any> {
    if (navigationInstruction.getAllInstructions().some(i => {
        let route = i.config as CustomRouteConfig;
        return !route.anomymous
      })) 
    {
        var isLoggedIn = this.settings.userName;
        if (!isLoggedIn) {
          return next.cancel(new Redirect('login'));
        }
    }

    return next();
  }
}