import { Component, ViewChild, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort, MatChipInputEvent } from '@angular/material';
import { Router } from "@angular/router";
import { FormControl, FormGroup, FormBuilder, Validators, NgForm } from '@angular/forms';
import { MentalHealthService } from "../service/mental.health.service";
import { NavbarService } from '../service/navbarservice';
import { facilityRelationship } from '../models/facilityRelationship';
import { Subject } from 'rxjs';
import { DataSource } from '@angular/cdk/collections';
//import { Pipe, PipeTransform } from '@angular/core';
import { ENTER, COMMA } from '@angular/cdk/keycodes';
import { BHAttributeType } from '../service/enum-service';
import { Observable } from 'rxjs/Observable';
import { catchError, map, tap, filter, startWith, switchMap, debounceTime, distinctUntilChanged, takeWhile, first } from 'rxjs/operators';
import { AbstractControl } from '@angular/forms/src/model';


@Component({
  selector: 'advanced-search',
  templateUrl: './advanced.search.component.html',
  styleUrls: ['./advanced.search.component.css']
})


export class AdvancedSearchComponent implements OnInit {



  //credentialsList = [
  //  { value: '1', viewValue: 'APNP' },
  //  { value: '2', viewValue: 'LPC' },
  //  { value: '3', viewValue: 'LCSW' }
  //];

  results: any;
  loading: boolean = false;
  showVar: boolean = true;
  facilityProviderRelationships: any = [];
  //TODO :previousSearchResults: any[];

  searchForm: FormGroup;
  message: string;
  provider: any = [];
  cities: any = [];
  gender: any = [];
  regions: any = [];
  languages: any = [];
  facilityList: any = [];
  testArray: any = [];
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
  myControl = new FormControl();
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

    this.mentalHealthService.getSearchResults().map(results => {
      //  this.nav.resetIDs();

      //  this.provider = new MatTableDataSource<any>(results.providers);
      this.dataSource = new MatTableDataSource<any>(results);

    });
    this.nav.hide();
    this.setAges();
    this.setConditions();
    this.setModes();
    this.setOthers();
    this.setTherapeuticApproaches();


    this.cities = this.mentalHealthService.getCities();
    this.regions = this.mentalHealthService.getRegions();
    this.languages = this.mentalHealthService.getLanguages();
    this.genders = this.mentalHealthService.getGenders();
    this.mentalHealthService.getFacilityList().subscribe(data => {
      this.facilityList = data;
    });

    this.testArray = this.mentalHealthService.getAdvancedSearchQuery();

    if (this.testArray.length == 0) {
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
          gender: this.testArray[0].gender,
          formLanguage: this.testArray[0].formLanguage,
          region: this.testArray[0].region,
          age: this.testArray[0].age,
          condition: this.testArray[0].condition,
          theraApporaceh: this.testArray[0].theraApproach,
          mode: this.testArray[0].mode,
          city: this.testArray[0].city,
          other: this.testArray[0].other,
          cspIndicator: this.testArray[0].cspIndicator,
          medicareIndicator:this.testArray[0].medicareIndicator,
          badgercareIndicator: this.testArray[0].badgercareIndicator,
          prescribingProvider: this.testArray[0].prescribingProvider,
          acceptingNewPatients: this.testArray[0].acceptingNewPatients,
          facilityId: this.testArray[0].facilityId

      });
      //this.advancedSearchForm.setValue({
      //  'gender': [],
      //  'formLanguage': [],
      //  'region': [],
      //  'age': [],
      //  'condition': [],
      //  'theraApproach': [],
      //  'mode': [],
      //  'city': [],
      //  'other': [],
      //  'cspIndicator': true,
      //  'medicareIndicator': true,
      //  'badgercareIndicator': true,
      //  'prescribingProvider': true,
      //  'acceptingNewPatients': true,
      //  'facilityID': []
      //})
    }

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

  ngAfterViewInit() {

  }


  filter(val: string): Observable<any[]> {
    return this.mentalHealthService.getFacilityList()
      .pipe(
      map(response => response.filter(option => {
        return option.facilityName.toLowerCase().indexOf(val.toLowerCase()) >= 0
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
    //this.provider = provRelationship.id;
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
    var searchObject = [];
    this.mentalHealthService.saveAdvancedSearchQuery(form);
    //Build all array key values
    searchObject.push({ key: "Gender", value: form.gender });
    searchObject.push({ key: "Language", value: form.formLanguage });
    searchObject.push({ key: "Region", value: form.region });
    searchObject.push({ key: "BHAttributeSet", value: form.age });
    searchObject.push({ key: "BHAttributeSet", value: form.condition });
    searchObject.push({ key: "BHAttributeSet", value: form.theraApproach });
    searchObject.push({ key: "BHAttributeSet", value: form.mode });
   
    searchObject.push({ key: "BHAttributeSet", value: form.other });
    searchObject.push({ key: "FacilityID", value: form.facilityID })



    var city = new Array(form.city);
    searchObject.push({ key: "City", value: city });
    //Build CSP Indicator Key Value
    var i = form.cspIndicator ? 1 : null;
    var cspIndicatorArray = [];
    cspIndicatorArray.push(i);
    searchObject.push({ key: "CSP", value: cspIndicatorArray });


    //Build Badgercare Indicator Key Value
    var i = form.badgercareIndicator ? 1 : null;
    var badgercareIndicatorArray = [];
    badgercareIndicatorArray.push(i);
    // searchObject.push({ key: "MedicaidIndicator", value: badgercareIndicatorArray });

    //Build Medicare Indicator Key Value
    var i = form.medicareIndicator ? 1 : null;
    var medicareIndicatorArray = [];
    medicareIndicatorArray.push(i);
    // searchObject.push({ key: "MedicareIndicator", value: medicareIndicatorArray });


  
    //Build Prescribing Provider Key Value
    var i = form.acceptingNewPatients ? 1 : null;
    var prescriberArray = [];
    prescriberArray.push(i);
    //searchObject.push({ key: "Prescriber", value: prescriberArray });


    //Build Accepting New Patients
    var i = form.acceptigNewPatients ? 1 : null;
    var acceptigNewPatientsArray = [];

    acceptigNewPatientsArray.push(i);
    // searchObject.push({ key: "AcceptingNewPatient", value: acceptigNewPatientsArray });

    //searchObject.filter(x => x != null)
    searchObject = searchObject.filter(x => x.value != null && x.value.length > 0)

    this.mentalHealthService.advancedSearch(searchObject).subscribe(results => {
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
      //  category.push(this.provConditionsList);
      }


      this.dataSource = new MatTableDataSource<any>(this.newResults);


        //var providerTherapeuticTreatments = results.behavioralHealthAttributes.filter(function (item) {
        //  return item.bhSpecialtyType = 4;
        //});
     
      


    
      this.mentalHealthService.insertSearchResults(results);
      this.dataSource.paginator = this.paginator;
      if (results.length == 0) {
        this.message = 'No Results were found.';
      }
      else {
        this.message = '';

      }
    });
    //  console.log(searchObject);
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


  //providerSearch(term: string) {

  //  this.loading = true;
  //  this.results == [];
  //  this.mentalHealthService.searchEntries(term)
  //    .subscribe(results => {
  //      this.nav.resetIDs();
  //      this.mentalHealthService.resetBaseData();
  //      this.results = results;
  //      this.mentalHealthService.insertSearchResults(results);
  //      this.nav.resetIDs();
  //      this.loading = false;
  //      this.facilityProviderRelationships = results.facilityProviderRelationships;
  //      this.dataSource = new MatTableDataSource(this.provider);

  //      if (results.facilities.length == 0 && results.facilityProviderRelationships.length == 0 && results.providers.length == 0) {
  //        this.message = 'No Results were found.';
  //      }
  //      else {
  //        this.message = '';

  //      }
  //    });
  //}
}
