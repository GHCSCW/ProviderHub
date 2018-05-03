import { Component, Inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl } from "@angular/forms";
import { FormBuilder, Validators, FormsModule, NgForm, FormGroup } from '@angular/forms';
import { MentalHealthService } from '../services/mental.health.service';

import 'rxjs/Rx';
import { Router } from '@angular/router';
import { NavbarService } from '../services/navbarservice';
export class Language {
  constructor(public name: string) { }
}


@Component({
  selector: 'create-provider',
  styleUrls: ['./create-provider.component.css'],
  templateUrl: './create-provider.component.html'

})

export class CreateProvider implements OnInit {
  providerID: any;

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

  constructor(private fb: FormBuilder, private mentalHealthService: MentalHealthService, private router: Router, public nav: NavbarService) {

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

  providerRoute(provider) {

    this.mentalHealthService.insertProviderData(provider);
    this.nav.addProviderID(provider.id);
    this.router.navigate(["/provider/" + this.providerID]);

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
    this.mentalHealthService.updateProvider(form).subscribe(provider =>
      this.providerID = provider
      )
    console.log(form);
  }


}
