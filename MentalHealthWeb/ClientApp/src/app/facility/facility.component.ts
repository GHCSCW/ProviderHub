import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { FormControl, FormGroup } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { catchError, map, tap, filter, startWith, switchMap, debounceTime, distinctUntilChanged, takeWhile, first } from 'rxjs/operators';

import { AddressType } from '../services/enum-service';
import { MentalHealthService } from '../services/mental.health.service'
import { NavbarService } from "../services/navbarservice";


@Component({
  selector: 'facility',
  templateUrl: './facility.component.html',
  styleUrls: ['./facility.component.css']
})
export class FacilityComponent  {
  relationshipDataByProvider: any = [];
  addressType: string;
 
  inputFormControl: any;
  public autocompleteOptions$: Observable<string[]>;
  states: any [];
  today: number = Date.now();
  facility: any = [];
  originalFacility: any = [];
  providerList: any;
  facilityList: any;
  facilityProviderRelationship: any = [];
  provider: any = [];
  facilityAddress: any = [];
  myControl = new FormControl();
  filteredOptions: Observable<any[]>;

  constructor(private mentalHealthService: MentalHealthService, private route: ActivatedRoute, public nav: NavbarService, private http: HttpClient)
  {

    this.nav.show();

    this.mentalHealthService.getFacilityProviderRelationshipData().map(results => {

      if (results.facility == undefined) {
        this.facility = this.mentalHealthService.getFacilityData();
      }
      else {
        this.provider = results.provider;
        this.facility = results.facility;
        this.facilityAddress = results.facility.facilityAddress;
        this.facilityProviderRelationship = results;
        this.mentalHealthService.GetRelationshipDataByFacilityID(results.facility.id, this.facilityProviderRelationship.relationshipID).subscribe(results => {
          this.relationshipDataByProvider = results
        });
      }
    });
    this.mentalHealthService.getProviderList().subscribe(data => {
      this.providerList = data;
    });
    //this.mentalHealthService.getFacilityList().subscribe(data => {
    //  this.facilityList = data;
    //});

    this.fillFacilityData();


    //this.filteredOptions = this.myControl.valueChanges
    //  .pipe(
    //  startWith(null),
    //  debounceTime(200),
    //  distinctUntilChanged(),
    //  switchMap(val => {
    //    return this.filter(val || '')
    //  })
    //  );
  
  }
 
  //filter(val: string): Observable<any[]> {
  //  return this.mentalHealthService.getFacilityList()
  //    .pipe(
  //    map(response => response.filter(option => {
  //      return option.FacilityName.toLowerCase().indexOf(val.toLowerCase()) >= 0
  //    }))
  //    )
  //}
  fillFacilityData() {
    return this.route.params.subscribe(params => {
      console.log(params);
      if (params['id']) {
        this.mentalHealthService.getFacility(params['id']).subscribe(data => {
          this.facility = data;
          this.facilityAddress = data.facilityAddress;
          this.nav.addFacilityID(this.facility.id);
        })
        this.mentalHealthService.GetRelationshipDataByFacilityID(this.facility.id,this.facilityProviderRelationship.relationshipID).subscribe(results => {
          this.relationshipDataByProvider = results
        });
      }
    });
  }
}
