import { Component, Inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl } from "@angular/forms";
import { FormBuilder, Validators, FormsModule, NgForm, FormGroup } from '@angular/forms';
import 'rxjs/Rx';

import { MentalHealthService } from '../services/mental.health.service';
import { Facility } from '../models/facility';
import { Address } from '../models/Address';



@Component({
  selector: 'create-facility',
  styleUrls: ['./create-facility.css'],
  templateUrl: './create-facility.html'
  //template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i></a>'

})

export class CreateFacility implements OnInit {
  address: any = [];
  //facility: any = [];

  public facility = new Facility();
  createFacilityForm: FormGroup;
  FacilityName: string = 'Alex';
  NPI: number = null;
  ExternalId: string = '';
  InternalNotes: string = '';


  constructor(private fb: FormBuilder, private mentalHealthService: MentalHealthService) {

    this.createFacilityForm = fb.group({
      'FacilityName': '',
      'NPI': [],
      'ExternalId': [],
      'InternalNotes':[]
    });
  }

  onNotify(form): void {
    //this.address.push(form);
    this.facility = this.createFacilityForm.value;
    this.facility.FacilityAddress = form;
    this.mentalHealthService.createFacility(this.facility);
    //facility = this.createFacilityForm.value;
    //facility.push(this.address);
  }
  ngOnInit() {

  }
  onFormSubmit(form) {
    this.mentalHealthService.createFacility(form);
    console.log(form);
  }
}
