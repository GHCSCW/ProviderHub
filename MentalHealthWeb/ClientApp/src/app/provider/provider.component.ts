import { Component, OnInit, Input } from '@angular/core';
import { MentalHealthService } from '../service/mental.health.service'
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from "@angular/router";
import { MatDialogModule } from '@angular/material/dialog';
import { Location } from '@angular/common';
import { NavbarService } from '../service/navbarservice';
import { Gender } from '../service/enum-service'
import 'rxjs/add/operator/map';
import { IProviderLanguageMapping } from '../interfaces/IProviderLanguageMapping';
import { ILanguage } from '../interfaces/ILanguage';

@Component({
  selector: 'provider',
  templateUrl: './provider.component.html',
  styleUrls: ['./provider.component.css']
})

export class ProviderComponent implements OnInit {
    originalFacilityProviderRelationship: any;

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

  constructor(
    private mentalHealthService: MentalHealthService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private location: Location,
    public nav: NavbarService
  )
  {
    this.showHide = false;
  }

  ngOnInit() {
    this.nav.show();
    var languages: ILanguage[] = [
      { languageID: 1, languageName: 'English' },
      { languageID: 2, languageName: 'Spanish' },
      {
        languageID: 3, languageName: 'Hmong'
      }];

 
 
    this.mentalHealthService.getFacilityProviderRelationshipData().map(results => {
      if (results.provider == undefined) {
        this.provider = this.mentalHealthService.getProviderData();
        this.gender = Gender[this.provider.gender];
      }
      else {
        this.provider = results.provider;
        this.originalProvider = results.provider;
        this.facilityProviderRelationship = results;
        this.originalFacilityProviderRelationship = JSON.parse(JSON.stringify(results));
        this.edittedFacilityProviderRelationship = results;
        this.gender = Gender[this.provider.gender];
      }
    });

    if (this.provider == undefined || this.facilityProviderRelationship.length == 0) {
      this.fillProviderData()
    }
  }

  fillProviderData (){
   return this.route.params.subscribe(params => {
      console.log(params);
      if (params['id']) {
        this.mentalHealthService.getFacilityProviderRelationshipById(params['id']).subscribe(data => {
          this.facilityProviderRelationship = data;
          this.originalFacilityProviderRelationship = JSON.parse(JSON.stringify(data));
          this.provider = data.provider;
          this.originalProvider = JSON.parse(JSON.stringify(data.provider));
          this.facility = data.facility;
          this.facilityAddress = data.facility.facilityAddress;
          this.gender = Gender[this.provider.gender];
          this.mentalHealthService.insertFacilityProviderRelationshipData(data);
          this.nav.addFacilityRelationshipProviderID(data);
        
        })
      }
      else if (params['provid']) {
        this.mentalHealthService.getProvider(params['provid']).subscribe(data => {
          //this.facilityProviderRelationship = data;
          this.provider = data;
          this.originalProvider = JSON.parse(JSON.stringify(data));
          this.gender = Gender[this.provider.gender];
          this.mentalHealthService.insertFacilityProviderRelationshipData(data);
        })
      }

    });
  }
}

