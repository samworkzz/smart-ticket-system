import { TestBed } from '@angular/core/testing';

import { TicketServiceTs } from './ticket.service.ts';

describe('TicketServiceTs', () => {
  let service: TicketServiceTs;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TicketServiceTs);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
