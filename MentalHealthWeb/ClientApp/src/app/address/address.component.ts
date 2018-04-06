import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormControl } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { InterfaceService } from '../service/interface.service';
import { Address } from '../interfaces/Address';
import { FormBuilder, Validators, FormsModule, NgForm } from '@angular/forms';
import { MentalHealthService } from '../service/mental.health.service';
import {MatInputModule} from '@angular/material';


@Component({
  selector: 'create-address',
  templateUrl: './address.component.html',
  styleUrls: ['./address.component.css']
})
export class AddressComponent implements OnInit {
  selected: string;
  //address: Address;
  myform: FormGroup;
  regiForm: FormGroup;
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


  constructor(public interfaceService: InterfaceService, private fb: FormBuilder,private mentalHealthService: MentalHealthService) {

    this.regiForm = fb.group({
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

    this.selected = 'option2';
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

  onFormSubmit(form: NgForm) {
    this.mentalHealthService.createAddress(form);
    console.log(form);
  }  

}
