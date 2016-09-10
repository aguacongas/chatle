import {autoinject} from 'aurelia-framework';

import { ChatService } from './services/chat.service';

@autoinject
export class App {
  message = 'Hello World!';

  constructor(private service: ChatService) { 
    service.start();
  }

}
