import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacilityProviderRelationshipComponent } from './facility-provider-relationship.component';

describe('FacilityProviderRelationshipComponent', () => {
  let component: FacilityProviderRelationshipComponent;
  let fixture: ComponentFixture<FacilityProviderRelationshipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacilityProviderRelationshipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacilityProviderRelationshipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
