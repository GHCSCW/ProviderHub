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
  language = new FormControl();
  toppings = new FormControl();
  toppingList = ['English', 'Spanish', 'Hmong'];
  newProvider: any = []; 
  visible: boolean = true;
  selectable: boolean = true;
  removable: boolean = true;
  addOnBlur: boolean = true;

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
    @Inject(MAT_DIALOG_DATA) public data: any) { }


  onSubmit() {
    this.mentalHealthService.updateProvider(this.data.provider).subscribe(updatedProvider => {
      Object.assign(this.data.originalProvider, updatedProvider);
      this.dialogRef.close();
    });
 
  }

  onNoClick(): void {

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
