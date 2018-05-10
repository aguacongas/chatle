import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { SignalrModule, HubSettings } from './shared/signalr-client';
import { LogLevel } from '@aspnet/signalr';

import { environment } from '../environments/environment';
import { AppComponent } from './app.component';
import { ContactComponent } from './contacts/contact.component';
import { ContactsComponent } from './contacts/contacts.component';
import { ConversationComponent } from './conversations/conversation.component';
import { ConversationPreviewComponent } from './conversations/conversationPreview.component';
import { ConversationsComponent } from './conversations/conversations.component';
import { Settings } from './shared/settings';
import { ChatService, ConnectionState } from './shared/chat.service';

@NgModule({
  imports: [BrowserModule, FormsModule, HttpClientModule, SignalrModule],
  declarations: [
    AppComponent,
    ContactComponent,
    ContactsComponent,
    ConversationPreviewComponent,
    ConversationComponent,
    ConversationsComponent
  ],
  bootstrap: [AppComponent],
  providers: [
    ChatService,
    { provide: Settings, useFactory: getSettings },
    { provide: HubSettings, useFactory: getHubSettings, deps: [Settings] },
    {
      provide: APP_INITIALIZER,
      useFactory: boot,
      deps: [ChatService],
      multi: true
    }
  ]
})
export class AppModule {}

export function getSettings(): Settings {
  return window['chatleSetting'] as Settings;
}

export function getHubSettings(settings: Settings): HubSettings {
  let hubSettings = settings.hubSettings;
  if (!hubSettings) {
    hubSettings = {
      url: environment.production ? '/chat' : 'http://localhost:5000/chat',
      logLevel: settings.debug ? LogLevel.Trace : LogLevel.Warning
    };
  }
  return hubSettings;
}

export function boot(service: ChatService): Function {
  return () => {
    return new Promise((resolve, reject) => {
      service.start(true).subscribe(
        () => resolve(),
        error => {
          location.assign('Account?reason=disconnected');
          reject();
        }
      );
    });
  };
}
