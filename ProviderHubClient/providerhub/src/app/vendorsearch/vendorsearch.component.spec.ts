import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorsearchComponent } from './vendorsearch.component';

describe('VendorsearchComponent', () => {
  let component: VendorsearchComponent;
  let fixture: ComponentFixture<VendorsearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VendorsearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VendorsearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
