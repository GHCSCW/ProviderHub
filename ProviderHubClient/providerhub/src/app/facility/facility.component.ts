import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { environment } from '../../environments/environment';
import { ProviderHubService } from '../app.service';
import { CommonModule, Location } from '@angular/common';
import { GenderPipe, NullablePipe, BoolPipe, SpecialtyTypePipe, ParentSpecialtyPipe, NoValuePipe, PHDatePipe, PhoneToDBPipe, PhoneFromDBPipe, TermStatusPipe } from '../pipes';
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
  public specsEdited: boolean = false;
  public editingDivWrappers: any;
  public editingDivHeaderWrappers: any;
  public editingHeaderDivs: any;
  public _specsList: any;
  public origSpecOrder: any;
  public currentSpecOrder: any;
  public environment: any;

  constructor(private route: ActivatedRoute, private router: Router,
    private service: ProviderHubService, private location: Location) {
    this.Service = service; this.Facility = {}; this.FacilityAddress = {};
    //TEST FACILITY FOR DEBUG
    this.nav = 'Demographics';//default=Demographics, change to test
    this.specsEdited = false;
  }

  public canEdit() {
    //SKP: Don't want to waste time: ask M$ why the fuck we have to make (or use) a custom JSON boolean serializer class, and it spits out "True" and "False" by default.
    return (environment.authUser.isSuperUser == 'True' || environment.authUser.isEditor == 'True');
  }

  ngOnInit() {
    this.apiRoot = environment.apiRoot; this.environment = environment; var _dis = this; var toClick = null;
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

    this.editingDivWrappers = ['#facility-demo', '#demoTable'];
    this.editingDivHeaderWrappers = ['#facility-main-header', '#demo-card .card-title'];
    this.editingHeaderDivs = [document.getElementById("facility-main-header"), document.getElementById("demo-card").getElementsByClassName('card-title')[0]];

    //0=PROVIDER HEADER EDIT, 1=PROVIDER DEMO EDIT
    for (var i = 0; i < this.editingHeaderDivs.length; i++) {
      (function () {
        var divs = _dis.editingHeaderDivs[i]; var _i = i;
        divs.getElementsByClassName("not-editing")[0].addEventListener("click", function (event) { //edit
          if (_dis.canEdit()) { _dis.editFacility(_i, event); }
        });
        divs.getElementsByClassName("is-editing")[0].addEventListener("click", function (event) { //save
          if (_dis.canEdit()) { _dis.saveFacility(_i, event); }
        });
        divs.getElementsByClassName("is-editing")[1].addEventListener("click", function (event) { //cancel
          if (_dis.canEdit()) { _dis.cancelEdit(_i, event); }
        });
      })();
    }

    //POTENTIAL OPTIMIZE, but makes code harder to read: convert to plain jane JS (like above click events), might save 100-300ms
    let _addSpec: any = $("#addSpec"); let _resetSpec: any = $("#resetSpecs"); let _saveSpec: any = $("#saveSpecs"); let _addSpecBody: any = $("#addSpecBody");
    _addSpec.click(function () {
      if (_dis.canEdit()) {
        _addSpecBody.show(); //_addSpec.hide(); _resetSpec.show(); _saveSpec.show();
        _dis.specsEdited = true;
      }
    });
    _saveSpec.click(function (e) {
      if (_dis.canEdit()) {
        var forIDs = _dis.Facility.FacilitySpecialties[0];
        _dis.saveFacility(2, e, forIDs.ID, forIDs.MappingID);
      }
    });
    _resetSpec.click(function () {
      if (_dis.canEdit()) {
        _addSpecBody.hide(); //_addSpec.show(); _resetSpec.hide(); _saveSpec.hide();
        _dis.specsEdited = false;
        let _unsavedSpec: any = $(".unsavedSpec");
        _unsavedSpec.remove();
        if (_dis.origSpecOrder != null) {
          var specIDs = _dis.origSpecOrder.split("|");
          //remove() all into memory to re-add?
          for (var i = 0; i < specIDs.length; i++) {
            var _specid = specIDs[i];
            if (_specid.trim() != "") {
              let _specWrapper: any = $("#specWrapper_" + _specid);
              //use prepend/postpend to reorder specs
              //$("h2").insertAfter($(".container"));
              if (i == 0) { //< specIDs.length - 1
                var _relation: any = $("#addSpecArea");//$("#specWrapper_" + specIDs[i + 1]);
                _specWrapper.insertAfter(_relation);
              } else {
                var _relation: any = $("#specWrapper_" + specIDs[i - 1]);//$("#specWrapper_" + specIDs[i - 1]);
                _specWrapper.insertAfter(_relation);
              }
            }
          }
        }
      }
    });

    this.service.hitAPI(this.apiRoot + "Facility/ByID/" + this.facilityId).subscribe(
      data => {
        //0. Edit/Save buttons and Misc UI
        for (var i = 0; i < this.editingDivWrappers.length; i++) {
          let _editDivs: any = $(this.editingDivHeaderWrappers[i] + " i.is-editing," + this.editingDivWrappers[i] + " .is-editing"); _editDivs.hide(); _editDivs = null;
          let _notEditDivs: any = $(this.editingDivHeaderWrappers[i] + " i.not-editing," + this.editingDivWrappers[i] + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
        }
        //1. header and address
        this.Facility = data.f; this.FacilityAddress = this.Facility.FacilityAddress;
        document.getElementById("page-title").innerHTML = this.Facility.FacilityName;
        this.Facility.LastUpdatedDate = new PHDatePipe().transform(this.Facility.LastUpdatedDate.replace(/\D/g, '').slice(0,-4));
        this.FacilityAddress.AddressLine1 = (this.FacilityAddress.AddressLine1 == null) ? "" : this.FacilityAddress.AddressLine1;
        this.FacilityAddress.AddressLine2 = (this.FacilityAddress.AddressLine2 == null) ? "" : this.FacilityAddress.AddressLine2;
        //1a. selectize
        //0b. SpecsList
        let toSelectize: any = $("#add_Facility_Specialty"); let sSelect: any = $("#add_Facility_Specialty"); //separate vars because adding any new select tags would affect toSelectize but not sSelect
        let specsList: any = data.s; var sSelectHTML = ""; this._specsList = [];
        for (var i = 0; i < specsList.length; i++) {
          sSelectHTML += "<option value='" + specsList[i].ID + "'>" + specsList[i].Name + "</option>";
          var toAdd = specsList[i]; toAdd.SequenceNumber = this.Facility.FacilitySpecialties.length; toAdd.MappingID = 0; toAdd.ID = specsList[i].ID;
          toAdd.LastUpdatedBy = environment.authUser.username; toAdd.LastUpdatedDate = new Date();
          toAdd.EffectiveDate = "/Date(1451628000000-0600)/".replace(/\D/g, '').slice(0, -4); toAdd.TerminationDate = '';
          toAdd.Status = "ACTIVE"; toAdd.ParentName = ''; toAdd.ParentSpecialtyID = 0; this._specsList[toAdd.ID] = toAdd;
        }
        sSelect.html("<select>" + sSelectHTML + "</select>");
        //SELECTIZE ALL SELECTS, SEND VAR TO GARBAGE COLLECTOR. IF DRAG_DROP DOESNT WORK WITH SINGLE SELECTS, MOVE SPEC SELECTIZE TO ITS OWN INITIALIZATION
        toSelectize.selectize({ plugins: ['drag_drop'] }); toSelectize = null;
        //1b. specs
        this.origSpecOrder = ""; this.currentSpecOrder = "";
        for (var i = 0; i < this.Facility.FacilitySpecialties.length; i++) {
          var s = this.Facility.FacilitySpecialties[i];
          s.EffectiveDate = s.EffectiveDate.replace(/\D/g, '').slice(0, -4);
          s.TerminationDate = (s.TerminationDate == null) ? '' : s.TerminationDate.replace(/\D/g, '').slice(0, -4);
          s.LastUpdatedDate = (s.LastUpdatedDate == null) ? '' : s.LastUpdatedDate.replace(/\D/g, '').slice(0, -4);
          this.origSpecOrder += s.ID + "|";
          this.currentSpecOrder += s.ID + ",";
        }
        if (this.origSpecOrder != "") { this.origSpecOrder = this.origSpecOrder.slice(0, -1); }
        if (this.currentSpecOrder != "") { this.currentSpecOrder = this.currentSpecOrder.slice(0, -1); }
        let list: any = $('#specList');
        list.sortable({
          update: function (event, ui) {
            _dis.specsEdited = true;
            let specs: any = $(".indivSpecWrapper"); //each one of these has an id="specWrapper_{{s.ID}}"
            _dis.currentSpecOrder = "";
            specs.each(function (this, index) {
              let spec: any = $(this);
              var _sid = spec.attr("id").replace("specWrapper_", "");//if you want you can make a custom attr and just do .attr("CUSTOM_ATTR_NAME");
              _dis.currentSpecOrder += _sid + ",";
            });
            if (_dis.currentSpecOrder != "") { _dis.currentSpecOrder = _dis.currentSpecOrder.slice(0, -1); }
            console.log(_dis.currentSpecOrder); console.log(_dis.origSpecOrder);
          }
        });
        let providersLink: any = $("#facility-nav li[tab-id='Providers']");
        providersLink.click(function () {
          if (typeof (Event) === 'function') { window.dispatchEvent(new Event('resize')); }
          else { var evt = window.document.createEvent('UIEvents'); evt.initUIEvent('resize', true, false, window, 0); window.dispatchEvent(evt); }
        });
        //2. providers
        for (var i = 0; i < this.Facility.FacilityProviders.length; i++) {
          var _c = this.Facility.FacilityProviders[i].CredentialListStr;
          this.Facility.FacilityProviders[i].Credentials = (_c==null)? "" : _c.slice(0, -1).replace(/,/g,", ");
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
          { data: null, render: function (data, type, row) { var d = data; var r; switch (d.Gender) { case 1: r = "Female"; break; case 2: r = "Male"; break; default: r = " "; break; } return r; }, searchable: false },
          { data: null, render: function (data, type, row) { var d = data.PrimarySpecialty; return d; }, searchable: false },
          { data: null, render: function (data, type, row) { var d = data; return ""; }, searchable: false }
          ],
          order: [[1, "asc"]],
          rowId: 'ID',
          initComplete: function (settings, json) {
            if (_dis.nav != 'Providers') { $("#providersTableDT_wrapper").hide(); } else { $("#providersTableDT_wrapper").show(); } /*cant do in Angular since generated by DT*/
            if (typeof (Event) === 'function') { window.dispatchEvent(new Event('resize')); }
            else { var evt = window.document.createEvent('UIEvents'); evt.initUIEvent('resize', true, false, window, 0); window.dispatchEvent(evt); }
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
        //2b. Vendor 
        // Facility-level properties to set from Vendor: VendorName ([0].VendorName) | VLastUpdatedBy ([0].LastUpdatedBy) | VLastUpdatedDate ([0].LastUpdatedDate)
        this.Facility.VendorName = this.Facility.VendorAddresses[0].VendorName;
        this.Facility.VID = this.Facility.VendorAddresses[0].VID;
        this.Facility.VLastUpdatedBy = this.Facility.VendorAddresses[0].LastUpdatedBy;
        this.Facility.VLastUpdatedDate = new PHDatePipe().transform(this.Facility.VendorAddresses[0].LastUpdatedDate.replace(/\D/g, '').slice(0, -4));
        // Any tweaking of vendor properties needed: mess with this.Facility.VendorAddresses[i] -- do for PhoneExtension and AlternatePhoneNumber
        //3. Additional properties for UI conditionals ('novalue' pipe doesn't work??)
        this.FacilityAddress.HidePhoneExtension = (this.FacilityAddress.PhoneExtension == null || this.FacilityAddress.PhoneExtension == '');
        this.FacilityAddress.HideAlternatePhoneNumber = (this.FacilityAddress.AlternatePhoneNumber == null || this.FacilityAddress.AlternatePhoneNumber == '');
        this.FacilityAddress.HideAltExtension = (this.FacilityAddress.AlternateExtension == null || this.FacilityAddress.AlternateExtension == '');
        /*this.providerDT.on('search.dt', () => {
          document.getElementById('providersTableDT').getElementsByTagName('tbody')[0].style.visibility = (document.getElementById('providersTableDT_filter').getElementsByTagName('input')[0].value.length < 2) ? "hidden" : "visible";
        });
        document.getElementById('providersTableDT').getElementsByTagName('tbody')[0].style.visibility = (document.getElementById('providersTableDT_filter').getElementsByTagName('input')[0].value.length < 2) ? "hidden" : "visible";*/
      }
    );
    document.getElementById("page-title").innerHTML = API.selectedFacility;
  }
  public transformDateForPHDB(datepicker_element_id: any) {
    var _e = document.getElementById(datepicker_element_id) as HTMLFormElement; var d = new Date(_e.value); var _jqe: any = $("#" + datepicker_element_id);
    var datestring = (_e.value == "" || _jqe.val().trim() == "") ? null : d.getFullYear() + "-" + ("0" + (d.getMonth() + 1)).slice(-2) + "-" + ("0" + d.getDate()).slice(-2); return datestring;
  }
  public onSpecClick(event: any) {
    $(event.target).parent().children("table.specTable").toggle();
    $(event.target).parent().parent().children(".provSpecFooter,.provFacFooter").toggle();
  }
  public addSpecToList(event: any) {
    let select: any = $("#add_Facility_Specialty option:selected");
    var specID = select.val(); var spec = this._specsList[specID];
    console.log(spec); this.Facility.FacilitySpecialties.unshift(spec);//like push but to beginning of array, so new spec is at top of list and easy to edit/identify as the 'new one' -- SKP
    //NOW USE EDIT BUTTON NOT SPEC CARD ITSELF
    let _newSpecDiv: any = $($("#specList div")[0]); _newSpecDiv.click(); _newSpecDiv.addClass("unsavedSpec");
    let specs: any = $(".indivSpecWrapper"); //each one of these has an id="specWrapper_{{s.ID}}"
    this.currentSpecOrder = specID + "," + this.currentSpecOrder;
    console.log(this.currentSpecOrder);
    console.log(this.Facility.FacilitySpecialties);
  }
  public toggleInactiveSpecialties() {
    let inactiveSpecs: any = $("div.status_INACTIVE");
    inactiveSpecs.toggle();
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
        + "<td><span class='childRowHeader'>External Provider</span> <br/>" + new BoolPipe().transform(d.FPRelationship.ExternalProviderIndicator) + "</td>"
        + "<td><span class='childRowHeader'>Accepting New Patients</span> <br/>" + new BoolPipe().transform(d.FPRelationship.AcceptingNewPatientIndicator) + "</td>"
        + "<td><span class='childRowHeader'>Prescriber</span> <br/>" + new BoolPipe().transform(d.FPRelationship.PrescriberIndicator) + "</td>"
        + "<td><span class='childRowHeader'>Referral</span> <br/>" + new BoolPipe().transform(d.FPRelationship.ReferralIndicator) + "</td>"
        //+ "<td>PCP Eligible <br/>" + new BoolPipe().transform(d.FPRelationship.PCPEligibleIndicator) + "</td>" BRANDON - Moved to Directory Table. Safe to delete this row of code -SKP
        + "<td><span class='childRowHeader'>Float Provider</span> <br/>" + new BoolPipe().transform(d.FPRelationship.FloatProviderIndicator) + "</td></tr>";
    }
  }
  //ADD_VENDOR_STUFF

  //END ADD_VENDOR_STUFF
  //save/edit Facility for Main/Demographics
  public saveFacility(type: number, event: any, specID?: number, specRelationshipID?: number) {
    if (!this.canEdit()) { alert("You think you smart?"); return; } //can only get here when inspect hacking, request hacking, network packet sniff/insert hacking, etc...
    let _editDivs: any = $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing"); _editDivs.hide(); _editDivs = null;
    let _notEditDivs: any = $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
    this.loading(true, type);//load starting, show overlay
    function val(which) { let _e: any = $("#edit_Facility_" + which); return _e.val(); }//MULTI SELECT VALS ARE ID ARRAYS
    function val2(which) { let _e: any = $("#edit_FacilityDemo_" + which); return _e.val(); } let body: any;
    switch (type) {
      case 0: //"Main" Facility-Header Info
        body = { Name: val("Name"), NPI: val("NPI"), User: environment.authUser.username };
        break;
      case 1: //Demographics (Really just an address)
        var phoneFixer = new PhoneToDBPipe();
        body = { Address1: val2("Address1"), Address2: val2("Address2"), City: val2("City"), State: val2("State"), Zip: val2("ZipCode"), User: environment.authUser.username };
        body.PhoneNumber = phoneFixer.transform(val2("PhoneNumber")); body.PhoneExtension = val2("PhoneExtension");
        body.AlternatePhoneNumber = phoneFixer.transform(val2("AltNumber")); body.AlternateExtension = val2("AltExtension");
        if (isNaN(body.PhoneNumber) || isNaN(body.AlternatePhoneNumber)) { alert("Invalid Phone Number. (must be 10 digit number, leading +1 allowed, dashes and parentheses allowed)"); return; }
        body.FaxNumber = val2("FaxNumber"); body.Website = val2("Website");
        break;
      case 2: //Specialties
        //reorganize specs first
        var specOrderArr = this.currentSpecOrder.split(",");
        var _FacilitySpecialties = JSON.parse(JSON.stringify(this.Facility.FacilitySpecialties));/*<--DEEP CLONE, so no circular references*/ this.Facility.FacilitySpecialties = [];
        for (var i = 0; i < specOrderArr.length; i++) {
          for (var j = 0; j < _FacilitySpecialties.length; j++) {
            if (_FacilitySpecialties[j].ID == specOrderArr[i]) { this.Facility.FacilitySpecialties.push(_FacilitySpecialties[j]); break; }
          }
        }
        //Stored Proc needs: (@SpecialtyID VARCHAR(10),@User VARCHAR(20),@ID INT, @SEQ INT, @EDATE DATE, @TDATE DATE = NULL, @First BIT = 0) for each Specialty
        body = { type: type, id: this.facilityId }; body.FacilitySpecialties = JSON.parse(JSON.stringify(this.Facility.FacilitySpecialties));
        for (var i = 0; i < body.FacilitySpecialties.length; i++) {
          var provSpec = body.FacilitySpecialties[i]; var pSpecLocal = this.Facility.FacilitySpecialties[i];
          provSpec.EffectiveDate = this.transformDateForPHDB("edit_ProviderSpec" + provSpec.ID + "_EffectiveDate");
          provSpec.TerminationDate = this.transformDateForPHDB("edit_ProviderSpec" + provSpec.ID + "_TerminationDate");
          var _e = document.getElementById("edit_ProviderSpec" + provSpec.ID + "_EffectiveDate") as HTMLFormElement; var d = new Date(_e.value);
          pSpecLocal.EffectiveDate = d.getTime(); _e = document.getElementById("edit_ProviderSpec" + provSpec.ID + "_TerminationDate") as HTMLFormElement; d = new Date(_e.value);
          pSpecLocal.TerminationDate = d.getTime();
        }
        break;
      default: //log error + weird behavior
        break;
    }
    console.log(body);
    this.service.hitAPI(this.apiRoot + "Facility/Save/" + type + "/" + this.facilityId, JSON.stringify(body)).subscribe(
      data => {
        console.log(data); this.loading(false, data.POSTvars.type);//load finished, hide overlay
        if (this.facilityId == 0) { document.location.href = "facility/" + data.result; }
        switch (data.POSTvars.type) {
          case 0:
            //header data to just transform over (replace with partial arr matching function)
            this.Facility.FacilityName = data.POSTvars.Name; this.Facility.NPI = data.POSTvars.NPI;
            this.Facility.LastUpdatedDate = new Date(); this.Facility.LastUpdatedBy = data.POSTvars.User;
            break;
          case 1:
            //demo data to just transform over (also replace with partial arr matching fxn)
            this.FacilityAddress.AddressLine1 = data.POSTvars.Address1; this.FacilityAddress.AddressLine2 = data.POSTvars.Address2;
            this.FacilityAddress.City = data.POSTvars.City; this.FacilityAddress.State = data.POSTvars.State; this.FacilityAddress.ZipCode = data.POSTvars.Zip;
            this.FacilityAddress.PhoneNumber = data.POSTvars.PhoneNumber; this.FacilityAddress.PhoneExtension = data.POSTvars.PhoneExtension;
            this.FacilityAddress.AlternatePhoneNumber = data.POSTvars.AlternatePhoneNumber; this.FacilityAddress.AlternateExtension = data.POSTvars.AlternateExtension;
            this.FacilityAddress.FaxNumber = data.POSTvars.FaxNumber; this.FacilityAddress.Website = data.POSTvars.Website;
            this.FacilityAddress.HideAlternatePhoneNumber = (data.POSTvars.AlternatePhoneNumber.trim() == "");
            this.FacilityAddress.HideAltExtension = (data.POSTvars.AlternateExtension.trim() == "");
            this.FacilityAddress.HidePhoneExtension = (data.POSTvars.PhoneExtension.trim() == "");
            this.Facility.LastUpdatedDate = new Date(); this.Facility.LastUpdatedBy = data.POSTvars.User;
            break;
          case 2:
            let _editDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing") : $("#specWrapper_" + specID + " .is-editing"); _editDivs.hide(); _editDivs = null;
            let _notEditDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing") : $("#specWrapper_" + specID + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
            for (var i = 0; i < this.Facility.FacilitySpecialties.length; i++) { var _ps = this.Facility.FacilitySpecialties[i]; _ps.LastUpdatedDate = new Date(); _ps.LastUpdatedBy = data.POSTvars.User; }
            let _addSpec: any = $("#addSpec"); let _resetSpec: any = $("#resetSpecs"); let _saveSpec: any = $("#saveSpecs"); let _addSpecBody: any = $("#addSpecBody");
            this.origSpecOrder = this.currentSpecOrder.replace(/\,/g, "|"); this.specsEdited = false;
            break;
          default: //log error + weird behavior
            break;
        }
      }
    );
  }

  public editFacility(type: number, event: any, specID?: number, specRelationshipID?: number) {
    if (!this.canEdit()) { alert("You think you smart?"); return; } //can only get here when inspect hacking, request hacking, network packet sniff/insert hacking, etc...
    let _editDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing") : $("#specWrapper_" + specID + " .is-editing"); _editDivs.show(); _editDivs = null;
    let _notEditDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing") : $("#specWrapper_" + specID + " .not-editing"); _notEditDivs.hide(); _notEditDivs = null;
    if (type == 2) { //specialty-specific edit: show card if not expanded + init Datepickers if not already init
      let _effDate: any = document.getElementById("edit_FacilitySpec" + specID + "_EffectiveDate"); let _termDate: any = document.getElementById("edit_FacilitySpec" + specID + "_TerminationDate");
      if (_effDate.getAttribute("ph-initialized") == "false") {
        let jQ_effDate: any = $(_effDate); jQ_effDate.datepicker({ showOtherMonths: true, selectOtherMonths: true, changeMonth: true, changeYear: true, dateFormat: "M d, yy", prevText: "<", nextText: ">" }); _effDate.setAttribute("ph-initialized", "true");
      }
      if (_termDate.getAttribute("ph-initialized") == "false") {
        let jQ_termDate: any = $(_termDate); jQ_termDate.datepicker({ showOtherMonths: true, selectOtherMonths: true, changeMonth: true, changeYear: true, dateFormat: "M d, yy", prevText: "<", nextText: ">" }); _effDate.setAttribute("ph-initialized", "true");
      }
      if (document.getElementById('specialtyTable_' + specID).style.display == "" || document.getElementById('specialtyTable_' + specID).style.display != "table") {
        let cardfooter: any = $("#specFooter_" + specID); cardfooter.toggle();
        let cardtable: any = $("#specialtyTable_" + specID); cardtable.toggle();
      }
    }
  }

  public cancelEdit(type: number, event: any, specID?: number, specRelationshipID?: number) {
    if (!this.canEdit()) { alert("You think you smart?"); return; } //can only get here when inspect hacking, request hacking, network packet sniff/insert hacking, etc...

    $("#edit_Facility_Name").val(this.Facility.FacilityName);
    $("#edit_Facility_NPI").val(this.Facility.NPI);
    $("#edit_FacilityDemo_Address1").val(this.Facility.FacilityAddress.AddressLine1);
    $("#edit_FacilityDemo_Address2").val(this.Facility.FacilityAddress.AddressLine2);
    $("#edit_FacilityDemo_City").val(this.Facility.FacilityAddress.City);
    $("#edit_FacilityDemo_State").val(this.Facility.FacilityAddress.State);
    $("#edit_FacilityDemo_ZipCode").val(this.Facility.FacilityAddress.ZipCode);
    $("#edit_FacilityDemo_PhoneNumber").val(this.Facility.FacilityAddress.PhoneNumber);
    $("#edit_FacilityDemo_PhoneExtension").val(this.Facility.FacilityAddress.PhoneExtension);
    $("#edit_FacilityDemo_FaxNumber").val(this.Facility.FacilityAddress.FaxNumber);
    $("#edit_FacilityDemo_Website").val(this.Facility.FacilityAddress.Website);

    let _editDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing") : $("#specWrapper_" + specID + " .is-editing"); _editDivs.hide(); _editDivs = null;
    let _notEditDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing") : $("#specWrapper_" + specID + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
    if (type == 2) { //specialty-specific cancel: hide card if not hidden
      if (document.getElementById('specialtyTable_' + specID).style.display == "" || document.getElementById('specialtyTable_' + specID).style.display != "table") {
        let cardfooter: any = $("#specFooter_" + specID); cardfooter.toggle();
        let cardtable: any = $("#specialtyTable_" + specID); cardtable.toggle();
      }
    }
  }

  public loading(isLoading, saveType) {
    let loadOverlay: any = $("#loadOverlay" + saveType);
    if (isLoading) { loadOverlay.show(); } else { loadOverlay.hide(); }
  }

}
