import { Component, Inject, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MentalHealthService } from '../../service/mental.health.service';
import { MatChipInputEvent } from '@angular/material';
import { ENTER, COMMA } from '@angular/cdk/keycodes';
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { startWith } from 'rxjs/operators/startWith';
import { map } from 'rxjs/operators/map';
import 'rxjs/Rx';
import { ArrayService } from '../../service/array.service';
import { BHAttributeType } from '../../service/enum-service';

@Component({
  selector: 'dialog-edit-bhAttribute',
  template: '<a class="btn"><i (click)="openDialog()" class="fa fa-pencil-square-o pull-right" aria-hidden="true"></i></a>'

})
export class DialogEditbhAttribute {


  @Input() bhaConditions: any[];
  @Input() bhaModes: any[];
  @Input() bhaAges: any[];
  @Input() bhaOthers: any[];
  @Input() bhaTherapeuticApproaches: any[];

  constructor(public dialog: MatDialog) { }

  openDialog(): void {
    let dialogRef = this.dialog.open(DialogEditbhAttributeDialog, {
      width: '650px',
      height: '400px',
      data: { bhaConditions: this.bhaConditions, bhaModes: this.bhaModes, bhaAges: this.bhaAges, bhaOthers: this.bhaOthers, bhaTherapeuticApproaches: this.bhaTherapeuticApproaches }
    });
    dialogRef.afterClosed().subscribe(result => {
    });
  }
}


@Component({
  selector: 'dialog-edit-bhAttributes',
  templateUrl: 'dialog-edit-bhAttributes.html',
  styleUrls: ['./dialog-edit-bhAttributes.css'],
})
export class DialogEditbhAttributeDialog {
  othersCopy: any;
  therapeuticApproachesCopy: any;
  modesCopy: any;
  agesCopy: any;
  ages: any = [];
  therapeuticApproaches: any = [];
  others: any = [];
  modes: any = [];
  conditions: any = [];
  conditionsCopy: any = [];
  bhaSpecialties: FormGroup;


  constructor(
    private fb: FormBuilder,
    private mentalHealthService: MentalHealthService,
    public dialogRef: MatDialogRef<DialogEditbhAttributeDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.setAges();
    this.setConditions();
    this.setModes();
    this.setOthers();
    this.setTherapeuticApproaches();
    this.conditionsCopy = JSON.parse(JSON.stringify(data.bhaConditions));
    this.agesCopy = JSON.parse(JSON.stringify(data.bhaAges));
    this.modesCopy = JSON.parse(JSON.stringify(data.bhaModes));
    this.othersCopy = JSON.parse(JSON.stringify(data.bhaOthers));
    this.therapeuticApproachesCopy = JSON.parse(JSON.stringify(data.bhaTherapeuticApproaches));
    //this.data.bhaConditions[0].textValue = 'TEST';
    this.bhaSpecialties = this.fb.group({
      'ages': [],
      'conditions': [],
      'theraApproaches': [],
      'modes': [],
      'others': []
    })

    this.conditionsCopy = this.conditionsCopy.map(function (conditions) {
      return conditions.setID
    });
    this.agesCopy = this.agesCopy.map(function (ages) {
      return ages.setID
    });
    this.modesCopy = this.modesCopy.map(function (modes) {
      return modes.setID
    });
    this.othersCopy = this.othersCopy.map(function (others) {
      return others.setID
    });
    this.therapeuticApproachesCopy = this.therapeuticApproachesCopy.map(function (theraApproach) {
      return theraApproach.setID
    });

    this.bhaSpecialties.patchValue({
      ages: this.agesCopy,
      conditions: this.conditionsCopy,
      modes: this.modesCopy,
      therapeuticApproaches: this.therapeuticApproaches,
      others: this.othersCopy
    });
  }

  onFormSubmit(form) {
    //save specalties
  }



  setAges() {
    this.mentalHealthService.getBehavioralHealthAttributeByID(BHAttributeType.Ages).subscribe(val =>
      this.ages = val
    );
  }

  setConditions() {
    this.mentalHealthService.getBehavioralHealthAttributeByID(BHAttributeType.Conditions).subscribe(val =>
      this.conditions = val
    );
  }

  setModes() {
    this.mentalHealthService.getBehavioralHealthAttributeByID(BHAttributeType.Models).subscribe(val =>
      this.modes = val
    );
  }

  setOthers() {
    this.mentalHealthService.getBehavioralHealthAttributeByID(BHAttributeType.Other).subscribe(val =>
      this.others = val
    );
  }

  setTherapeuticApproaches() {
    this.mentalHealthService.getBehavioralHealthAttributeByID(BHAttributeType.TherapeuticApproaches).subscribe(val =>
      this.therapeuticApproaches = val
    );
  }
}
