import { Component, Inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl } from "@angular/forms";
import { FormBuilder, Validators, FormsModule, NgForm, FormGroup } from '@angular/forms';
import 'rxjs/Rx';
import { Router } from "@angular/router";

import { NavbarService } from '../services/navbarservice';
import { MentalHealthService } from '../services/mental.health.service';
import { ToastrService } from 'ngx-toastr';
import { Facility } from '../models/facility';
import { Address } from '../models/Address';


@Component({
  selector: 'create-facility',
  styleUrls: ['./create-facility.css'],
  templateUrl: './create-facility.html'
  //template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i></a>'

})

export class CreateFacility implements OnInit {
  facilityID: number;
  address: any = [];
  //facility: any = [];

  public facility = new Facility();

  createFacilityForm: FormGroup;
  NPI: number = null;
  ExternalId: string = '';
  InternalNotes: string = '';


  constructor(private fb: FormBuilder, private mentalHealthService: MentalHealthService, private router: Router, public nav: NavbarService, private toastr: ToastrService) {

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
      this.toastr.error('Invalid Entry', 'Please Entery a Valid Facility Name');
    }
    else {
      this.facility = this.createFacilityForm.value;
      this.facility.FacilityAddress = form;
      this.mentalHealthService.createFacility(this.facility).subscribe(facilityID =>
        this.facilityID = facilityID
      )
      if (this.facilityID > 0) {
        this.facilityRoute(this.facilityID);
      }
    }


  }
 
  onFormSubmit(form) {
    this.mentalHealthService.createFacility(form);
    console.log(form);
  }
  facilityRoute(facilityID) {
    this.toastr.success('Create Success', 'The Facility ' + this.facility.FacilityName + ' was created');
    this.nav.addFacilityID(facilityID);
    this.router.navigate(["/facility/" + facilityID]);
  }
}
