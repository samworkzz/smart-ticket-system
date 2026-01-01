import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EnduserDashboardComponent } from './enduser-dashboard.component';

describe('EnduserDashboardComponent', () => {
  let component: EnduserDashboardComponent;
  let fixture: ComponentFixture<EnduserDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EnduserDashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EnduserDashboardComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
