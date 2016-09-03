import { Component, OnInit } from '@angular/core';

import { ChatService, ConnectionState } from './shared/chat.service'
import { Settings } from './shared/settings'

@Component({
  selector: 'chatle',
  templateUrl: '/app/app.component.html',
  providers: [ChatService]
})
export class AppComponent implements OnInit{

  constructor(private service: ChatService) { }

  ngOnInit() {
    this.service.start(true);
  }
}
