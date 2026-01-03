import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignedTickets } from './assigned-tickets';

describe('AssignedTickets', () => {
  let component: AssignedTickets;
  let fixture: ComponentFixture<AssignedTickets>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssignedTickets]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssignedTickets);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
