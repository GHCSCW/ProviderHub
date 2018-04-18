import { Component, Inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl } from "@angular/forms";
import { FormBuilder, Validators, FormsModule, NgForm, FormGroup } from '@angular/forms';
import 'rxjs/Rx';

import { MentalHealthService } from '../services/mental.health.service';



@Component({
  selector: 'create-facility',
  styleUrls: ['./create-facility.css'],
  templateUrl: './create-facility.html'
  //template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i></a>'

})

export class CreateFacility implements OnInit {


  createFacilityForm: FormGroup;
  FacilityName: string = '';
  NPI: number = null;
  ExternalId: string = '';
  ExternalNotes: string = '';


  constructor(private fb: FormBuilder, private mentalHealthService: MentalHealthService) {

    this.createFacilityForm = fb.group({
      'FacilityName': [],
      'NPI': [],
      'ExternalId': [],
      'InternalNotes':[]
    });
  }


  ngOnInit() {

  }
  onFormSubmit(form: NgForm) {
    this.mentalHealthService.createFacility(form);
    console.log(form);
  }
}
