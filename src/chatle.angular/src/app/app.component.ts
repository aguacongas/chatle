import { Component } from '@angular/core';
import { ChatService, ConnectionState } from './shared/chat.service';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'chatle',
  templateUrl: './app.component.html'
})
export class AppComponent {
  constructor(private service: ChatService) {
    this.service.connectionState.subscribe(state => {
      if (state === ConnectionState.Error && location) {
        location.href = 'Account?reason="SignalR connection state on error"';
      }
    });
  }
}
