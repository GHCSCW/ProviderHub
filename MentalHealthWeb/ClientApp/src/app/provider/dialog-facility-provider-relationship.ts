import { Component, Inject, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { startWith } from 'rxjs/operators/startWith';
import { map } from 'rxjs/operators/map';

import { Gender } from '../services/enum-service'
import { MentalHealthService } from '../services/mental.health.service';

export class Language {
  constructor(public name: string) { }
}


@Component({
  selector: 'dialog-facility-provider-relationship',
  styleUrls: ['./dialog-facility-provider-relationship.css'],
  template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i></a>'

})
export class DialogFacilityProviderRelationship{

    @Input() facilityProviderRelationship: any[];
    @Input() originalFacilityProviderRelationship: any[];


  constructor(public dialog: MatDialog, private mentalHealthService: MentalHealthService) {
  }


 
  openDialog(): void {
    let dialogRef = this.dialog.open(DialogFacilityProviderRelationshipDialog, {
      width: '500px',
        height: '650px',
        data: { facilityProviderRelationship: this.facilityProviderRelationship, originalFacilityProviderRelationship: this.originalFacilityProviderRelationship }
    });
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
     
    });
  }

}
@Component({
  selector: 'dialog-facility-provider-relationship-dialog',
  templateUrl: 'dialog-facility-provider-relationship.html',
})
export class DialogFacilityProviderRelationshipDialog {
  email = new FormControl('', [Validators.required, Validators.email]);


  constructor(private mentalHealthService: MentalHealthService,
    public dialogRef: MatDialogRef<DialogFacilityProviderRelationshipDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
  }

  getErrorMessage() {
    return this.email.hasError('email') ? 'Not a valid email' :
        '';
  }

  onNoClick(): void {
    Object.assign(this.data.facilityProviderRelationship, this.data.originalFacilityProviderRelationship);
    this.dialogRef.close();
  }

  onSubmit() {

      this.mentalHealthService.updateFacilityProviderRelationship(this.data.facilityProviderRelationship).subscribe(updateFacilityProviderRelationship => {
          Object.assign(this.data.originalFacilityProviderRelationship, updateFacilityProviderRelationship);
   
      });
    this.dialogRef.close();
  }
}
