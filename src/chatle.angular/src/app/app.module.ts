import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { SignalrModule, HubSettings } from './shared/signalr-client';

import { environment } from '../environments/environment';
import { AppComponent } from './app.component';
import { ContactComponent } from './contacts/contact.component';
import { ContactsComponent } from './contacts/contacts.component';
import { ConversationComponent } from './conversations/conversation.component';
import { ConversationPreviewComponent } from './conversations/conversationPreview.component';
import { ConversationsComponent } from './conversations/conversations.component';
import { Settings } from './shared/settings';

const hubSettings = {
  url: environment.production ?  '/chat' : 'http://localhost:5000/chat'
};

@NgModule({
  imports: [ BrowserModule, FormsModule, HttpModule, SignalrModule ],
  declarations: [
    AppComponent,
    ContactComponent,
    ContactsComponent,
    ConversationPreviewComponent,
    ConversationComponent,
    ConversationsComponent ],
  bootstrap: [AppComponent],
  providers: [
    { provide: Settings, useFactory: getSettings },
    { provide: HubSettings, useValue: hubSettings }
  ]
})
export class AppModule { }

export function getSettings(): Settings {
  return window['chatleSetting'] as Settings;
}
