import { TestBed, inject } from '@angular/core/testing';

import { SignalrService, HUB_CONNECTION_FACTORY } from './signalr.service';

describe('SignalrService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        SignalrService,
        { provide: HUB_CONNECTION_FACTORY, useValue: undefined },
      ]
    });
  });

  it('should be created', inject([SignalrService], (service: SignalrService) => {
    expect(service).toBeTruthy();
  }));
});
