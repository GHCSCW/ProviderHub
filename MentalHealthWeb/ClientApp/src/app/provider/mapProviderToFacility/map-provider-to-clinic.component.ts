import { Component, Inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl } from "@angular/forms";
import { FormBuilder, Validators, FormsModule, NgForm, FormGroup } from '@angular/forms';
import { catchError, map, tap, filter, startWith, switchMap, debounceTime, distinctUntilChanged, takeWhile, first } from 'rxjs/operators';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { Router } from '@angular/router';


import { NavbarService } from '../../services/navbarservice';
import { MentalHealthService } from '../../services/mental.health.service';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'mapprovidertoclinic',
  styleUrls: ['./map-provider-to-clinic.component.css'],
  templateUrl: './map-provider-to-clinic.component.html'

})

export class MapProviderToClinic implements OnInit {
  DropdownSettings: {};
  provider: any;
  facilityRelationshipID: any;
  selectedValue: string;
  filteredOptions: Observable<any[]>;
  clinicList: any = [];
  facilityNameGroup:  FormGroup;

  constructor(
    private mentalHealthService: MentalHealthService,
    private router: Router,
    public nav: NavbarService,
    private toastr: ToastrService,
    private fb: FormBuilder) {

    this.facilityNameGroup = this.fb.group({
      'facility': []
    });

    this.DropdownSettings = {
      singleSelection: true,
      text: "Select Facility to Map",
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      maxHeight: 500,
      primaryKey: 'id',
      labelKey: 'facilityName'
    };
  } 

  ngOnInit() {

    this.mentalHealthService.getFacilityList().subscribe(data => {
      this.clinicList = data;
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

  onFormSubmit(form) {
    var test = form;
    this.mentalHealthService.MapProviderToFacility(this.nav.providerID, form.facility[0].id).subscribe(data => {
      this.facilityRelationshipID = data
    },
      error => {
        this.toastr.error('Mapping Failed', 'This mapping to facility Failed');
      }, () => {
        this.toastr.success('Mapping Succeeded', 'This mapping to facility Succeeded');
        // this.nav.addFacilityRelationshipProviderID(this.facilityRelationshipID);
        this.router.navigate(["/provider/facilityrel/" + this.facilityRelationshipID]);
      }
    );
  }

  SaveMapping() {
    this.provider = this.mentalHealthService.getProviderData();
    //this.mentalHealthService.MapProviderToFacility(this.nav.providerID, this..value).subscribe(data => {
    //  this.facilityRelationshipID = data
    //},
    //  error => {
    //    this.toastr.error('Mapping Failed', 'This mapping to facility Failed');
    //  }, () => {
    //    this.toastr.success('Mapping Succeeded', 'This mapping to facility Succeeded');
    //   // this.nav.addFacilityRelationshipProviderID(this.facilityRelationshipID);
    //    this.router.navigate(["/provider/facilityrel/" + this.facilityRelationshipID]);
    //  }
    //);
  }

}
