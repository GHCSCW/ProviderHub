import { Component, OnInit } from '@angular/core';
import { MentalHealthService } from '../service/mental.health.service'
import { ActivatedRoute } from "@angular/router";
import { NavbarService } from "../service/navbarservice";
import { HttpClient } from "@angular/common/http";
import { FormControl, FormGroup } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { catchError, map, tap, filter, startWith, switchMap, debounceTime, distinctUntilChanged, takeWhile, first } from 'rxjs/operators';
import { AddressType } from '../service/enum-service';


@Component({
  selector: 'facility',
  templateUrl: './facility.component.html',
  styleUrls: ['./facility.component.css']
})
export class FacilityComponent  {
  addressType: string;
  edittedFacility: any;
  inputFormControl: any;
  public autocompleteOptions$: Observable<string[]>;
  states: any [];
  today: number = Date.now();
  facility: any = []; 
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
        this.edittedFacility = JSON.parse(JSON.stringify(this.facility));
       
      }
      else {
        this.provider = results.provider;
        this.facility = results.facility;
        this.facilityAddress = results.facility.facilityAddress;
        this.edittedFacility = JSON.parse(JSON.stringify(results.facility));
        this.facilityProviderRelationship = results;
      }

    });
    this.mentalHealthService.getProviderList().subscribe(data => {
      this.providerList = data;
    });
    //this.mentalHealthService.getFacilityList().subscribe(data => {
    //  this.facilityList = data;
    //});

    this.fillFacilityData();


    this.filteredOptions = this.myControl.valueChanges
      .pipe(
      startWith(null),
      debounceTime(200),
      distinctUntilChanged(),
      switchMap(val => {
        return this.filter(val || '')
      })
      );
  
  }
 
  filter(val: string): Observable<any[]> {
    return this.mentalHealthService.getFacilityList()
      .pipe(
      map(response => response.filter(option => {
        return option.facilityName.toLowerCase().indexOf(val.toLowerCase()) >= 0
      }))
      )
  }
  fillFacilityData() {
    return this.route.params.subscribe(params => {
      console.log(params);
      if (params['id']) {
        this.mentalHealthService.getFacility(params['id']).subscribe(data => {
          this.facility = data;
          this.facilityAddress = data.facilityAddress;
          this.edittedFacility = data;
          this.nav.addFacilityID(this.facility.id);
        })
      }
    });
  }
}
