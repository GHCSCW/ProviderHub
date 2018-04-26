import { Component, Inject, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MentalHealthService } from '../services/mental.health.service';
import { FormControl } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { startWith } from 'rxjs/operators/startWith';
import { map } from 'rxjs/operators/map';
import 'rxjs/Rx';

@Component({
  selector: 'dialog-vendor',
  template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i></a>'

})
export class DialogVendor {

  @Input() vendor: any[];
  constructor(public dialog: MatDialog) { }


  openDialog(): void {
    let dialogRef = this.dialog.open(DialogVendorDialog, {
      width: '500px',
      height: '350px',
      data: {vendor: this.vendor }
    });
    dialogRef.afterClosed().subscribe(result => {

      if (result == undefined) {

      }
      else {
        this.vendor = result;
      }

    });
  }

}

@Component({
  selector: 'dialog-vendor-dialog',
  styleUrls: ['./dialog.vendor.component.css'],
  templateUrl: 'dialog.vendor.component.html',
})
export class DialogVendorDialog {

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

  constructor(private mentalHealthService: MentalHealthService,
    public dialogRef: MatDialogRef<DialogVendorDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    dialogRef.disableClose = true;
  }

  onSubmit() {

   this.mentalHealthService.updateVendor(this.data.vendor);
    this.dialogRef.close();
  }

  onNoClick(): void {
  
    this.dialogRef.close();
  }

}
