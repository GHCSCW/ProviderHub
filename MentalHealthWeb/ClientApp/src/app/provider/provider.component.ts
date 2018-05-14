import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from "@angular/router";
import { MatDialogModule } from '@angular/material/dialog';
import { Location } from '@angular/common';
import 'rxjs/add/operator/map';

import { NavbarService } from '../services/navbarservice';
import { MentalHealthService } from '../services/mental.health.service'
import { Language, Gender } from '../services/enum-service';
import { IProviderLanguageMapping } from '../interfaces/IProviderLanguageMapping';
import { ILanguage } from '../interfaces/ILanguage';
import { Facility } from '../models/facility';


@Component({
  selector: 'provider',
  templateUrl: './provider.component.html',
  styleUrls: ['./provider.component.css']
})

export class ProviderComponent implements OnInit {
  //facilityList: any;
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
    public nav: NavbarService
  ) {
    this.showHide = false;
  }

  ngOnInit() {
    this.nav.show();
    //this.mentalHealthService.getFacilityList().subscribe(data => {
    //  this.facilityList = data;
    //});

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
        this.mentalHealthService.GetRelationshipDataByProviderID(this.provider.id).subscribe(results => {
          this.relationshipDataByProvider = results
        });

      }
    });

    if (this.provider == undefined || this.facilityProviderRelationship.length == 0) {
      this.fillProviderData();
    }
  }
  onItemSelect(item: any) {
    console.log(item);
    console.log(this.selectedItems);
  }
  OnItemDeSelect(item: any) {
    console.log(item);
    console.log(this.selectedItems);
  }
  onSelectAll(items: any) {
    console.log(items);
  }
  onDeSelectAll(items: any) {
    console.log(items);
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
          this.mentalHealthService.GetRelationshipDataByProviderID(this.provider.id).subscribe(results => {
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


}

