import { Component, OnInit } from '@angular/core';

import { ChatService, ConnectionState } from './shared/chat.service';
import { Settings } from './shared/settings';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'chatle',
  templateUrl: './app.component.html',
  providers: [ChatService]
})
export class AppComponent implements OnInit {

  constructor(private service: ChatService) { }

  ngOnInit() {
    this.service.start(true).subscribe(
      null,
      error => location.assign('/Account?reason=disconnected'));
  }
}
