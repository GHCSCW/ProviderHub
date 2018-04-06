import { Component, Inject, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MentalHealthService } from '../service/mental.health.service';
import { FormControl } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { startWith } from 'rxjs/operators/startWith';
import { map } from 'rxjs/operators/map';
import { Gender } from '../service/enum-service'

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


  constructor(public dialog: MatDialog) {
  }

  openDialog(): void {
    let dialogRef = this.dialog.open(DialogFacilityProviderRelationshipDialog, {
      width: '500px',
        height: '425px',
        data: { facilityProviderRelationship: this.facilityProviderRelationship, orginalFacilityProviderRelationship: this.originalFacilityProviderRelationship }
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

  constructor(private mentalHealthService: MentalHealthService,
    public dialogRef: MatDialogRef<DialogFacilityProviderRelationshipDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
  }



  onNoClick(): void {
    this.dialogRef.close();
  }

  onSubmit() {

      this.mentalHealthService.updateFacilityProviderRelationship(this.data.facilityProviderRelationship).subscribe(updateFacilityProviderRelationship => {
          Object.assign(this.data.originalFacilityProviderRelationship, updateFacilityProviderRelationship);
   
      });
   // this.dialogRef.close();
    // alert("Thanks for submitting! Data: " + JSON.stringify(this.data.providerNew));

      //this.mentalHealthService.updateProvider(this.data.provider).subscribe(updatedProvider => {
      //    Object.assign(this.data.originalProvider, updatedProvider);
      //    this.dialogRef.close();
      //});
  }
}
