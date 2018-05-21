import { Injectable, Inject } from '@angular/core';
import { Http, Response, RequestOptions, Headers } from '@angular/http';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';
import 'rxjs/add/operator/switchMap';
import 'rxjs/add/observable/from';
import 'rxjs/add/operator/toPromise';
import { Language } from '../models/language';
import { LogService,LogLevel } from './log.service';
import { Facility } from '../models/facility';

@Injectable()


export class MentalHealthService {

  therapeuticApproaches: any;
  others: any;
  modes: any;
  conditions: any;
  ages: any;
  cities: { value: string; viewValue: string; }[];
  regions: { value: string; viewValue: string; }[];
  genders: { value: string; viewValue: string; }[];
  languages: { value: string; viewValue: string; }[];


  handleError: any;
  forecasts: any;
  private server_url: string = 'api/home';
  queryUrl: string = '/SearchForValue/';

  serviceData: any = [];
  results = [];
  loading = false;
  facilityProviderRelationship: any = [];
  facility: any = [];
  provider: any = [];
  advancedSearchQuery: any = [];
  items: Array<any>;
  searchResults: any = [];
  advancedSearchResults: any = [];
  states: any = [];
  constructor(public http: Http, public httpClient: HttpClient, private logger: LogService) {

    this.setStates();
    this.setCities();
    this.setGenders();
    this.setLanguages();
    this.setRegions();

  }

  private headers = new Headers({ 'Content-Type': 'application/json' });

    testLog()
    {
      this.logger.level = LogLevel.Off;
      this.logger.log("Test that log method");
    }
  saveAdvancedSearchQuery(form) {
    this.advancedSearchQuery = [];
    this.advancedSearchQuery.push(form);
  }

  getAdvancedSearchQuery() {
    return this.advancedSearchQuery;
  }

  insertFacilityProviderRelationshipData(facilityRel) {
    this.facilityProviderRelationship = [];
    this.facilityProviderRelationship.push(facilityRel);
  }
  updateFacilityProviderRelationshipData(provider) {
    // this.facilityProviderRelationship = [];
    this.facilityProviderRelationship[0].provider.push(provider);
  }

  getFacilityProviderRelationshipData() {
    return this.facilityProviderRelationship;
  }

  insertProviderData(providerData) {
    this.provider.push(providerData);
  }

  getProviderData() {
    return this.provider;
  }

  insertFacilityData(facility) {
    this.facility.push(facility);
  }

  getFacilityData() {
    return this.facility;
  }

  insertSearchResults(results) {
    this.searchResults = [];
    this.searchResults.push(results);
  }

  getSearchResults() {
    return this.searchResults;
  }

  insertAdvancedSearchResults(results) {
    this.advancedSearchResults = [];
    this.advancedSearchResults.push(results);

  }

  getAdvancedSearchResults() {
    return this.advancedSearchResults;
  }


  resetBaseData() {
    this.facility = [];
    this.provider = [];
    this.facilityProviderRelationship = [];
  }


  private extractData(res) {

    if (res.status < 200 || res.status >= 300) {
      throw new Error('Bad response status: ' + res.status);
    }
    // console.log(res.json());
    this.serviceData = (res.json());
    return this.serviceData || {};
  }

  private insertData(res) {

    if (res.status < 200 || res.status >= 300) {
      throw new Error('Bad response status: ' + res.status);
    }
    // console.log(res.json());
    this.serviceData = (res.json());
    return this.serviceData || {};
  }


  getProvider(id: number): Observable<any> {
    this.logger.level = LogLevel.Off;
      this.logger.log("Test that log method");
    return this.http.get(this.server_url + '/getproviderbyid/' + id)
      .map(this.extractData);
  }

  getProviderRelationshipById(id: number): Observable<any> {
    return this.http.get(this.server_url + '/getProviderRelationshipById/' + id)
      .map(this.extractData);
  }

  GetRelationshipDataByProviderID(providerID: number, relationshipID: number): Observable<any> {
    return this.http.get(this.server_url + '/GetRelationshipDataByProviderID/' + providerID +'/'+ relationshipID)
      .map(this.extractData);
  }

  GetRelationshipDataByFacilityID(facilityID: number, relationshipID: number): Observable<any> {
    return this.http.get(this.server_url + '/GetRelationshipDataByFacilityID/' + facilityID + '/' + relationshipID)
      .map(this.extractData);
  }

  getFacility(id: number): Observable<any> {
    return this.http.get(this.server_url + '/GetFacilityById/' + id)
      .map(this.extractData);
  }

  getFacilityList(): Observable<any> {
    return this.httpClient.get(this.server_url + '/getFacilityList');
  }

  getProviderList(): Observable<any> {
    return this.httpClient.get(this.server_url + '/getProviderList');
  }

  getBehavioralHealthAttributeByID(id): Observable<any> {
    return this.http.get(this.server_url + '/GetBehavioralHealthAttributeByID/' + id)
      .map(this.extractData);
  }

  getCredentialList(): Observable<any> {
    return this.httpClient.get(this.server_url + '/getCredentialList');
  }
  getCredentialListByProvID(id): Observable<any> {
    return this.http.get(this.server_url + '/getCredentialListByProviderId/' + id)
      .map(this.extractData);
  }
 
  getFacilityProviderRelationshipById(id: number): Observable<any> {
    return this.http.get(this.server_url + '/getfacilityproviderrelationshipbyid/' + id)
      .map(this.extractData);
  }


  getLanguages(): Observable<any> {
    return this.httpClient.get(this.server_url + '/getLanguageList');
  }

  //TO DO
  getVendor(id: number): Observable<any> {
    return this.http.get(this.server_url + '/getvendorbyid/' + id)
      .map(this.extractData);
  }

  //POSTS
  createAddress(body) {
    const url = `${this.server_url}/createAddress/`;
    this.httpClient.post(url, body).subscribe(
      body => {
        console.log("Address Create was successful", body);
      },
      error => {
        console.log("Error", error);
      }
    );
  }

  createProvider(body) {
    const url = `${this.server_url}/createProvider/`;
    return this.httpClient.post(url, body).map((res: any) => {
      let data = res;
      return data;
    }).catch(this.handleError)
  }

  updateProvider(body) {
    const url = `${this.server_url}/updateProvider/`;
    return this.httpClient.post(url, body).map((res: any) => {
      let data = res;
      return data;
    }).catch(this.handleError)
  }

  updateCredentials(body, id) {
    const url = `${this.server_url}/updateCredentials/` + id;
    return this.httpClient.post(url, body).map((res: any) => {
      let data = res;
      return data;
    }).catch(this.handleError)
  }

  updateLanguage(body, id) {
    const url = `${this.server_url}/updateLanguage/` + id;
    return this.httpClient.put(url, body).map((res: any) => {
      let data = res;
      return data;
    }).catch(this.handleError)
  }

  updateBHAttribute(body, id) {
    const url = `${this.server_url}/UpdateBhAttributes/` + id;
    return this.httpClient.put(url, body).map((res: any) => {
      let data = res;
      return data;
    }).catch(this.handleError)
  }


  createFacility(body) {
    const url = `${this.server_url}/createFacility/`;
 
    return this.httpClient.post(url, body).map((res: any) => {
      let data = res;
      return data;
    }).catch(this.handleError)
  }


  updateFacility(body) {
    const url = `${this.server_url}/updateFacility/`;
    return this.httpClient.post(url, body).map((res: any) => {
      let data = res;
      return data;
    }).catch(this.handleError)
  }

  //TODO mapAddressToFacility(facilityID, addressID)
  //TODO  mapAddressToVendor (vendorID, addressID)
  //TODO  mapFacilityToVendor(facilityID,vendorID )

  MapProviderToFacility(providerID: number, facilityID): Observable<any> {
    return this.http.get(this.server_url + '/MapProviderToFacility/' + providerID +'/'+ facilityID)
      .map(this.extractData);
  }

  updateFacilityProviderRelationship(body) {
    const url = `${this.server_url}/updateFacilityProviderRelationship/`;
    return this.httpClient.post(url, body).map((res: any) => {
      let data = res;
      return data;
    }).catch(this.handleError)
  }

  searchEntries(term) {
    return this.http
      .get((this.server_url + '/SearchForValues/' + term))
      .map(this.extractData);
  }

  advancedSearch(body: Object): Observable<any> {
    const url = `${this.server_url}/AdvancedSearchMethod/`;
    return this.http
      .post(url, JSON.stringify(body), { headers: this.headers })
      .map(this.extractData)
      //.then(this.extractData)
      .catch(this.handleError);
  }



  AllFacilityProviderRelationships() {
    return this.http
      .get((this.server_url + '/AllFacilityProviderRelationships'))
      .map(this.extractData);
  }

  searchNew(term: string) {
    let promise = new Promise((resolve, reject) => {
      let apiURL = this.server_url + '/SearchForValues/' + term;
      this.http.get(apiURL)
        .toPromise()
        .then(
        res => { // Success

          this.results = res.json().results;
          resolve();
        },
        msg => { // Error
          reject(msg);
        }
        );
    });
    return promise;
  }



  //TO DO
  //updateFacility(facility): Observable<any> {
  //  return this.http.put(this.server_url + '/updatevendor')
  //    .map(this.extractData);
  //} 

  //TO DO
  updateVendor(body: Object): Promise<Object> {
    const url = `${this.server_url}/updateVendor/`;
    return this.http
      .post(url, JSON.stringify(body), { headers: this.headers })
      .toPromise()
      .then(() => body)
      .catch(this.handleError);
  }

  createVendor(body: Object): Promise<Object> {
    const url = `${this.server_url}/createVendor/`;
    return this.http
      .post(url, JSON.stringify(body), { headers: this.headers })
      .toPromise()
      .then(() => body)
      .catch(this.handleError);
  }


  //TO DO
  linkToProvider(): Observable<any> {
    return this.http.get(this.server_url + '/updatevendor')
      .map(this.extractData);
  }

  //TO DO
  //linkToVendor(): Observable<any> {
  //  return this.http.put(this.server_url + '/updatevendor')
  //    .map(this.extractData);
  //} 




  setStates() {
    this.states = ['AL', 'AK', 'AS', 'AZ', 'AR', 'CA', 'CO', 'CT', 'DE', 'DC', 'FM',
      'FL', 'GA', 'GU', 'HI', 'ID', 'IL', 'IN', 'IA', 'KS', 'KY', 'LA',
      'ME', 'MH', 'MD', 'MA', 'MI', 'MN', 'MS', 'MO', 'MT', 'NE', 'NV',
      'NH', 'NJ', 'NM', 'NY', 'NC', 'ND', 'MP', 'OH', 'OK', 'OR', 'PW',
      'PA', 'PR', 'RI', 'SC', 'SD', 'TN', 'TX', 'UT', 'VT', 'VI', 'VA',
      'WA', 'WV', 'WI', 'WY'];
  }


  setLanguages() {
    this.languages = [
      { value: '1', viewValue: 'English' },
      { value: '2', viewValue: 'Spanish' },
      { value: '3', viewValue: 'Hmong' }
    ];
  }

  setGenders() {
    this.genders = [
      { value: '1', viewValue: 'Female' },
      { value: '2', viewValue: 'Male' },
      { value: '3', viewValue: 'Unknown' },
    ];
  }
  setRegions() {
    this.regions = [
      { value: '1', viewValue: 'Far East' },
      { value: '2', viewValue: 'East' },
      { value: '3', viewValue: 'Far West' },
      { value: '4', viewValue: 'West' },
      { value: '4', viewValue: 'North' },
      { value: '4', viewValue: 'South' }
    ];
  }

  setCities() {
    this.cities = [
      { value: '1', viewValue: 'Madison' },
      { value: '2', viewValue: 'Sun Prairie' },
      { value: '3', viewValue: 'Middleton' },
      { value: '4', viewValue: 'Stoughton' },
      { value: '5', viewValue: 'DeForest' },
      { value: '6', viewValue: 'Verona' },
      { value: '7', viewValue: 'McFarland' },
      { value: '8', viewValue: 'Waunakee' }
    ];
  }

 
  getGenders() {
    return this.genders;
  }
  getRegions() {
    return this.regions;
  }
  getStates() {
    return this.states;
  }
  getCities() {
    return this.cities;
  }
  getAges() {
    return this.ages;
  }
  getModes() {
    return this.modes;
  }
  getOthers() {
    return this.others;
  }
  getTherapeuticApproaches() {
    return this.therapeuticApproaches;
  }
  getConditions() {
    return this.conditions;
  }
}
