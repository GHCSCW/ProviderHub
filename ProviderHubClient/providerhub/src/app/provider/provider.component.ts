import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { ProviderHubService } from '../app.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-provider',
  templateUrl: './provider.component.html',
  styleUrls: ['./provider.component.scss']
})
export class ProviderComponent implements OnInit {

  public apiRoot: string;
  public providerId: number;
  public Provider: any;
  public Service: any;
  public nav: string;

  constructor(private route: ActivatedRoute, private router: Router,
              private service: ProviderHubService) {
    this.Service = service;
    //TEST PROVIDER FOR DEBUG
    this.Provider = {
      FirstName: "John", LastName: "Doe", Credentials: "MD, PhD, NCIS, CSI", NPI: 3913874644,
      Languages: "English, Spanish", ABMS: { has: "Yes", exp: "12/19/2024" },
      License: { no: 452567, exp: "12/19/2024" }, CertifyingBoard: { name: "USDA", exp: "12/19/2024" },
      FacilityNames: "UW, Meriter", LastUpdatedBy:"GHC-HMO\\spillai", LastUpdatedDate:"Jun 18, 2018"
    };
    //"(Primary)" to mark primary credential takes up too much space for no benefit
    this.nav = 'Specialties'; //default tab is Demographics
  }

  ngOnInit() {
    this.route.data.subscribe(v => this.apiRoot = v.apiRoot);
    console.log(this.apiRoot);
    this.route.params.subscribe(params => { this.providerId = +params['id']; });
    console.log(this.providerId); var _dis = this;
    //0. nav
    var navs = document.getElementById("provider-nav").getElementsByTagName("li");
    for (var i = 0; i < navs.length; i++) {
      navs[i].addEventListener("click", (function(event) {
        return function (e) { _dis.nav = this.getAttribute("tab-id"); }
      })(_dis),false);
    }
    
    this.service.hitAPI(this.apiRoot + "Provider/ByID/" + this.providerId).subscribe(
      data => {
        //1. Main and Demo
        this.Provider = data; var _c = this.Provider.CredentialListStr;
        this.Provider.LastUpdatedDate = this.Provider.LastUpdatedDate.replace(/\D/g, '');
        this.Provider.Credentials = _c.slice(0, -1);//trailing comma
        document.getElementById("page-title").innerHTML = this.Provider.FirstName + " " + this.Provider.LastName;
        //2. Specialties
        // Object spec (per Provider):
        //  ProviderSpecialties:[{Specialty},{Specialty},...]
        // Each Specialty = {ID,Name,Description,MappingID,SequenceNumber,CreatedDate,
        // CreatedBy,EffectiveDate,TerminationDate,SpecialtyType,ParentSpecialtyID,ParentName}
        for (var i = 0; i < this.Provider.ProviderSpecialties.length; i++) {
          var s = this.Provider.ProviderSpecialties[i];
          s.EffectiveDate = s.EffectiveDate.replace(/\D/g, '');
          s.TerminationDate = s.TerminationDate.replace(/\D/g, '');
        }
      }
    );
    //note: if navigated to from direct link, and not clicking a provider,
    //      it'll be empty til the provider object loads from AJAX...but that's okay
    document.getElementById("page-title").innerHTML = API.selectedProvider;//faster than jQ or Ang
  }

}
