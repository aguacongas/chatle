import { Injector } from '@angular/core';
import { TestBed, inject } from '@angular/core/testing';
import { HttpModule, Http, XHRBackend } from '@angular/http';
import { MockBackend } from '@angular/http/testing';

import { AppComponent } from './app.component';
import { ChatService } from './shared/chat.service';
import { Settings } from './shared/settings';

describe('AppComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpModule],
      providers: [
        { provide: XHRBackend, useClass: MockBackend },
        { provide: Settings, useClass: Settings }
      ]
    });
  });

  it(
    'app component should exist',
    inject([ChatService], (service: ChatService) => {
      const component = new AppComponent(service);
      expect(component).toBeDefined();
    })
  );
});
