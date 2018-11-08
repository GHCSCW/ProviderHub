import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacilitysearchComponent } from './facilitysearch.component';

describe('FacilitysearchComponent', () => {
  let component: FacilitysearchComponent;
  let fixture: ComponentFixture<FacilitysearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacilitysearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacilitysearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
