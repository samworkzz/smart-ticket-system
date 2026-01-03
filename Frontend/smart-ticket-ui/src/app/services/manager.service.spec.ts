import { TestBed } from '@angular/core/testing';

import { ManagerServiceTs } from './manager.service.js';

describe('ManagerServiceTs', () => {
  let service: ManagerServiceTs;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ManagerServiceTs);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
