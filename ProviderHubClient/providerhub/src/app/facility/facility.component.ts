import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { environment } from '../../environments/environment';
import { ProviderHubService } from '../app.service';
import { CommonModule, Location } from '@angular/common';
import { GenderPipe, NullablePipe, BoolPipe, SpecialtyTypePipe, ParentSpecialtyPipe, NoValuePipe, PHDatePipe } from '../pipes';
import * as $ from 'jquery';
import 'datatables.net';
import 'datatables.net-bs4';
import 'datatables.net-select-bs4';
import 'jquery-ui-bundle';

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
  public providerDT: any;

  constructor(private route: ActivatedRoute, private router: Router,
    private service: ProviderHubService, private location: Location) {
    this.Service = service; this.Facility = {}; this.FacilityAddress = {};
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
          if (_dis.nav != 'Providers') { $("#providersTableDT_wrapper").hide(); } else { $("#providersTableDT_wrapper").show(); } /*cant do in Angular since generated by DT*/
        }
      })(_dis), false);
      if (navs[i].getAttribute("tab-url") == this.initialTab) { toClick = navs[i] as HTMLElement; }
    }
    if (this.initialTab != "") { toClick.click(); }

    this.service.hitAPI(this.apiRoot + "Facility/ByID/" + this.facilityId).subscribe(
      data => {
        this.Facility = data; this.FacilityAddress = this.Facility.FacilityAddress;
        document.getElementById("page-title").innerHTML = this.Facility.FacilityName;
        this.Facility.LastUpdatedDate = new PHDatePipe().transform(this.Facility.LastUpdatedDate);
        this.FacilityAddress.AddressLine1 = (this.FacilityAddress.AddressLine1 == null) ? "" : this.FacilityAddress.AddressLine1;
        this.FacilityAddress.AddressLine2 = (this.FacilityAddress.AddressLine2 == null) ? "" : this.FacilityAddress.AddressLine2;
        //1. specs
        for (var i = 0; i < this.Facility.FacilitySpecialties.length; i++) {
          var s = this.Facility.FacilitySpecialties[i];
          s.EffectiveDate = s.EffectiveDate.replace(/\D/g, '');
          s.TerminationDate = (s.TerminationDate == null) ? '' : s.TerminationDate.replace(/\D/g, '');
          s.LastUpdatedDate = (s.LastUpdatedDate == null) ? '' : s.LastUpdatedDate.replace(/\D/g, '');
        }
        let list: any = $('#specList');
        list.sortable();
        //2. providers
        for (var i = 0; i < this.Facility.FacilityProviders.length; i++) {
          var _c = this.Facility.FacilityProviders[i].CredentialListStr;
          this.Facility.FacilityProviders[i].Credentials = _c.slice(0, -1).replace(/,/g,", ");
        }
        let providersDTID: any = $('#providersTableDT');
        this.providerDT = providersDTID.DataTable({
          select: true,
          paging: false,
          language: { search: "", searchPlaceholder: "Begin typing in a Provider Name or NPI to filter search results" },
          data: this.Facility.FacilityProviders,
          columns: [{ data: null, orderable: false,searchable:false,defaultContent:''},
          { data: "NPI" }, { data: "LastName" }, { data: "FirstName" },
          { data: null, render: function (data, type, row) { var d = data.CredentialListStr; return (d == null) ? "" : d.slice(0, -1).replace(/,/g, ", "); }, searchable: false },
          { data: null, render: function (data, type, row) { var d = data; var r; switch (d.Gender) { case 1: r = "Female"; break; case 2: r = "Male"; break; default: r = "Other"; break; } return r; }, searchable: false },
          { data: null, render: function (data, type, row) { var d = data.PrimarySpecialty; return d; }, searchable: false },
          { data: null, render: function (data, type, row) { var d = data; return ""; }, searchable: false }
          ],
          order: [[1, "asc"]],
          rowId: 'ID',
          initComplete: function (settings, json) {
            if (_dis.nav != 'Providers') { $("#providersTableDT_wrapper").hide(); } else { $("#providersTableDT_wrapper").show(); } /*cant do in Angular since generated by DT*/
          }
        });
        this.providerDT.on('select',
          (e, dt, type, indexes) => {
            console.log(this.providerDT.rows(indexes).data().pluck("ID"));
            this.onRowSelect(this.providerDT.rows(indexes).data().pluck("ID"));
          }
        );
        this.providerDT.on('deselect',
          (e, dt, type, indexes) => {
            console.log(this.providerDT.rows(indexes).data().pluck("ID"));
            this.onRowSelect(this.providerDT.rows(indexes).data().pluck("ID"));
          }
        );
        //3. Additional properties for UI conditionals ('novalue' pipe doesn't work??)
        this.FacilityAddress.HidePhoneExtension = (this.FacilityAddress.PhoneExtension == null || this.FacilityAddress.PhoneExtension == '');
        this.FacilityAddress.HideAlternatePhoneNumber = (this.FacilityAddress.AlternatePhoneNumber == null || this.FacilityAddress.AlternatePhoneNumber == '');
        /*this.providerDT.on('search.dt', () => {
          document.getElementById('providersTableDT').getElementsByTagName('tbody')[0].style.visibility = (document.getElementById('providersTableDT_filter').getElementsByTagName('input')[0].value.length < 2) ? "hidden" : "visible";
        });
        document.getElementById('providersTableDT').getElementsByTagName('tbody')[0].style.visibility = (document.getElementById('providersTableDT_filter').getElementsByTagName('input')[0].value.length < 2) ? "hidden" : "visible";*/
      }
    );
    document.getElementById("page-title").innerHTML = API.selectedFacility;
  }
  public onSpecClick(event: any) {
    $(event.target).parent().children("table.specTable").toggle();
    $(event.target).parent().parent().children(".provSpecFooter,.provFacFooter").toggle();
  }
  private onRowSelect(indexes: number[]): void {
    var providerId = indexes[0];
    console.log(providerId);
    console.log($("tr#" + providerId));
    var row = this.providerDT.row($("tr#" + providerId)[0]);
    if (row.child.isShown()) {
      row.child.hide();
    } else {
      row.child(format(row.data()),'expandedFP').show();
    }
    function format(d) {
      return "<table class='plainjane'>" + "<tr><td><a href='../provider/"+d.ID+"'>View Provider</a></td>"
        + "<td>External Provider <br/>" + new BoolPipe().transform(d.FPRelationship.ExternalProviderIndicator) + "</td>"
        + "<td>Accepting New Patients <br/>" + new BoolPipe().transform(d.FPRelationship.AcceptingNewPatientIndicator) + "</td>"
        + "<td>Prescriber <br/>" + new BoolPipe().transform(d.FPRelationship.PrescriberIndicator) + "</td>"
        + "<td>Referral <br/>" + new BoolPipe().transform(d.FPRelationship.ReferralIndicator) + "</td>"
        + "<td>PCP Eligible <br/>" + new BoolPipe().transform(d.FPRelationship.PCPEligibleIndicator) + "</td>"
        + "<td>Float Provider <br/>" + new BoolPipe().transform(d.FPRelationship.FloatProviderIndicator) + "</td></tr>";
    }
  }

}
