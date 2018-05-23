import { Component, OnInit, Input, Output } from '@angular/core';
import { EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormControl } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material';
import { FormBuilder, Validators, FormsModule, NgForm } from '@angular/forms';

import { MentalHealthService } from '../services/mental.health.service';
import { InterfaceService } from '../services/interface.service';

@Component({
  selector: 'create-address',
  templateUrl: './address.component.html',
  styleUrls: ['./address.component.css']
})
export class AddressComponent implements OnInit {
 

  @Input() facility: string;
  @Output() notify: EventEmitter<NgForm> = new EventEmitter<NgForm>();

  addressForm: FormGroup;
  addressType: string = ''; 
  addressLine1: string = '';
  addressLine2: string = '';
  city: string = '';
  state: string = '';
  zipCode: string = '';
  county: string = '';
  region: string = '';
  phoneNumber: string = '';
  phoneExtention: string = '';
  alternatePhoneNumber: string = '';
  faxNumber: string = '';
  email: string = '';
  website: string = '';
  contactFirstName: string = '';
  contactLastName: string = '';



  constructor(public interfaceService: InterfaceService, private fb: FormBuilder, private mentalHealthService: MentalHealthService) {
   
    this.addressForm = fb.group({
      'addressType': [null, Validators.required],
      'addressLine1': [null, Validators.compose([Validators.required, Validators.minLength(5), Validators.maxLength(500)])],
      'addressLine2': [null],
      'city': [null, Validators.required],
      'state': [null, Validators.required],
      'zipCode': [null, Validators.required],
      'county': [null],
      'region': [null],
      'phoneNumber': [null],
      'phoneExtension': [null],
      'alternatePhoneNumber': [null],
      'faxNumber': [null],
      'website': [null],
      'email': [null],
      'contactFirstName': [null],
      'contactLastName': [null]
    });  
  }


  ngOnInit() {

  }

  addressTypes = [
    {
      value: 1,
      label: 'Clinical Practice Service Location'
    },
    {
      value: 2,
      label: 'Main Address'
    },
    {
      value: 3,
      label: 'Mail Address'
    },
    {
      value: 3,
      label: 'Business Administration'
    },
  ];

  onClick() {
    this.notify.emit(this.addressForm.value);
  }
}
