import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from "@angular/router";
import { Location } from '@angular/common';

import { NavbarService } from '../services/navbarservice';
import { MentalHealthService } from '../services/mental.health.service';

@Component({
  selector: 'facility-provider-relationship',
  templateUrl: './facility-provider-relationship.component.html',
  styleUrls: ['./facility-provider-relationship.component.css']
})
export class FacilityProviderRelationshipComponent  implements OnInit {
  facilityProviderRelationship: any = [];
  provider: any = [];
  facility: any = [];
  facilityAddress: any = [];

  constructor(private mentalHealthService: MentalHealthService) {
   
  }
  ngOnInit() {

    this.mentalHealthService.getFacilityProviderRelationshipData().map(results => {
      this.facilityProviderRelationship = results;
      if (results.provider == undefined) {
        this.provider = results;
      }
      else {
        this.provider = results.provider;
        this.facility = results.facility;
        this.facilityAddress = results.facility.facilityAddress;
        this.facilityProviderRelationship = results;
      }

    });
  }

}
