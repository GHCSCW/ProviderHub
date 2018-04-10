import { Component, Inject, Input } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import {MentalHealthService } from '../service/mental.health.service';

/**
 * @title Dialog Overview
 */
@Component({
  selector: 'dialog-facility-details',
  //templateUrl: 'dialog-facility-details.html',
  styleUrls: ['./dialog-facility-details-dialog.css'],
  template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i> </a>'
})
export class DialogFacilityDetails {

  @Input() facility: any[];
  @Input() originalFacility: any[];
  constructor(public dialog: MatDialog) { }

  openDialog(): void {
    let dialogRef = this.dialog.open(DialogFacilityDetailsDialog, {
      width: '500px',
      height: '850px',

      data: { facility: this.facility, originalFacility: this.originalFacility }
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }

}

@Component({
  selector: 'dialog-facility-details-dialog',
  templateUrl: 'dialog-facility-details-dialog.html',
})
export class DialogFacilityDetailsDialog {

  constructor(private mentalHealthService: MentalHealthService,
    public dialogRef: MatDialogRef<DialogFacilityDetailsDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  states = this.mentalHealthService.getStates();

  onNoClick(): void {

    Object.assign(this.data.facility, this.data.originalFacility);
    this.dialogRef.close();
  }

  onSubmit() {

    this.mentalHealthService.updateFacility(this.data.facility).subscribe(updateFacility => {
      Object.assign(this.data.originalFacility, updateFacility);

    });
    this.dialogRef.close();
  }

}
