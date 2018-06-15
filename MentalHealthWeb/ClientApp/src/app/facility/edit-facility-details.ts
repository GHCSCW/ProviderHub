import { Component, Inject, Input } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MentalHealthService } from '../services/mental-health.service';
import * as moment from 'moment';
import { AuthenticationService } from '../services/authentication.service';

@Component({
  selector: 'edit-facility-details',
  styleUrls: ['./edit-facility-details.dialog.css'],
  template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i> </a>'
})
export class DialogFacilityDetails {

  @Input() facility: any[];

  constructor(public dialog: MatDialog) { }

  openDialog(): void {
    let dialogRef = this.dialog.open(DialogFacilityDetailsDialog, {
      width: '650px',
      height: '850px',
      data: { facility: this.facility}
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }

}

@Component({
  selector: 'edit-facility-details-dialog',
  templateUrl: 'edit-facility-details.dialog.html',
})
export class DialogFacilityDetailsDialog {

  originalFacility: any = []; 
  constructor(
    private mentalHealthService: MentalHealthService,
    public dialogRef: MatDialogRef<DialogFacilityDetailsDialog>,
    private authSvc: AuthenticationService,
    @Inject(MAT_DIALOG_DATA) public data: any)
  {
    this.originalFacility = JSON.parse(JSON.stringify(data.facility));
    dialogRef.disableClose = true;
  }
  states = this.mentalHealthService.getStates();
  regionList = this.mentalHealthService.getRegions();

  onNoClick(): void {
    Object.assign(this.data.facility, this.originalFacility);
    this.dialogRef.close();
  }

  onSubmit() {
    this.mentalHealthService.updateFacility(this.data.facility).subscribe(updateFacility => {
      updateFacility.lastUpdatedDate = moment().toDate();
      updateFacility.lastUpdatedBy = this.authSvc.userName;
      Object.assign(this.data.facility, updateFacility);
    });
    this.dialogRef.close();
  }
}
