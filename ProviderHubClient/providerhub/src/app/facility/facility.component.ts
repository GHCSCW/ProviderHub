import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { environment } from '../../environments/environment';
import { ProviderHubService } from '../app.service';
import { CommonModule, Location } from '@angular/common';
import { GenderPipe, NullablePipe, BoolPipe, SpecialtyTypePipe, ParentSpecialtyPipe } from '../pipes';

@Component({
  selector: 'app-facility',
  templateUrl: './facility.component.html',
  styleUrls: ['./facility.component.scss']
})
export class FacilityComponent implements OnInit {

  public apiRoot: string;
  public facilityId: number;
  public initialTab: string = "";
  public Facility: any;
  public FacilityAddress: any;
  public Service: any;
  public nav: string;

  constructor(private route: ActivatedRoute, private router: Router,
              private service: ProviderHubService, private location: Location) {
    this.Service = service; this.Facility = {};
    //TEST FACILITY FOR DEBUG
    this.nav = 'Demographics';//default=Demographics, change to test
  }

  ngOnInit() {
    this.apiRoot = environment.apiRoot; var _dis = this; var toClick = null;
    this.route.params.subscribe(params => {
      this.facilityId = +params['id'];
      if (params['tabURL']) { this.initialTab = params['tabURL']; } else {
        this.location.replaceState("/Facility/Demographics/" + this.facilityId);
      }
    });
    var navs = document.getElementById("facility-nav").getElementsByTagName("li");
    for (var i = 0; i < navs.length; i++) {
      navs[i].addEventListener("click", (function (event) {
        return function (e) {
          _dis.nav = this.getAttribute("tab-id");
          _dis.location.replaceState("/Facility/" + this.getAttribute("tab-url") + "/" + _dis.facilityId);
        }
      })(_dis), false);
      if (navs[i].getAttribute("tab-url") == this.initialTab) { toClick = navs[i] as HTMLElement; }
    }
    if (this.initialTab != "") { toClick.click(); }

    this.service.hitAPI(this.apiRoot + "Facility/ByID/" + this.facilityId).subscribe(
      data => {
        this.Facility = data; this.FacilityAddress = this.Facility.FacilityAddress;
        document.getElementById("page-title").innerHTML = this.Facility.FacilityName;
        this.FacilityAddress.AddressLine1 = (this.FacilityAddress.AddressLine1 == null) ? "" : this.FacilityAddress.AddressLine1;
        this.FacilityAddress.AddressLine2 = (this.FacilityAddress.AddressLine2 == null) ? "" : this.FacilityAddress.AddressLine2;
        //specs
        for (var i = 0; i < this.Facility.FacilitySpecialties.length; i++) {
          var s = this.Facility.FacilitySpecialties[i];
          s.EffectiveDate = s.EffectiveDate.replace(/\D/g, '');
          s.TerminationDate = (s.TerminationDate==null)? '' : s.TerminationDate.replace(/\D/g, '');
        }
        //providers
        for (var i = 0; i < this.Facility.FacilityProviders.length; i++) {
          var _c = this.Facility.FacilityProviders[i].CredentialListStr;
          this.Facility.FacilityProviders[i].Credentials = _c.slice(0, -1);
        }
      }
    );
    document.getElementById("page-title").innerHTML = API.selectedFacility;
  }

}
