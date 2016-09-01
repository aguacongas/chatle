import { Component, Input } from '@angular/core';

import { User } from '../shared/user';

@Component({
  selector: 'contact',
  templateUrl: '/app/contacts/contact.component.html'
})

export class ContactComponent {
    @Input()
    user: User;

    onClick() {

    }
}
