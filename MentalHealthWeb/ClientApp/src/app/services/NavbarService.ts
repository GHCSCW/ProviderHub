import { Injectable } from '@angular/core';

@Injectable()
export class NavbarService {
  facilityID: number;
  vendorID: number;
  facilityRelProviderID: number;
  providerID: number;

  visible: boolean;

  constructor()
  {
    this.visible = false;
  
  }
  addFacilityRelationshipProviderID(provRelationship)
  {
    this.facilityRelProviderID = provRelationship.relationshipID;
    this.facilityID = provRelationship.facility.id;
  }

  addFacilityID(id) {
    this.facilityID = id;
  }
  addVendorID(id) {
    this.vendorID = id;
  }

  addProviderID(id) {
    this.providerID = id;
  }

  resetIDs() {
    this.vendorID = undefined;
    this.providerID = undefined;
    this.facilityRelProviderID = undefined;
    this.facilityID = undefined;
  }

  hide() { this.visible = false; }

  show() { this.visible = true; }

  toggle() { this.visible = !this.visible; }

}
