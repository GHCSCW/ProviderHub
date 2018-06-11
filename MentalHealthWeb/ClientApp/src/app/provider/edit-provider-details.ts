import { Component, Inject, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import 'rxjs/Rx';
import { MatChipInputEvent } from '@angular/material';
import { ENTER, COMMA } from '@angular/cdk/keycodes';
import { FormControl } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { startWith } from 'rxjs/operators/startWith';
import { map } from 'rxjs/operators/map';

import { Gender } from '../services/enum.service'
import { MentalHealthService } from '../services/mental-health.service';
import { ArrayService } from '../services/array.service';
import * as moment from 'moment';
import { AuthenticationService } from '../services/authentication.service';


@Component({
  selector: 'edit-provider-details',
  styleUrls: ['./edit-provider-details-dialog.css'],
  template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i></a>'

})
export class DialogProviderDetails {

  @Input() provider: any[];

  constructor(public dialog: MatDialog) { }

  openDialog(): void {
    let dialogRef = this.dialog.open(DialogProviderDetailsDialog, {
      width: '500px',
      height: '800px',
      data: { provider: this.provider }
    });
    dialogRef.afterClosed().subscribe(result => {
    });
  }

}

@Component({
  selector: 'edit-provider-details-dialog',
  templateUrl: 'edit-provider-details-dialog.html',
})
export class DialogProviderDetailsDialog {
  provID: any;

  providerLanguages: any = [];
  providerCredentials: any = [];
  credentialList: any = [];
  languageList: any = [];
  toppings = new FormControl();
  credentials = new FormControl();
  newProvider: any = [];
  visible: boolean = true;
  selectable: boolean = true;
  removable: boolean = true;
  addOnBlur: boolean = true;
  baseCredentials: any = [];
  credentialDropdownSettings: {};
  languageDropdownSettings: {};
  originalProvider: any = []; 
  genderSelected = Gender[this.data.provider.gender];
  errors = [];


  constructor(private mentalHealthService: MentalHealthService,
    private arrayService: ArrayService,
    private authSvc: AuthenticationService,
    public dialogRef: MatDialogRef<DialogProviderDetailsDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    dialogRef.disableClose = true;
    this.originalProvider = JSON.parse(JSON.stringify(data.provider));


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

  onSubmit() {

    this.provID = this.data.provider.id;

    this.mentalHealthService.updateProvider(this.data.provider).subscribe(updatedProvider => {
      this.data.provider.lastUpdatedDate = moment().toDate();
      this.data.provider.lastUpdatedBy = this.authSvc.userName;
      Object.assign(this.data.provider, updatedProvider);

    });
    //this.mentalHealthService.updateCredentials(this.data.provider.credentialList, this.provID).subscribe(updatedCredentials => {
    //  //update credential object
    //}
    //)
    //this.mentalHealthService.updateLanguage(this.data.provider.languageList, this.provID)
    //  .subscribe(
    //  updatedLanguage => {
    //    this.dialogRef.close();
    //  }
    //  )


  }

  onNoClick(): void {

    Object.assign(this.data.provider, this.originalProvider);
    this.dialogRef.close();
  }

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

  selectedGender = this.data.provider.gender;



  // filter(name: string): Language[] {
  //  return this.options.filter(option =>
  //    option.name.toLowerCase().indexOf(name.toLowerCase()) === 0);
  //}

  //displayFn(user?: Language): string | undefined {
  //  return user ? user.name : undefined;
  //}



}



  //updateProvider(data) {
  //  this.mentalHealthService.updateProvider(data);
  //  return 
  //  //window.location.reload();
  //}
    //this.filteredOptions = this.language.valueChanges
    //  .pipe(
    //  startWith<string | Language>(''),
    //  map(value => typeof value === 'string' ? value : value.name),
    //  map(name => name ? this.filter(name) : this.options.slice())
    //  );

