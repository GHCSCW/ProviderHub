import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { environment } from '../../environments/environment';
import { ProviderHubService } from '../app.service';
import { CommonModule, Location } from '@angular/common';
import { GenderPipe, NullablePipe, BoolPipe, SpecialtyTypePipe, ParentSpecialtyPipe } from '../pipes';
import * as $ from 'jquery';
import 'jquery-ui-bundle';

@Component({
  selector: 'app-provider',
  templateUrl: './provider.component.html',
  styleUrls: ['./provider.component.scss']
})
export class ProviderComponent implements OnInit {

  public apiRoot: string;
  public providerId: number;
  public initialTab: string = "";
  public Provider: any;
  public Service: any;
  public nav: string;

  constructor(private route: ActivatedRoute, private router: Router,
              private service: ProviderHubService, private location: Location) {
    this.Service = service; this.Provider = {};
    //"(Primary)" to mark primary credential takes up too much space for no benefit
    this.nav = 'Demographics'; //default tab should be Demographics
  }

  ngOnInit() {
    this.apiRoot = environment.apiRoot;
    this.route.params.subscribe(params => {
      this.providerId = +params['id'];
      if (params['tabURL']) { this.initialTab = params['tabURL']; } else {
        this.location.replaceState("/Provider/Demographics/" + this.providerId);
      }
    });
    var _dis = this; var toClick = null;
    //0. nav
    var navs = document.getElementById("provider-nav").getElementsByTagName("li");
    for (var i = 0; i < navs.length; i++) {
      navs[i].addEventListener("click", (function(event) {
        return function (e) {
          _dis.nav = this.getAttribute("tab-id");
          _dis.location.replaceState("/Provider/" + this.getAttribute("tab-url") + "/" + _dis.providerId);
        }
      })(_dis), false);
      if (navs[i].getAttribute("tab-url") == this.initialTab) { toClick = navs[i] as HTMLElement; }
    }
    if (this.initialTab != "") { toClick.click(); }
    
    this.service.hitAPI(this.apiRoot + "Provider/ByID/" + this.providerId).subscribe(
      data => {
        //1. Main and Demo
        this.Provider = data; var _c = this.Provider.CredentialListStr;
        this.Provider.LastUpdatedDate = this.Provider.LastUpdatedDate.replace(/\D/g, '');
        this.Provider.Credentials = _c.slice(0, -1).replace(/,/g,", ");//trailing comma
        document.getElementById("page-title").innerHTML = this.Provider.FirstName + " " + this.Provider.LastName + ", " + this.Provider.Credentials;
        //2. Specialties
        // Object spec (per Provider):
        //  ProviderSpecialties:[{Specialty},{Specialty},...]
        // Each Specialty = {ID,Name,Description,MappingID,SequenceNumber,CreatedDate,
        // CreatedBy,EffectiveDate,TerminationDate,SpecialtyType,ParentSpecialtyID,ParentName}
        for (var i = 0; i < this.Provider.ProviderSpecialties.length; i++) {
          var s = this.Provider.ProviderSpecialties[i];
          s.EffectiveDate = s.EffectiveDate.replace(/\D/g, '');
          s.TerminationDate = (s.TerminationDate == null)?'':s.TerminationDate.replace(/\D/g, '');
          console.log(s);
        }
        let list: any = $('#specList');
        list.sortable();
      }
    );
    //note: if navigated to from direct link, and not clicking a provider,
    //      it'll be empty til the provider object loads from AJAX...but that's okay
    document.getElementById("page-title").innerHTML = API.selectedProvider;
  }

  public onSpecClick(event: any) {
    $(event.target).parent().children("table.specTable").toggle();
  }

  public setSortable(event: any) {
    alert("set sortable!");
  }

}
