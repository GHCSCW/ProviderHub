import { Component, Inject, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

import { MatChipInputEvent } from '@angular/material';
import { ENTER, COMMA } from '@angular/cdk/keycodes';
import { FormControl, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { startWith } from 'rxjs/operators/startWith';
import { map } from 'rxjs/operators/map';
import 'rxjs/Rx';
import { ArrayService } from '../../services/array.service';
import { BHAttributeType } from '../../services/enum.service';
import { MentalHealthService } from '../../services/mental-health.service';
import { forEach } from '@angular/router/src/utils/collection';

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
      width: '500px',
      height: '900px',
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
  originalAges: any = [];
  originalModes: any = [];
  originalConditions: any = [];
  originalOthers: any = [];
  originalTherapeuticApproaches: any = [];

  ages: any = [];
  therapeuticApproaches: any = [];
  others: any = [];
  modes: any = [];
  conditions: any = [];
  bhaSpecialties: FormGroup;
  bhAttributeArray: any = [];

  agesDropdownSettings = {};
  othersDropdownSettings: {};
  modesDropdownSettings: {};
  therapeuticApproachesDropdownSettings: {};
  conditionsDropdownSettings: {};

  constructor(
    private fb: FormBuilder,
    private mentalHealthService: MentalHealthService,
    public dialogRef: MatDialogRef<DialogEditbhAttributeDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {


    this.originalAges = JSON.parse(JSON.stringify(this.data.bhaAges));
    this.originalModes = JSON.parse(JSON.stringify(this.data.bhaModes));
    this.originalConditions = JSON.parse(JSON.stringify(this.data.bhaConditions));
    this.originalOthers = JSON.parse(JSON.stringify(this.data.bhaOthers));
    this.originalTherapeuticApproaches = JSON.parse(JSON.stringify(this.data.bhaTherapeuticApproaches));

    this.setAges();
    this.setConditions();
    this.setModes();
    this.setOthers();
    this.setTherapeuticApproaches();
    this.bhaSpecialties = this.fb.group({
      'ages': [],
      'conditions': [],
      'therapeuticApproaches': [],
      'modes': [],
      'others': []
    })

    this.agesDropdownSettings = {
      singleSelection: false,
      text: "Select Ages",
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      maxHeight: 500,
      primaryKey: 'setID',
      labelKey: 'textValue'
    };
    this.conditionsDropdownSettings = {
      singleSelection: false,
      enableSearchFilter: true,
      text: "Select Conditions",
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      maxHeight: 500,
      primaryKey: 'setID',
      labelKey: 'textValue'
    };
    this.therapeuticApproachesDropdownSettings = {
      singleSelection: false,
      text: "Select Thereapeutic Approaches",
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      maxHeight: 500,
      primaryKey: 'setID',
      labelKey: 'textValue'
    };
    this.modesDropdownSettings = {
      singleSelection: false,
      enableSearchFilter: true,
      text: "Select Modes",
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      maxHeight: 500,
      primaryKey: 'setID',
      labelKey: 'textValue'
    };
    this.othersDropdownSettings = {
      singleSelection: false,
      text: "Select Other",
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      maxHeight: 500,
      primaryKey: 'setID',
      labelKey: 'textValue'
    };


    this.bhaSpecialties = this.fb.group({
      ages: [[], Validators.required],
      conditions: [[], Validators.required],
      therapeuticApproaches: [[], Validators.required],
      modes: [[], Validators.required],
      others: [[], Validators.required],
    });
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
  onClick() {

  }
  onFormSubmit(form) {

    for (var key of form.ages) {
      this.bhAttributeArray.push(key)
    }
    for (var key of form.conditions) {
      this.bhAttributeArray.push(key)
    }
    for (var key of form.therapeuticApproaches) {
      this.bhAttributeArray.push(key)
    }
    for (var key of form.modes) {
      this.bhAttributeArray.push(key)
    }
    for (var key of form.others) {
      this.bhAttributeArray.push(key)
    }


    this.mentalHealthService.updateBHAttribute(this.bhAttributeArray, this.mentalHealthService.facilityProviderRelationship[0].relationshipID)
      .subscribe(
      updatedLanguage => {
        this.dialogRef.close();
      }
      )
  }


  onNoClick(): void {
    Object.assign(this.data.bhaConditions, this.originalConditions);
    Object.assign(this.data.bhaAges, this.originalAges);
    Object.assign(this.data.bhaModes, this.originalModes);
    Object.assign(this.data.bhaOthers, this.originalOthers);
    Object.assign(this.data.bhaTherapeauticApproaches, this.originalTherapeuticApproaches);

    this.dialogRef.close();
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

