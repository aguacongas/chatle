import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent }  from './app.component';
import { ContactComponent }  from './contacts/contact.component';

@NgModule({
  imports:      [ BrowserModule ],
  declarations: [ AppComponent,ContactComponent ],
  bootstrap:    [ AppComponent ]
})
export class AppModule { }
