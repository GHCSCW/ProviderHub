import { Component, ViewChild, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort } from '@angular/material';
import { FormControl, FormGroup, FormBuilder, Validators, NgForm } from '@angular/forms';
import { Router } from "@angular/router";
import { Subject } from 'rxjs';

import { MentalHealthService } from "../services/mental-health.service";
import { BHAttributeType } from '../services/enum.service';
import { NavbarService } from '../services/navbar.service';
import { DataSource } from '@angular/cdk/collections';
//import { Pipe, PipeTransform } from '@angular/core';


import { Observable } from 'rxjs/Observable';
import { catchError, map, tap, filter, startWith, switchMap, debounceTime, distinctUntilChanged, takeWhile, first } from 'rxjs/operators';
import { AbstractControl } from '@angular/forms/src/model';


@Component({
  selector: 'advanced-search',
  templateUrl: './advanced.search.component.html',
  styleUrls: ['./advanced.search.component.css']
})


export class AdvancedSearchComponent implements OnInit {

  results: any;
  loading: boolean = false;
  showVar: boolean = true;
  facilityProviderRelationships: any = [];
  searchForm: FormGroup;
  message: string;
  provider: any = [];
  cities: any = [];
  gender: any = [];
  regions: any = [];
  languages: any = [];
  facilityList: any = [];
  previousResultsArray: any = [];
  providerName: string;
  genders: any = [];
  ages: any = [];
  conditions: any = [];
  therapeuticApproaches: any = [];
  otherList: any = [];
  modes: any = [];
  visible: boolean = true;
  selectable: boolean = true;
  removable: boolean = true;
  addOnBlur: boolean = true;
  acceptingNewPatients: boolean;
  filteredOptions: Observable<any[]>;
  provConditionsList: any = [];
  therapeuticApproachesList: any = [];
  newResults: any = [];

  advancedSearchForm: FormGroup;

  displayedColumns = ['fullName', 'facilityName', 'facilityAddress', 'facilityPhoneNumber', 'conditions', 'therapeuticApproaches'];

  dataSource = new MatTableDataSource<any>([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;


  constructor
    (
    private mentalHealthService: MentalHealthService,
    private fb: FormBuilder, private router: Router,
    public nav: NavbarService
    ) {

  }

  ngOnInit() {

    this.mentalHealthService.getAdvancedSearchResults().map(results => {
      this.dataSource = new MatTableDataSource<any>(results);

     // this.dataSource.paginator = this.paginator; this was clearing the data???
    });
    this.nav.hide();
    this.setAges();
    this.setConditions();
    this.setModes();
    this.setOthers();
    this.setTherapeuticApproaches();


    this.cities = this.mentalHealthService.getCities();
    this.regions = this.mentalHealthService.getRegions();
    this.mentalHealthService.getLanguages().subscribe(
      result => this.languages = result
    );
    this.genders = this.mentalHealthService.getGenders();
    this.mentalHealthService.getFacilityList().subscribe(data => {
      this.facilityList = data;
    });

    this.previousResultsArray = this.mentalHealthService.getAdvancedSearchQuery();

    if (this.previousResultsArray.length == 0) {
      this.advancedSearchForm = this.fb.group({
        'gender': [],
        'formLanguage': [],
        'region': [],
        'age': [],
        'condition': [],
        'theraApproach': [],
        'mode': [],
        'city': [],
        'other': [],
        'cspIndicator': [],
        'medicareIndicator': [],
        'badgercareIndicator': [],
        'prescribingProvider': [],
        'acceptingNewPatients': [],
        'facilityID': []
      })
      this.acceptingNewPatients = true;
    }
    else {
      this.advancedSearchForm = this.fb.group({
        'gender': [],
        'formLanguage': [],
        'region': [],
        'age': [],
        'condition': [],
        'theraApproach': [],
        'mode': [],
        'city': [],
        'other': [],
        'cspIndicator': [],
        'medicareIndicator': [],
        'badgercareIndicator': [],
        'prescribingProvider': [],
        'acceptingNewPatients': [],
        'facilityID': []
      })

      this.advancedSearchForm.patchValue({
          gender: this.previousResultsArray[0].gender,
          formLanguage: this.previousResultsArray[0].formLanguage,
          region: this.previousResultsArray[0].region,
          age: this.previousResultsArray[0].age,
          condition: this.previousResultsArray[0].condition,
          theraApporaceh: this.previousResultsArray[0].theraApproach,
          mode: this.previousResultsArray[0].mode,
          city: this.previousResultsArray[0].city,
          other: this.previousResultsArray[0].other,
          cspIndicator: this.previousResultsArray[0].cspIndicator,
          medicareIndicator:this.previousResultsArray[0].medicareIndicator,
          badgercareIndicator: this.previousResultsArray[0].badgercareIndicator,
          prescribingProvider: this.previousResultsArray[0].prescribingProvider,
          acceptingNewPatients: this.previousResultsArray[0].acceptingNewPatients,
          facilityId: this.previousResultsArray[0].facilityId

      });
    }
  }

  filter(val: string): Observable<any[]> {
    return this.mentalHealthService.getFacilityList()
      .pipe(
      map(response => response.filter(option => {
        return option.FacilityName.toLowerCase().indexOf(val.toLowerCase()) >= 0
      }))
      )
  }
  providerRelationshipRoute(provRelationship) {

    this.mentalHealthService.insertFacilityProviderRelationshipData(provRelationship);
    this.nav.addFacilityRelationshipProviderID(provRelationship);
    this.router.navigate(["/provider/facilityrel/" + provRelationship.relationshipID]);

  }

  providerRelationshipFacilityRoute(provRelationship) {
    this.mentalHealthService.insertFacilityProviderRelationshipData(provRelationship);
    this.nav.addFacilityRelationshipProviderID(provRelationship);
    this.router.navigate(["/facility/" + provRelationship.facility.id]);
  }

  clearDataSource() {
    this.dataSource = new MatTableDataSource<any>([]);
    this.facilityProviderRelationships = [];
    this.newResults = [];
    this.message = '';
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  onFormSubmit(form) {
    this.loading = true;
    var searchObject = [];
    this.mentalHealthService.saveAdvancedSearchQuery(form);
    //Build all array key values
    searchObject.push({ key: "Gender", value: form.gender });
    searchObject.push({ key: "Language", value: form.formLanguage });
    searchObject.push({ key: "BHAttributeSet", value: form.age });
    searchObject.push({ key: "BHAttributeSet", value: form.condition });
    searchObject.push({ key: "BHAttributeSet", value: form.theraApproach });
    searchObject.push({ key: "BHAttributeSet", value: form.mode });
    searchObject.push({ key: "BHAttributeSet", value: form.other });
    searchObject.push({ key: "FacilityID", value: form.facilityID })

    var city = new Array(form.city);
    searchObject.push({ key: "City", value: city });

    var region = new Array(form.region);
    searchObject.push({ key: "Region", value: region});

    //Build CSP Indicator Key Value
    var i = form.cspIndicator ? 1 : null;
    var cspIndicatorArray = [];
    cspIndicatorArray.push(i);
    searchObject.push({ key: "CSP", value: cspIndicatorArray });

    //Build Badgercare Indicator Key Value
    var i = form.badgercareIndicator ? 1 : null;
    var badgercareIndicatorArray = [];
    badgercareIndicatorArray.push(i);
    searchObject.push({ key: "MedicaidIndicator", value: badgercareIndicatorArray });

    //Build Medicare Indicator Key Value
    var i = form.medicareIndicator ? 1 : null;
    var medicareIndicatorArray = [];
    medicareIndicatorArray.push(i);
    searchObject.push({ key: "MedicareIndicator", value: medicareIndicatorArray });
  
    //Build Prescribing Provider Key Value
    var i = form.prescribingProvider ? 1 : null;
    var prescriberArray = [];
    prescriberArray.push(i);
    searchObject.push({ key: "Prescriber", value: prescriberArray });

    //Build Accepting New Patients
    var i = form.acceptingNewPatients ? 1 : null;
    var acceptigNewPatientsArray = [];
    acceptigNewPatientsArray.push(i);
    searchObject.push({ key: "AcceptingNewPatient", value: acceptigNewPatientsArray });

    //searchObject.filter(x => x != null)
    searchObject = searchObject.filter(x => x.value != null && x.value.length > 0)

    this.mentalHealthService.advancedSearch(searchObject).subscribe(results => {
      
      this.loading = false;
      this.newResults = [];
      this.facilityProviderRelationships = results;

      for (var category of results) {
        this.provConditionsList = [];
        this.therapeuticApproachesList = [];

        var providerConditions = category.behavioralHealthAttributes.filter(conditions =>
          conditions.bhSpecialtyType == 3
        );

        for (var list of providerConditions) {
          this.provConditionsList.push(list.textValue)
        }
        category["providerConditionsList"] = this.provConditionsList;

        var therapeuticApproaches = category.behavioralHealthAttributes.filter(conditions =>
          conditions.bhSpecialtyType == 4
        );

        for (var list of therapeuticApproaches ) {
          this.therapeuticApproachesList.push(list.textValue)
        }
        category["therapeuticApproachesList"] = this.therapeuticApproachesList;

        this.newResults.push(category);
      }

      this.dataSource = new MatTableDataSource<any>(this.newResults);
      this.mentalHealthService.insertAdvancedSearchResults(results);
      this.dataSource.paginator = this.paginator;

      if (results.length == 0) {
        this.message = 'No Results were found.';
      }
      else {
        this.message = '';

      }
    });
  }

  setAges() {

    this.mentalHealthService.getBehavioralHealthAttributeByID(BHAttributeType.Ages).subscribe(val =>
      this.ages = val
    );
  }

  setConditions() {

    this.mentalHealthService.getBehavioralHealthAttributeByID(BHAttributeType.Conditions).subscribe(val =>
      this.conditions = val
    );
  }

  setModes() {

    this.mentalHealthService.getBehavioralHealthAttributeByID(BHAttributeType.Models).subscribe(val =>
      this.modes = val
    );
  }

  setOthers() {
    this.mentalHealthService.getBehavioralHealthAttributeByID(BHAttributeType.Other).subscribe(val =>
      this.otherList = val
    );
  }

  setTherapeuticApproaches() {
    this.mentalHealthService.getBehavioralHealthAttributeByID(BHAttributeType.TherapeuticApproaches).subscribe(val =>
      this.therapeuticApproaches = val
    );
  }
}
