import { Component,OnInit } from '@angular/core';
import { ActivatedRoute,Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { FormControl, FormGroup } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import { catchError, map, tap, filter, startWith, switchMap, debounceTime, distinctUntilChanged, takeWhile, first } from 'rxjs/operators';
import { AddressType } from '../services/enum.service';
import { MentalHealthService } from '../services/mental-health.service'
import { NavbarService } from "../services/navbar.service";
import { AuthenticationService } from "../services/authentication.service";

@Component({
  selector: 'facility',
  templateUrl: './facility.component.html',
  styleUrls: ['./facility.component.css']
})
export class FacilityComponent  {

  canEdit: any = [];
  userRoles: any = [];
  relationshipDataByProvider: any = [];
  addressType: string;
 
  inputFormControl: any;
  public autocompleteOptions$: Observable<string[]>;
  states: any [];
  today: number = Date.now();
  facility: any = [];
  originalFacility: any = [];
  providerList: any;

  facilityProviderRelationship: any = [];
  provider: any = [];
  facilityAddress: any = [];
  myControl = new FormControl();
  filteredOptions: Observable<any[]>;

  constructor(
    private mentalHealthService: MentalHealthService,
    private route: ActivatedRoute,
    public nav: NavbarService,
    private authSvc: AuthenticationService,
    private router: Router
  )
  {

    this.nav.show();
    this.canEdit = this.authSvc.canEdit;
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

    this.fillFacilityData();

  
  }

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

  RouteNewProvider(data) {
    this.mentalHealthService.insertFacilityProviderRelationshipData(data);
    this.nav.addFacilityRelationshipProviderID(data);
    this.router.navigate(["/provider/facilityrel/" + data.relationshipID]);
  }

}
