import { Component, Inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl } from "@angular/forms";
import { FormBuilder, Validators, FormsModule, NgForm, FormGroup } from '@angular/forms';
import 'rxjs/Rx';
import { Router } from "@angular/router";

import { NavbarService } from '../services/navbar.service';
import { MentalHealthService } from '../services/mental-health.service';
import { ToastrService } from 'ngx-toastr';
import { Facility } from '../models/facility';
import { Address } from '../models/Address';

@Component({
  selector: 'create-facility',
  styleUrls: ['./create-facility.component.css'],
  templateUrl: './create-facility.component.html'
})

export class CreateFacility implements OnInit {

  public facility = new Facility();
  address: any = [];
  createFacilityForm: FormGroup;
  NPI: number = null;
  ExternalId: string = '';
  InternalNotes: string = '';

  constructor(
    private fb: FormBuilder,
    private mentalHealthService: MentalHealthService,
    private router: Router, public nav: NavbarService,
    private toastr: ToastrService
  ) {

    this.createFacilityForm = fb.group({
      'FacilityName': '',
      'NPI': [],
      'ExternalId': [],
      'InternalNotes': []
    });
  }

  ngOnInit() {
  }

  onNotify(form): void {
    if (this.createFacilityForm.value.FacilityName === "") {
      this.toastr.error('Invalid Entry', 'Please Enter a Valid Facility Name');
    }
    else {
      this.facility = this.createFacilityForm.value;
      this.facility.FacilityAddress = form;
      this.mentalHealthService.createFacility(this.facility).subscribe(facilityID =>
        this.facilityRoute(facilityID)
      )
    }
  }
  facilityRoute(facilityID) {
    if (facilityID > 0) {
      this.toastr.success('Create Success', 'The Facility ' + this.facility.FacilityName + ' was created');
      this.nav.addFacilityID(facilityID);
      this.router.navigate(["/facility/" + facilityID]);
    }
  }
}
