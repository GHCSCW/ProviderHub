import { Component, Inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MentalHealthService } from '../service/mental.health.service';
import { FormControl } from "@angular/forms";
import { FormBuilder, Validators, FormsModule, NgForm, FormGroup } from '@angular/forms';

import 'rxjs/Rx';
export class Language {
  constructor(public name: string) { }
}


@Component({
  selector: 'create-provider',
  styleUrls: ['./create-provider.component.css'],
  templateUrl: './create-provider.component.html'
  //template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i></a>'

})

export class CreateProvider implements OnInit {

  genders = [
    {
      value: 1,
      label: 'Female'
    },
    {
      value: 2,
      label: 'Male'
    },
    {
      value: 3,
      label: 'Unknown'
    },
  ];


  createProviderForm: FormGroup;
  FirstName: string = '';
  MiddleName: string = '';
  LastName: string = '';
  FullName: string = '';
  CSP_Indicator: boolean = false;
  DateOfBirth: Date;
  //DelegateName: string = '';
  //DelegateIndicator: boolean = false;
  EpicProviderID:string = '';
  NPI: number = null;
 // TaxonomyCode: string = '';
  //UniquePhysicianId: string = '';
 // ExternalProviderName: string = '';
  Gender: string = '';
 // HopsitalAffiliation: string = '';
  //InternalNotes: string = '';
  //LastUpdatedBy: string = '';
  //LasteUpdateDate: Date;
  //DateOfBirth: Date;
  MedicaidIndicator: boolean = false;
  MedicareEffectiveDate: Date;
  MedicareTerminationDate: Date;

  constructor(private fb: FormBuilder, private mentalHealthService: MentalHealthService) {

    this.createProviderForm = fb.group({
      'FirstName': [],
      'MiddleName': [],
      'LastName': [],
      'FullName': [],
      'CSP_Indicator': [],
      'DateOfBirth':[],
      //'credentials':[]
      //'DelegateName': [],
      //'DelegateIndicator': [],
      'EpicProviderID': [],
      'NPI': [],
      //'TaxonomyCode': [],
      //'UniquePhyscianId':[],
      //'ExternalProviderName': [],
      'Gender': [],
      //'HospitalAffiliation':[],
      'InternalNotes': [],
      //LanguageList
      //Specialties List
      //'LastUpdateBy': [],
      //'LastUpdateDate': [],
      'MedicaidIndicator': [],
      'MedicareEffectiveDate': [],
      'MedicareTerminationDate': []
      

    });
  }


  ngOnInit() {

  }
  onFormSubmit(form: NgForm) {
    this.mentalHealthService.createProvider(form);
    console.log(form);
  }  
}
