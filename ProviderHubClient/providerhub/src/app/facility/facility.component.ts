import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { ProviderHubService } from '../app.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-facility',
  templateUrl: './facility.component.html',
  styleUrls: ['./facility.component.scss']
})
export class FacilityComponent implements OnInit {

  public apiRoot: string;
  public facilityId: number;
  public Facility: any;
  public FacilityAddress: any;
  public Service: any;
  public nav: string;

  constructor(private route: ActivatedRoute, private router: Router,
              private service: ProviderHubService) {
    this.Service = service; this.Facility = {};
    //TEST FACILITY FOR DEBUG
    this.nav = 'Demographics';//default=Demographics, change to test
  }

  ngOnInit() {
    this.route.data.subscribe(v => this.apiRoot = v.apiRoot); var _dis = this;
    this.route.params.subscribe(params => { this.facilityId = +params['id']; });
    var navs = document.getElementById("facility-nav").getElementsByTagName("li");
    for (var i = 0; i < navs.length; i++) {
      navs[i].addEventListener("click", (function (event) {
        return function (e) { _dis.nav = this.getAttribute("tab-id"); }
      })(_dis), false);
    }
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
          s.TerminationDate = s.TerminationDate.replace(/\D/g, '');
        }
      }
    );
    document.getElementById("page-title").innerHTML = API.selectedFacility;
  }

}
