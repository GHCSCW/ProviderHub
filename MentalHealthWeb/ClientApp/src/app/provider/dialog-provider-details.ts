import { Component, Inject, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MentalHealthService } from '../service/mental.health.service';
import { MatChipInputEvent } from '@angular/material';
import { ENTER, COMMA } from '@angular/cdk/keycodes';
import { FormControl } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { startWith } from 'rxjs/operators/startWith';
import { map } from 'rxjs/operators/map';
import { Gender } from '../service/enum-service'
import 'rxjs/Rx'; 

export class Language {
  constructor(public name: string) { }
}


@Component({
  selector: 'dialog-provider-details',
  styleUrls: ['./dialog-provider-details-dialog.css'],
  template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i></a>'

})
export class DialogProviderDetails {

  @Input() provider: any[];
  @Input() originalProvider: any[];

  constructor(public dialog: MatDialog) {}

  openDialog(): void {
    let dialogRef = this.dialog.open(DialogProviderDetailsDialog, {
      width: '500px',
      height: '600px',
      data: { provider: this.provider, originalProvider: this.originalProvider }
    });
    dialogRef.afterClosed().subscribe(result => {
     // this.provider = this.originalProvider;
    });
  }

}

@Component({
  selector: 'dialog-provider-details-dialog',
  templateUrl: 'dialog-provider-details-dialog.html',
})
export class DialogProviderDetailsDialog {
  newitem: any;
  selectedValue: any = [];
  selectedItem: any = [];
  credentialList: any = [];
  language = new FormControl();
  toppings = new FormControl();
  credentials = new FormControl();
  toppingList = ['English', 'Spanish', 'Hmong'];
  newProvider: any = []; 
  visible: boolean = true;
  selectable: boolean = true;
  removable: boolean = true;
  addOnBlur: boolean = true;
  baseCredentials: any = [];


  // Enter, comma
  separatorKeysCodes = [ENTER, COMMA];

  fruits = [
    { name: 'Lemon' },
    { name: 'Lime' },
    { name: 'Apple' },
  ];




  add(event: MatChipInputEvent): void {
    let input = event.input;
    let value = event.value;

    // Add our fruit
    if ((value || '').trim()) {
      this.fruits.push({ name: value.trim() });
    }

    // Reset the input value
    if (input) {
      input.value = '';
    }
  }

  remove(fruit: any): void {
    let index = this.fruits.indexOf(fruit);

    if (index >= 0) {
      this.fruits.splice(index, 1);
    }
  }


  genderSelected = Gender[this.data.provider.gender];
  filteredOptions: Observable<Language[]>;
  errors = [];

  constructor(private mentalHealthService: MentalHealthService,
    public dialogRef: MatDialogRef<DialogProviderDetailsDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {



    this.mentalHealthService.getCredentialListByProvID(this.data.originalProvider.id).subscribe(credList =>
      this.selectedItem = credList
    );
  
    this.mentalHealthService.getCredentialList().subscribe(credList =>
      this.credentialList = credList
    );

  }


  onSubmit() {
    this.mentalHealthService.updateProvider(this.data.provider).subscribe(updatedProvider => {
      Object.assign(this.data.originalProvider, updatedProvider);
      
    });
    this.mentalHealthService.updateCredentials(this.selectedItem, this.data.originalProvider.id).subscribe(updatedCredentials => {
      //update credential object
      this.dialogRef.close();
    }


      )
 
  }

  onNoClick(): void {

    this.newitem = this.credentials.value;
    Object.assign(this.data.provider, this.data.originalProvider);
    this.dialogRef.close();
  }


  items = [
    {

      createdDate:"2018-03-29T16:16:03.03",
      description: "Advanced Practice Nurse Practitioner",
      id: 31,
      mappingID: 80,
      sequenceNumber:1,
      status:false,
      value:"APNP"
    },
    {
      createdDate: "2018-03-29T16:16:03.03",
      description: "Advanced HUH",
      id: 345,
      mappingID: 80,
      sequenceNumber: 1,
      status: false,
      value: "MD"
    },
    {
      createdDate: "2018-03-29T16:16:03.03",
      description: "Advanced Practice Nurse HUHHH",
      id: 1,
      mappingID: 344,
      sequenceNumber: 1,
      status: false,
      value: "AA"
    }
  ];
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

  //filter(name: string): Language[] {
  //  return this.options.filter(option =>
  //    option.name.toLowerCase().indexOf(name.toLowerCase()) === 0);
  //}

  //displayFn(user?: Language): string | undefined {
  //  return user ? user.name : undefined;
  //}
