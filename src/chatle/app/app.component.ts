import { Component, OnInit } from '@angular/core';

import { ChatService, ConnectionState} from './shared/chat.service'

@Component({
  selector: 'chatle',
  template: `<h1>chatle main Component</h1>
    <span>connection state: {{connectionState}}</span>
    <contacts></contacts>
  `,
  providers: [ChatService]
})
export class AppComponent implements OnInit{

  connectionState: ConnectionState;

  constructor(private service: ChatService) { 
    this.connectionState = service.currentState;
  }

  ngOnInit() {
    this.service.start(true)
      .toPromise()
      .then(connectionState => this.connectionState = connectionState);
  }
}
