import { Component, Inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MentalHealthService } from '../services/mental-health.service';
import { FormControl } from "@angular/forms";
import { FormBuilder, Validators, FormsModule, NgForm, FormGroup } from '@angular/forms';
import 'rxjs/Rx';



@Component({
  selector: 'create-vendor',
  styleUrls: ['./create-vendor.component.css'],
  templateUrl: './create-vendor.component.html'

})

export class CreateVendorComponent implements OnInit {


  createVendorForm: FormGroup;
  FacilityName: string = '';
  TaxId: string = '';
  NPI: number = null;
  ExternalId: string = '';
  ExternalNotes: string = '';
  VendorEpicId: number = null;


  constructor(private fb: FormBuilder, private mentalHealthService: MentalHealthService) {

    this.createVendorForm = fb.group({
      'VendorName': [],
      'NPI': [],
      'TaxId':[],
      'ExternalId': [],
      'VendorEpicId':[],
      'InternalNotes':[]
    });
  }


  ngOnInit() {

  }
  onFormSubmit(form: NgForm) {
   this.mentalHealthService.createVendor(form);
    console.log(form);
  }
}
