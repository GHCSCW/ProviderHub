import { Component, Inject, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { startWith } from 'rxjs/operators/startWith';
import { map } from 'rxjs/operators/map';

import { Gender } from '../../services/enum.service'
import { MentalHealthService } from '../../services/mental-health.service';

export class Language {
  constructor(public name: string) { }
}


@Component({
  selector: 'edit-facility-provider-relationship-component',
  styleUrls: ['./edit-facility-provider-relationship.component.css'],
  template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i></a>'

})
export class DialogFacilityProviderRelationship{

    @Input() facilityProviderRelationship: any[];

  constructor(public dialog: MatDialog, private mentalHealthService: MentalHealthService) {
  }


 
  openDialog(): void {
    let dialogRef = this.dialog.open(DialogFacilityProviderRelationshipDialog, {
      width: '500px',
        height: '650px',
        data: { facilityProviderRelationship: this.facilityProviderRelationship }
    });
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
     
    });
  }

}
@Component({
  selector: 'edit-facility-provider-relationship-dialog',
  templateUrl: 'edit-facility-provider-relationship.component.html',
})
export class DialogFacilityProviderRelationshipDialog {
  email = new FormControl('', [Validators.required, Validators.email]);
  originalFacilityProviderRelationship: any = []; 

  constructor(private mentalHealthService: MentalHealthService,
    public dialogRef: MatDialogRef<DialogFacilityProviderRelationshipDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    dialogRef.disableClose = true;
    this.originalFacilityProviderRelationship = JSON.parse(JSON.stringify(this.data.facilityProviderRelationship));
  }
  
  getErrorMessage() {
    return this.email.hasError('email') ? 'Not a valid email' :
        '';
  }

  onNoClick(): void {
    Object.assign(this.data.facilityProviderRelationship, this.originalFacilityProviderRelationship);
    this.dialogRef.close();
  }

  onSubmit() {

      this.mentalHealthService.updateFacilityProviderRelationship(this.data.facilityProviderRelationship).subscribe(updateFacilityProviderRelationship => {
          Object.assign(this.data.facilityProviderRelationship, updateFacilityProviderRelationship);
      });
    this.dialogRef.close();
  }
}
