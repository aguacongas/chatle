import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { SignalrModule, HubSettings } from './shared/signalr-client';

import { environment } from '../environments/environment';
import { AppComponent } from './app.component';
import { ContactComponent } from './contacts/contact.component';
import { ContactsComponent } from './contacts/contacts.component';
import { ConversationComponent } from './conversations/conversation.component';
import { ConversationPreviewComponent } from './conversations/conversationPreview.component';
import { ConversationsComponent } from './conversations/conversations.component';
import { Settings } from './shared/settings';
import { ChatService, ConnectionState } from './shared/chat.service';

const hubSettings = {
  url: environment.production ?  '/chat' : 'http://localhost:5000/chat'
};

@NgModule({
  imports: [ BrowserModule, FormsModule, HttpClientModule, SignalrModule ],
  declarations: [
    AppComponent,
    ContactComponent,
    ContactsComponent,
    ConversationPreviewComponent,
    ConversationComponent,
    ConversationsComponent ],
  bootstrap: [AppComponent],
  providers: [
    ChatService,
    { provide: Settings, useFactory: getSettings },
    { provide: HubSettings, useValue: hubSettings },
    {
        provide: APP_INITIALIZER,
        useFactory: boot,
        deps: [ChatService],
        multi: true
    },
  ]
})
export class AppModule { }

export function getSettings(): Settings {
  return window['chatleSetting'] as Settings;
}

export function boot(service: ChatService): Function {
    return () => {
        return new Promise((resolve, reject) => {
            service.start(true).subscribe(
                () => resolve(),
                error => {
                    location.assign('/Account?reason=disconnected');
                    reject();
                });
        });
    };
}
