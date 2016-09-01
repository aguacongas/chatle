import { NgModule, provide }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule }    from '@angular/forms';
import { HttpModule }     from '@angular/http';

import { AppComponent }  from './app.component';
import { ContactComponent }  from './contacts/contact.component';
import { ContactsComponent }  from './contacts/contacts.component';
import { ConversationComponent }  from './conversations/conversation.component';
import { ConversationsComponent }  from './conversations/conversations.component';
import { Settings } from './shared/settings';

@NgModule({
  imports:      [ BrowserModule, FormsModule, HttpModule ],
  declarations: [AppComponent, ContactComponent, ContactsComponent, ConversationComponent, ConversationsComponent ],
  bootstrap: [AppComponent],
  providers: [{ provide: Settings, useFactory: () => window['chatleSetting'] }]
})
export class AppModule { }
