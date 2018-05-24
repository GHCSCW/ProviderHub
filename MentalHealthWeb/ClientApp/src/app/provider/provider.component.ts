import { Component, OnInit, Input, NgZone } from '@angular/core';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, NavigationStart } from "@angular/router";
import { MatDialogModule } from '@angular/material/dialog';
import { Location } from '@angular/common';
import 'rxjs/add/operator/map';

import { NavbarService } from '../services/navbar.service';
import { MentalHealthService } from '../services/mental-health.service'
import { Language, Gender } from '../services/enum.service';
import { IProviderLanguageMapping } from '../interfaces/IProviderLanguageMapping';
import { ILanguage } from '../interfaces/ILanguage';
import { Facility } from '../models/facility';
import { AuthenticationService } from '../services/authentication.service';


@Component({
  selector: 'provider',
  templateUrl: './provider.component.html',
  styleUrls: ['./provider.component.css']
})

export class ProviderComponent implements OnInit {

  canEdit: any = [];
  public facilityList: Facility[] = [];
  relationshipDataByProvider: any = [];
  bhaAges: any[];
  bhaModes: any[];
  bhaConditions: any[];
  bhaTherapeuticApproaches: any[];
  bhaOthers: any[];

  panelOpenState: boolean = true;
  facilityProviderRelationship: any = [];
  edittedFacilityProviderRelationship: any[];
  provider: any = [];
  originalProvider: any = [];
  facility: any = [];
  facilityAddress: any = [];
  toppingList: any = []
  credentialList: any = [];
  isOffline: boolean;
  gender: string;
  test: string;
  showHide: boolean;
  dropdownList: any = [];
  selectedItems: any = [];
  dropdownSettings = {};

  constructor(
    private mentalHealthService: MentalHealthService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private location: Location,
    public  nav: NavbarService,
    private authSvc: AuthenticationService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private zone: NgZone
  ) {
    this.showHide = false;
    this.router.events
      .map(event => event instanceof NavigationStart)
      .subscribe(() => {
        // TODO
      });
    this.activatedRoute.url.subscribe(url => {
      console.log(url);
    });
  }
 
  ngOnInit() {

   
    this.authSvc.canEditUpdated.subscribe(edit => {
      this.canEdit = edit;
    });
    this.nav.show();
    //this.canEdit = this.authSvc.canEdit;
    this.mentalHealthService.getFacilityProviderRelationshipData().map(results => {
      if (results.provider == undefined) {
        this.provider = this.mentalHealthService.getProviderData();
        this.gender = Gender[this.provider.gender];

      }
      else {
        this.provider = results.provider;
        this.facilityProviderRelationship = results;
        this.createBHSpecialtyLists(results);
        this.gender = Gender[this.provider.gender];
        this.mentalHealthService.GetRelationshipDataByProviderID(this.provider.id, this.facilityProviderRelationship.relationshipID).subscribe(results => {
          this.relationshipDataByProvider = results
        });
      }
    });

    if (this.provider == undefined || this.facilityProviderRelationship.length == 0) {
      this.fillProviderData();
    }
  }

  onItemSelect(item: any) {
  }

  OnItemDeSelect(item: any) {
  }

  onSelectAll(items: any) {
  }

  onDeSelectAll(items: any) {
  }
  fillProviderData() {
    return this.route.params.subscribe(params => {
      console.log(params);
      if (params['id']) {
        this.mentalHealthService.getFacilityProviderRelationshipById(params['id']).subscribe(data => {
          this.facilityProviderRelationship = data;
          this.provider = data.provider;
          this.facility = data.facility;
          this.facilityAddress = data.facility.facilityAddress;
          this.gender = Gender[this.provider.gender];
          this.createBHSpecialtyLists(this.facilityProviderRelationship);
          this.mentalHealthService.insertFacilityProviderRelationshipData(data);
          this.nav.addFacilityRelationshipProviderID(data);
          this.mentalHealthService.GetRelationshipDataByProviderID(this.provider.id, this.facilityProviderRelationship.relationshipID).subscribe(results => {
            this.relationshipDataByProvider = results
          });
        })
      }
      else if (params['provid']) {
        this.mentalHealthService.getProvider(params['provid']).subscribe(data => {
          this.provider = data;
          this.gender = Gender[this.provider.gender];
          this.mentalHealthService.insertFacilityProviderRelationshipData(data);
        })
      }

    });
  }

  createBHSpecialtyLists(facilityProviderRelationship): any {

    this.bhaAges = facilityProviderRelationship.behavioralHealthAttributes.filter(ages =>
      ages.bhSpecialtyType == 1
    );

    this.bhaModes = facilityProviderRelationship.behavioralHealthAttributes.filter(modes =>
      modes.bhSpecialtyType == 2
    );

    this.bhaConditions = facilityProviderRelationship.behavioralHealthAttributes.filter(conditions =>
      conditions.bhSpecialtyType == 3
    )


    this.bhaTherapeuticApproaches = facilityProviderRelationship.behavioralHealthAttributes.filter(theraApproaches =>
      theraApproaches.bhSpecialtyType == 4
    );

    this.bhaOthers = facilityProviderRelationship.behavioralHealthAttributes.filter(others =>
      others.bhSpecialtyType == 5
    );
  }

  RouteNewProvider(data) {
    this.mentalHealthService.insertFacilityProviderRelationshipData(data);
    this.nav.addFacilityRelationshipProviderID(data);
    this.zone.run(() => {
      this.router.navigateByUrl('/provider/facilityrel/' + data.relationshipID);
    });
  }
}

