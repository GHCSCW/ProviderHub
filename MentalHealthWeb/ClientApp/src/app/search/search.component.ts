import { Component, ViewChild, OnInit } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort } from '@angular/material';
import { Router } from "@angular/router";
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MentalHealthService } from "../services/mental-health.service";
import { Subject } from "rxjs/Subject";
import { NavbarService } from '../services/navbar.service';
import { AuthenticationService } from '../services/authentication.service';

@Component({
  selector: 'app-home',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})


export class SearchComponent implements OnInit {

  facilityRelationshipColumns = ['firstName', 'lastName', 'credentials', 'relationshipStatus','facilityName','region', 'address', 'city', 'zip', 'phoneNumber'];
  facilityColumns = ['facilityName', 'address', 'city', 'zip','phoneNumber'];
  providerColumns = ['firstName', 'lastName'];
  vendorColumns = ['vendorName'];

@ViewChild(MatPaginator) paginator: MatPaginator;
@ViewChild(MatSort) sort: MatSort;

results: any;
  searchTerm$ = new Subject<string>();
  loading: boolean = false;s
  showVar: boolean = true;
  facility: any;
  provider: any;
  facilityProviderRelationships: any;
  vendors: any;
  previousSearchResults: any[]
  searchForm: FormGroup;
  message: string;

  constructor(
    private mentalHealthService: MentalHealthService,
    private fb: FormBuilder, private router: Router,
    public nav: NavbarService,
    private authSvc: AuthenticationService
  ) {

    this.nav.hide();
    this.createForm();
    this.mentalHealthService.getSearchResults().map(results => {
      this.nav.resetIDs();

      this.provider = new MatTableDataSource<any>(results.providers);
      this.facility = new MatTableDataSource<any>(results.facilities);
      this.vendors = new MatTableDataSource<any>(results.vendors);
      this.facilityProviderRelationships = new MatTableDataSource<any>(results.facilityProviderRelationships);
    
      this.results = results;
    });
  }

  ngOnInit()
  {
  }

  ngAfterViewInit() {

    if (this.facilityProviderRelationships != null) {
      this.facilityProviderRelationships.sort = this.sort;
      this.facilityProviderRelationships.paginator = this.paginator;
    }
  }

  providerRelationshipRoute(provRelationship) {


    this.mentalHealthService.insertFacilityProviderRelationshipData(provRelationship);
    this.nav.addFacilityRelationshipProviderID(provRelationship);
    this.router.navigate(["/provider/facilityrel/" + provRelationship.relationshipID]);
  }

  providerRoute(provider) {
    this.mentalHealthService.insertProviderData(provider);
    this.nav.addProviderID(provider.id);
    this.router.navigate(["/provider/" + provider.id]);
  }

  providerRelationshipFacilityRoute(provRelationship) {
    this.mentalHealthService.insertFacilityProviderRelationshipData(provRelationship);
    this.nav.addFacilityRelationshipProviderID(provRelationship);
    this.router.navigate(["/facility/" + provRelationship.facility.id]);
  }

  facilityRoute(facility) {
    this.nav.addFacilityID(facility.id);
    this.mentalHealthService.insertFacilityData(facility);
    this.router.navigate(["/facility/"+ facility.id]);
  }

  vendorRoute(vendor) {
    this.nav.addVendorID(vendor.id);
    this.mentalHealthService.insertFacilityProviderRelationshipData(vendor);
    this.router.navigate(["/vendor/" + vendor.id]);
  }


  toggleChild() {
    this.showVar = !this.showVar;
  }

  createForm() {
    this.searchForm = this.fb.group({
      'text': ['', Validators.required] // <--- the FormControl called "name"
    });
  }


  providerSearch(term: string) {

    this.loading = true;
    this.results == [];
    this.mentalHealthService.searchEntries(term)
      .subscribe(results => {
        this.nav.resetIDs();
        this.mentalHealthService.resetBaseData();
        this.results = results;
        this.mentalHealthService.insertSearchResults(results);
        this.nav.resetIDs();
        this.loading = false;
        if (this.authSvc.canEdit == false) {
          //do not show inactive relationships to non editor roles. Also hide facilties and providers that don't have relationships. 
          results.facilityProviderRelationships = results.facilityProviderRelationships.filter(
            rel => rel.relationshipStatus == true
          )
          results.facilities = [];
          results.providers = [];
        }
        else {
          results.facilityProviderRelationships
        }
        
        this.facilityProviderRelationships = new MatTableDataSource<any>(results.facilityProviderRelationships);
        this.facility = new MatTableDataSource<any>(results.facilities);
        this.provider = new MatTableDataSource<any>(results.providers);
        this.vendors = new MatTableDataSource<any>(results.vendors);
        this.facilityProviderRelationships.paginator = this.paginator;
        this.facilityProviderRelationships.sort = this.sort;
        if (results.facilities.length == 0  && results.facilityProviderRelationships.length == 0 && results.providers.length == 0 )
        {
          this.message = 'No Results were found.';
        }
        else
        {
          this.message = '';

        }
      });
  }
}
