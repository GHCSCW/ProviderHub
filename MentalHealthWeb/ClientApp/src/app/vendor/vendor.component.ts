import { Component, OnInit, Input } from '@angular/core';
import { MentalHealthService } from '../service/mental.health.service'
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from "@angular/router";
import 'rxjs/add/operator/map';
import { MatDialogModule } from '@angular/material/dialog';
import { Location } from '@angular/common';
import { NavbarService } from '../service/navbarservice';

@Component({
  selector: 'vendor',
  templateUrl: './vendor.component.html',
  styleUrls: ['./vendor.component.css']
})
export class VendorComponent implements OnInit {

  today: number = Date.now();
  vendor: any = [];
  edittedVendor: any = [];
  facilityProviderRelationship: any = [];
  provider: any = [];
  facility: any = [];
  facilityAddress: any = [];
  providerId: number;

  constructor(private mentalHealthService: MentalHealthService, private fb: FormBuilder, private route: ActivatedRoute, public nav: NavbarService) {

  }
  ngOnInit() {
    this.nav.show();
    this.mentalHealthService.getFacilityProviderRelationshipData().map(results => {

      this.vendor = results;
      this.edittedVendor = JSON.parse(JSON.stringify(results));
    });
  this.fillVendorData();
  }

  fillVendorData() {
    return this.route.params.subscribe(params => {
      console.log(params);
      if (params['id']) {
        this.mentalHealthService.getVendor(params['id']).subscribe(data => {
          this.vendor = data;
          this.edittedVendor = JSON.parse(JSON.stringify(data));
          this.nav.addVendorID(this.vendor.id);
        })
      }
    });
  }
}


//this.route.params.subscribe(params => {
//  console.log(params);
//  if (params['id']) {
//    this.mentalHealthService.getVendor(params['id']).subscribe(data => {
//      this.vendor = data;

//      this.myform.patchValue({
//        firstName: data.firstName,
//        lastName: data.lastName
//      });
//    })
//  }
//});
