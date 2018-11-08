import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UwimportComponent } from './uwimport.component';

describe('UwimportComponent', () => {
  let component: UwimportComponent;
  let fixture: ComponentFixture<UwimportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UwimportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UwimportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
