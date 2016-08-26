import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule }    from '@angular/forms';
import { HttpModule }     from '@angular/http';

import { AppComponent }  from './app.component';
import { ContactComponent }  from './contacts/contact.component';
import { ContactsComponent }  from './contacts/contacts.component';

@NgModule({
  imports:      [ BrowserModule, FormsModule, HttpModule ],
  declarations: [ AppComponent, ContactComponent, ContactsComponent ],
  bootstrap:    [ AppComponent ]
})
export class AppModule { }
