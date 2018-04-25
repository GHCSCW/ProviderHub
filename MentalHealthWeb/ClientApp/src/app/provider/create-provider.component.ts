import { Component, Inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl } from "@angular/forms";
import { FormBuilder, Validators, FormsModule, NgForm, FormGroup } from '@angular/forms';

import { MentalHealthService } from '../services/mental.health.service';

import 'rxjs/Rx';
export class Language {
  constructor(public name: string) { }
}


@Component({
  selector: 'create-provider',
  styleUrls: ['./create-provider.component.css'],
  templateUrl: './create-provider.component.html'

})

export class CreateProvider implements OnInit {

  credentialList: any = [];
  languageList: any = [];
  credentialDropdownSettings = {};
  languageDropdownSettings = {}; 
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
  CredentialList: any = [];
  LanguageList: any = [];

  constructor(private fb: FormBuilder, private mentalHealthService: MentalHealthService) {

    this.createProviderForm = fb.group({
      'FirstName': [],
      'MiddleName': [],
      'LastName': [],
      'FullName': [],
      'CSP_Indicator': false,
      'DateOfBirth':[],
      'CredentialList': [],
      'LanguageList':[],
      //'DelegateName': [],
      //'DelegateIndicator': [],
      'EpicProviderID': [],
      'NPI': [],
      //'TaxonomyCode': [],
      //'UniquePhyscianId':[],
      //'ExternalProviderName': [],
      'Gender': [],
      //'HospitalAffiliation':[],
     // 'InternalNotes': []
     
      //Specialties List
      //'LastUpdateBy': [],
      //'LastUpdateDate': [],
      'MedicareIndicator': false,
      'MedicaidIndicator': false,
      'MedicareEffectiveDate': [],
      'MedicareTerminationDate': []
      

    });
  }


  ngOnInit() {

    this.credentialDropdownSettings = {
      singleSelection: false,
      text: "Select Credential(s)",
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      maxHeight: 500,
      labelKey: 'value'
    };

    this.languageDropdownSettings = {
      singleSelection: false,
      text: "Select Language(s)",
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      maxHeight: 500,
      labelKey: 'name'
    };
    this.mentalHealthService.getLanguages().subscribe(result =>
      this.languageList = result
    );

    this.mentalHealthService.getCredentialList().subscribe(credList =>
      this.credentialList = credList
    );

  }
  onItemSelect(item: any) {
    console.log(item);

  }
  OnItemDeSelect(item: any) {
    console.log(item);

  }
  onSelectAll(items: any) {
    console.log(items);
  }
  onDeSelectAll(items: any) {
    console.log(items);
  }

  onFormSubmit(form: NgForm) {
    this.mentalHealthService.createProvider(form);
    console.log(form);
  }  
}
