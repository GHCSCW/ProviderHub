import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { environment } from '../../environments/environment';
import { ProviderHubService } from '../app.service';
import { CommonModule, Location, DatePipe } from '@angular/common';
import { GenderPipe, NullablePipe, BoolPipe, ReverseBoolPipe, SpecialtyTypePipe, ParentSpecialtyPipe, NoValuePipe, PHDatePipe, SpecStatusPipe, TermStatusPipe } from '../pipes';

import * as $ from 'jquery';
import 'datatables.net';
import 'datatables.net-bs4';
import 'datatables.net-select-bs4';
import 'jquery-ui-bundle';
import 'selectize';

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
  public specsEdited: boolean = false;
  public facsEdited: boolean = false;
  public editingDivWrappers: any;
  public editingDivHeaderWrappers: any;
  public editingHeaderDivs: any;
  public _specsList: any;
  public _facsList: any;
  public origSpecOrder: any = null;
  public origFacOrder: any = null;
  public currentSpecOrder: any = null;
  public currentFacOrder: any = null;
  public defaultSpecDate: any = null;
  public environment: any = null;
  public networksDT: any;
  public employmentDT: any;


  constructor(private route: ActivatedRoute, private router: Router,
              private service: ProviderHubService, private location: Location) {
    this.Service = service; this.Provider = {};
    //"(Primary)" to mark primary credential takes up too much space for no benefit
    this.nav = 'Networks'; //default tab should be Demographics ***SKP 11/6: NOW NETWORKS***
    this.specsEdited = false; this.facsEdited = false;
  }

  public canEdit() {
    //SKP: Don't want to waste time: ask M$ why the fuck we have to make (or use) a custom JSON boolean serializer class, and it spits out "True" and "False" by default.
    return (environment.authUser.isSuperUser == 'True' || environment.authUser.isEditor == 'True');
  }

  ngOnInit() {
    this.apiRoot = environment.apiRoot; this.environment = environment;
    this.route.params.subscribe(params => {
      this.providerId = +params['id'];
      if (params['tabURL']) { this.initialTab = params['tabURL']; } else {
        this.location.replaceState("/Provider/Demographics/" + this.providerId);
      }
      if (this.providerId == 0) {
        this.nav = 'Demographics';
      }
    });
    console.log(environment.authUser);
    //_dis used as an alias for this as in the Angular this, for async JS to access the Angular 'this'. (otherwise most async JS functions' "this" means the event target)
    var _dis = this; var toClick = null;
    //0. nav
    var navs = document.getElementById("provider-nav").getElementsByTagName("li");
    for (var i = 0; i < navs.length; i++) {
      navs[i].addEventListener("click", (function (event) {
        return function (e) {
          _dis.nav = this.getAttribute("tab-id");
          _dis.location.replaceState("/Provider/" + this.getAttribute("tab-url") + "/" + _dis.providerId);
        }
      })(_dis), false);
      if (navs[i].getAttribute("tab-url") == this.initialTab) { toClick = navs[i] as HTMLElement; }
    }
    if (this.initialTab != "") { toClick.click(); }

    this.editingDivWrappers = ['#provider-demo', '#demoTable'];
    this.editingDivHeaderWrappers = ['#provider-main-header','#demo-card .card-title'];
    this.editingHeaderDivs = [document.getElementById("provider-main-header"), document.getElementById("demo-card").getElementsByClassName('card-title')[0]];

    //0=PROVIDER HEADER EDIT, 1=PROVIDER DEMO EDIT
      for (var i = 0; i < this.editingHeaderDivs.length; i++) {
        (function () {
          var divs = _dis.editingHeaderDivs[i]; var _i = i;
          divs.getElementsByClassName("not-editing")[0].addEventListener("click", function (event) { //edit
            if (_dis.canEdit()) { _dis.editProvider(_i, event); }
          });
          divs.getElementsByClassName("is-editing")[0].addEventListener("click", function (event) { //save
            if (_dis.canEdit()) { _dis.saveProvider(_i, event); }
          });
          divs.getElementsByClassName("is-editing")[1].addEventListener("click", function (event) { //cancel
            if (_dis.canEdit()) { _dis.cancelEdit(_i, event); }
          });
        })();
      }
    //PROVIDER SPEC EDIT, could be incorporated into above to save code, but enough is different about it that I separated it. - SKP
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
        var forIDs = _dis.Provider.ProviderSpecialties[0];
        _dis.saveProvider(2, e, forIDs.ID, forIDs.MappingID);
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
              if (i==0) { //< specIDs.length - 1
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
    //PROVIDER FAC EDIT, COULD BE COMBINED WITH SPEC TO SAVE LINES OF CODE BUT WOULD BE MESSY AND FILLED WITH TRINARIES AND SWITCHES
    let _addFac: any = $("#addFac"); let _resetFac: any = $("#resetFacs"); let _saveFac: any = $("#saveFacs"); let _addFacBody: any = $("#addFacBody");
    _addFac.click(function () {
      if (_dis.canEdit()) {
        _addFacBody.show(); //_addSpec.hide(); _resetSpec.show(); _saveSpec.show(); handled by facsEdited
        _dis.facsEdited = true;
      }
    });
    _saveFac.click(function (e) {
      if (_dis.canEdit()) {
        var forIDs = _dis.Provider.ProviderFacilities[0];
        _dis.saveProvider(3, e, 0, forIDs.RelationshipID);
      }
    });
    _resetFac.click(function () {
      if (_dis.canEdit()) { 
        _addFacBody.hide(); //_addSpec.show(); _resetSpec.hide(); _saveSpec.hide(); handled by facsEdited
        _dis.facsEdited = false;
        let _unsavedFac: any = $(".unsavedFac");
        _unsavedFac.remove();
        if (_dis.origFacOrder != null) {
          var facIDs = _dis.origFacOrder.split("|");
          //remove() all into memory to re-add?
          for (var i = 0; i < facIDs.length; i++) {
            var _facid = facIDs[i];
            if (_facid.trim() != "") {
              let _facWrapper: any = $("#facWrapper_" + _facid);
              //use prepend/postpend to reorder specs
              //$("h2").insertAfter($(".container"));
              if (i == 0) { //< specIDs.length - 1
                var _relation: any = $("#addFacArea");//$("#specWrapper_" + specIDs[i + 1]);
                _facWrapper.insertAfter(_relation);
              } else {
                var _relation: any = $("#facWrapper_" + facIDs[i - 1]);//$("#specWrapper_" + specIDs[i - 1]);
                _facWrapper.insertAfter(_relation);
              }
            }
          }
        }
      }
    });

    
    this.service.hitAPI(this.apiRoot + "Provider/ByID/" + this.providerId).subscribe(
      data => {
        //0. Edit/Save buttons and Misc UI
        for (var i = 0; i < this.editingDivWrappers.length; i++) {
          let _editDivs: any = $(this.editingDivHeaderWrappers[i] + " i.is-editing," + this.editingDivWrappers[i] + " .is-editing"); _editDivs.hide(); _editDivs = null;
          let _notEditDivs: any = $(this.editingDivHeaderWrappers[i] + " i.not-editing," + this.editingDivWrappers[i] + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
        }
        //1. Main and Demo
        this.Provider = data.p; var _c = this.Provider.CredentialListStr; this.Provider.NetworkTab = data.net; this.Provider.Employment = data.e;
        this.Provider.HospitalAffiliations = data.ha; /*HAHACD RIP 10/30/2018: I guess he was gonna go, but the season is still going on!*/
        console.log(this.Provider);
        var _credArr = this.Provider.CredentialListStr.split(","); var _langArr = this.Provider.Languages.split(",");
        //for (var i = 0; i < this.Provider.CredentialList.length;i++)
        for (var i = 0; i < _credArr.length; i++) { _credArr[i] = _credArr[i].trim(); } for (var i = 0; i < _langArr.length; i++) { _langArr[i] = _langArr[i].trim(); }
        this.Provider.LastUpdatedDate = this.Provider.LastUpdatedDate.replace(/\D/g, '').slice(0,-4);
        this.Provider.Credentials = (_c==null)? "" : _c.slice(0, -1).replace(/,/g,", ");//trailing comma
        document.getElementById("page-title").innerHTML = this.Provider.FirstName + " " + this.Provider.LastName + ", " + this.Provider.Credentials;
        //MEDICARE DATES
        this.Provider.MedicareEffectiveDate = (this.Provider.MedicareEffectiveDate && this.Provider.MedicareEffectiveDate != null) ? this.Provider.MedicareEffectiveDate.replace(/\D/g, '').slice(0, -4) : "";
        this.Provider.MedicareTerminationDate = (this.Provider.MedicareTerminationDate && this.Provider.MedicareTerminationDate != null) ? this.Provider.MedicareTerminationDate.replace(/\D/g, '').slice(0, -4) : "";
        //1b. SELECTIZE from 'full entity' lists: Credentials and Languages
        let toSelectize: any = $("#edit_Provider_Credentials,#edit_ProviderDemo_Language,#add_Provider_Specialty");
        //POPULATE CREDENTIALS AND LANGUAGES SELECT TAG, THEN SET PROVIDER'S CURRENT VALUES AS SELECTED OPTION(S)
        let credsList: any = data.c; let languagesList: any = data.l; let specsList: any = data.s; var lSelectHTML, fSelectHTML, cSelectHTML, sSelectHTML = ""; this._specsList = []; let facsList: any = data.f; this._facsList = [];
        let lSelect: any = $("#edit_ProviderDemo_Language"); let cSelect: any = $("#edit_Provider_Credentials"); let sSelect: any = $("#add_Provider_Specialty"); let fSelect: any = $("#add_Provider_Facility");
        for (var i = 0; i < credsList.length; i++) { var selected = (_credArr.includes(credsList[i].Value)) ? "selected" : ""; cSelectHTML += "<option value='" + credsList[i].ID + "' " + selected + ">" + credsList[i].Value + " - " + credsList[i].Description + "</option>"; }
        for (var i = 0; i < languagesList.length; i++) { var selected = (_langArr.includes(languagesList[i].Name)) ? "selected" : ""; lSelectHTML += "<option value='" + languagesList[i].ID + "' " + selected + ">" + languagesList[i].Name + "</option>"; }
        //initializing blank versions of Specialty object for each Specialty ID (for use in Add Provider-Specialty dropdown)
        for (var i = 0; i < specsList.length; i++) {
          sSelectHTML += "<option value='" + specsList[i].ID + "'>" + specsList[i].Name + "</option>";
          var toAdd = specsList[i]; toAdd.SequenceNumber = this.Provider.ProviderSpecialties.length; toAdd.MappingID = 0; toAdd.ID =specsList[i].ID; toAdd.LastUpdatedBy = environment.authUser.username;
          toAdd.EffectiveDate = "/Date(1451628000000-0600)/".replace(/\D/g, '').slice(0, -4); toAdd.TerminationDate = ''; toAdd.LastUpdatedDate = new Date();//use datepipe if needed
          toAdd.Status = "ACTIVE"; toAdd.ParentName = ''; toAdd.ParentSpecialtyID = 0; this._specsList[toAdd.ID]=toAdd;
        }
        console.log(facsList);
        //initializing blank versions of Facility object for each Facility ID (for use in Add Provider-Facility dropdown)
        for (var i = 0; i < facsList.length; i++){
          var toAdd = facsList[i];
          fSelectHTML += "<option value='"+facsList[i].ID + "'>" + facsList[i].FacilityName + "</option>";
          toAdd.FPRelationship = {
            RelationshipID: 0,
              ExternalProviderIndicator:null,
                AcceptingNewPatientIndicator:null,
                  PrescriberIndicator:null,
                    ReferralIndicator:null,
                      FloatProviderIndicator:null,
                      EffectiveDate : "Date(1451628000000)",
                      TerminationDate : null,
                        ProviderPhoneNumber:'',
                          PhoneExtension:'',
                            LastUpdatedBy:environment.authUser.username,
                              LastUpdatedDate:new Date()
          }
          //dates like spec
          this._facsList[toAdd.ID] = toAdd; //set fields
        }
        //MAKE THIS INTO A HELPER FUNCTION THAT ITERATES THROUGH EACH SELECT TAG AND APPLIES APPROPRIATE SELECT HTML
        sSelect.html("<select>" + sSelectHTML + "</select>"); lSelect.html("<select>" + lSelectHTML + "</select>"); cSelect.html("<select>" + cSelectHTML + "</select>"); fSelect.html("<select>" + fSelectHTML + "</select>");
        //SELECTIZE ALL SELECTS, SEND VAR TO GARBAGE COLLECTOR. IF DRAG_DROP DOESNT WORK WITH SINGLE SELECTS, MOVE SPEC SELECTIZE TO ITS OWN INITIALIZATION
        toSelectize.selectize({plugins:['drag_drop']}); toSelectize = null;
        //"BOOL" SELECTS (YES/NO/UNKNOWN) NOT TO BE SELECTIZED. JUST SET PROVIDER'S CURRENT VALUE AS SELECTED
        //2. Specialties
        // Object spec (per Provider):
        //  ProviderSpecialties:[{Specialty},{Specialty},...]
        // Each Specialty = {ID,Name,Description,MappingID,SequenceNumber,CreatedDate,
        // CreatedBy,EffectiveDate,TerminationDate,SpecialtyType,ParentSpecialtyID,ParentName}
        //this.origSpecOrder.split("|");
        this.origSpecOrder = ""; this.currentSpecOrder = ""; this.origFacOrder = ""; this.currentFacOrder = "";
        for (var i = 0; i < this.Provider.ProviderSpecialties.length; i++) {
          var s = this.Provider.ProviderSpecialties[i];
          s.EffectiveDate = s.EffectiveDate.replace(/\D/g, '').slice(0, -4); if (i == 0) { this.defaultSpecDate = s.EffectiveDate; }
          s.TerminationDate = (s.TerminationDate == null) ? '' : s.TerminationDate.replace(/\D/g, '').slice(0, -4);
          s.LastUpdatedDate = (s.LastUpdatedDate == null) ? '' : s.LastUpdatedDate.replace(/\D/g, '').slice(0, -4);
          s.Status = new SpecStatusPipe().transform(s);
          this.origSpecOrder += s.ID + "|";
          this.currentSpecOrder += s.ID + ",";//comma for this one since used in SP that takes comma-separated intlist and updates order: (see intlist_to_tbl helper fxn in DB and SPs that use it)
          //console.log(s);
        }
        if (this.origSpecOrder != "") { this.origSpecOrder = this.origSpecOrder.slice(0, -1); }
        if (this.currentSpecOrder != "") { this.currentSpecOrder = this.currentSpecOrder.slice(0, -1); }
        let list: any = $('#specList');
        //update(event, ui): This event is triggered when the user stopped sorting and the DOM position has changed. AKA when spec is 'dropped' after being 'dragged' -- SKP
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
        let facList: any = $("#facList");
        facList.sortable({
          update: function (event, ui) {
            _dis.facsEdited = true;
            let facs: any = $(".indivFacWrapper"); //each one of these has an id="facWrapper_{{f.ID}}"
            _dis.currentFacOrder = "";
            facs.each(function (this, index) {
              let spec: any = $(this);
              var _fid = spec.attr("id").replace("facWrapper_", "");//if you want you can make a custom attr and just do .attr("CUSTOM_ATTR_NAME");
              _dis.currentFacOrder += _fid + ",";
            });
            if (_dis.currentFacOrder != "") { _dis.currentFacOrder = _dis.currentFacOrder.slice(0, -1); }
            console.log(_dis.currentFacOrder); console.log(_dis.origFacOrder);
          }
        });
        //2b. Networks tab (new default! tab)
        /*datatable looks like this
        <th>Network</th>
              <th>Network Effective Date</th>
              <th>Provider</th>
              <th>Facility</th>
              <th>Specialty</th>
              <th>EPIC Network ID</th>
        */
        for (var i = 0; i < this.Provider.NetworkTab.length; i++) {
          //any manipulation of fields needed for clean DT functionality, UI display, or other reason (html in column, link in column, etc...see Fac Provider list for examples)
        }
        let networksDTID: any = $('#networksTableDT');
        this.networksDT = networksDTID.DataTable({
          select: true,
          paging: false,
          language: { search: "", searchPlaceholder: "Begin typing in a Provider Name or NPI to filter search results" },
          data: this.Provider.NetworkTab,
          columns: [{ data: null, orderable: false, searchable: false, defaultContent: '' },
            { data: "Network" }, { data: "Facility" }, { data: "Specialty" }//SKP FIELDS NO LONGER USED, but returned from Service if needed - { data: "Provider" }, { data: "EpicNetworkID" }, { data: "NetworkEffectiveDate" }, 
          ],
          order: [[1, "asc"]],
          rowId: 'ID',
          "createdRow": function ( row, data, index ) {
            if ( new TermStatusPipe().transform(data.DirectoryTerminationDate.replace(/\D/g, '').slice(0,-4))  == "INACTIVE" ) {
              $(row).addClass("directory-inactive");
            }
          },
          initComplete: function (settings, json) {
            if (_dis.nav != 'Providers') { $("#providersTableDT_wrapper").hide(); } else { $("#providersTableDT_wrapper").show(); } /*cant do in Angular since generated by DT*/
            if (typeof (Event) === 'function') { window.dispatchEvent(new Event('resize')); }
            else { var evt = window.document.createEvent('UIEvents'); evt.initUIEvent('resize', true, false, window, 0); window.dispatchEvent(evt); }
          }
        });
        this.toggleInactiveDirectories(); //initally hide inactive directories
        this.networksDT.on('select',
          (e, dt, type, indexes) => {
            console.log(this.networksDT.rows(indexes).data().pluck("ID"));
            this.onRowSelect(this.networksDT.rows(indexes).data().pluck("ID"));
          }
        );
        this.networksDT.on('deselect',
          (e, dt, type, indexes) => {
            console.log(this.networksDT.rows(indexes).data().pluck("ID"));
            this.onRowSelect(this.networksDT.rows(indexes).data().pluck("ID"));
          }
        );
        let employmentDTID: any = $('#employmentTableDT');
        this.employmentDT = employmentDTID.DataTable({
          select: true,
          paging: false,
          searching: false,
          data: this.Provider.Employment,
          columns: [{ data: null, orderable: false, searchable: false, defaultContent: '' },
            { data: "FacilityName" }, { data: "SpecialtyName" }//SKP FIELDS NO LONGER USED, but returned from Service if needed - { data: "Provider" }, { data: "EpicNetworkID" }, { data: "NetworkEffectiveDate" }, 
          ],
          order: [[1, "asc"]],
          rowId: 'ID',
          initComplete: function (settings, json) {
            if (_dis.nav != 'Providers') { $("#providersTableDT_wrapper").hide(); } else { $("#providersTableDT_wrapper").show(); } /*cant do in Angular since generated by DT*/
            if (typeof (Event) === 'function') { window.dispatchEvent(new Event('resize')); }
            else { var evt = window.document.createEvent('UIEvents'); evt.initUIEvent('resize', true, false, window, 0); window.dispatchEvent(evt); }
          }
        });
        this.employmentDT.on('select',
          (e, dt, type, indexes) => {
            console.log(this.employmentDT.rows(indexes).data().pluck("ID"));
            this.onEmploymentsRowSelect(this.employmentDT.rows(indexes).data().pluck("ID"));
          }
        );
        this.employmentDT.on('deselect',
          (e, dt, type, indexes) => {
            console.log(this.employmentDT.rows(indexes).data().pluck("ID"));
            this.onEmploymentsRowSelect(this.employmentDT.rows(indexes).data().pluck("ID"));
          }
        );
        //3. Additional properties for UI conditionals ('novalue' pipe doesn't work in HTML. Not sure why so we use it here)
        //this.origFacOrder = ""; this.currentFacOrder = "";
        for (var i = 0; i < this.Provider.ProviderFacilities.length; i++) {
          var f = this.Provider.ProviderFacilities[i];
          f.FacilityAddress.HidePhoneExtension = new NoValuePipe().transform(f.FacilityAddress.PhoneExtension);
          f.FacilityAddress.HideAlternatePhoneNumber = new NoValuePipe().transform(f.FacilityAddress.AlternatePhoneNumber);
          var fp = f.FPRelationship; fp.LastUpdatedDate = new PHDatePipe().transform(fp.LastUpdatedDate.replace(/\D/g, '').slice(0, -4));
          fp.status = new TermStatusPipe().transform(fp.TerminationDate.replace(/\D/g, '').slice(0, -4));
          this.origFacOrder += f.ID + "|"; this.currentFacOrder += f.ID + ",";
        }
        //this.toggleInactiveProvFacRel(); //initially hide Inactive Provider-Facility Relationships
        if (this.origFacOrder != "") { this.origFacOrder = this.origFacOrder.slice(0, -1); }
        if (this.currentFacOrder != "") { this.currentFacOrder = this.currentFacOrder.slice(0, -1); }
        //4. Post-load UI actions
        let facilityLink: any = $("#provider-nav li[tab-id='Facility']");
        facilityLink.click(function () {
          if (typeof (Event) === 'function') { window.dispatchEvent(new Event('resize')); }
          else { var evt = window.document.createEvent('UIEvents'); evt.initUIEvent('resize', true, false, window, 0); window.dispatchEvent(evt); }
        });
        let datepickers: any = $(".datepicker"); datepickers.datepicker({ showOtherMonths: true, selectOtherMonths:true, changeMonth:true, changeYear:true, dateFormat:"M d, yy", prevText:"<", nextText:">" });
      }
    );
    //note: if navigated to from direct link, and not clicking a provider,
    //      it'll be empty til the provider object loads from AJAX...but that's okay
    document.getElementById("page-title").innerHTML = API.selectedProvider;
  }

  public transformDateForPHDB(datepicker_element_id: any) {
    var _e = document.getElementById(datepicker_element_id) as HTMLFormElement; var d = new Date(_e.value); var _jqe: any = $("#"+datepicker_element_id);
    var datestring = (_e.value=="" || _jqe.val().trim()=="")? null : d.getFullYear() + "-" + ("0" + (d.getMonth() + 1)).slice(-2) + "-" + ("0" + d.getDate()).slice(-2); return datestring;
  }

  public toggleInactiveSpecialties() {
    let inactiveSpecs: any = $("div.status_INACTIVE");
    inactiveSpecs.toggle();
  }

  public toggleInactiveDirectories() {
    let inactiveDirs: any = $(".directory-inactive");
    inactiveDirs.toggle();
  }

  public toggleInactiveProvFacRel() {
    let inactiveProvFacRel: any = $("div.fac_status_INACTIVE");
    inactiveProvFacRel.toggle();
  }

  public toggleFields() {
    let select: any = $("#edit_ProviderDemo_MedicareIndicator option:selected");
    this.Provider.MedicareIndicator = (select.val()=="Yes")? true: (select.val()=="No")? false : null;
  }

  public toggleFields2() {
    let select: any = $("#edit_ProviderDemo_MedicaidIndicator option:selected");
    this.Provider.MedicaidIndicator = (select.val() == "Yes") ? true : (select.val() == "No") ? false : null;
  }

  public addSpecToList(event: any) {
    let select: any = $("#add_Provider_Specialty option:selected");
    var specID = select.val(); var spec = this._specsList[specID];
    console.log(spec); this.Provider.ProviderSpecialties.unshift(spec);//like push but to beginning of array, so new spec is at top of list and easy to edit/identify as the 'new one' -- SKP
    //NOW USE EDIT BUTTON NOT SPEC CARD ITSELF
    let _newSpecDiv: any = $($("#specList div")[0]); _newSpecDiv.click(); _newSpecDiv.addClass("unsavedSpec");
    let specs: any = $(".indivSpecWrapper"); //each one of these has an id="specWrapper_{{s.ID}}"
    this.currentSpecOrder = specID + "," + this.currentSpecOrder;
    console.log(this.currentSpecOrder);
    //console.log(_dis.currentSpecOrder); console.log(_dis.origSpecOrder);
    console.log(this.Provider.ProviderSpecialties);
  }

  public addFacToList(event: any) {
    let select: any = $("#add_Provider_Facility option:selected");
    var facID = select.val(); var fac = this._facsList[facID];
    console.log(fac); this.Provider.ProviderFacilities.unshift(fac);
    let _newFacDiv: any = $($("#facList div")[0]); _newFacDiv.click(); _newFacDiv.addClass("unsavedSpec");
    let facs: any = $(".indivFacWrapper"); //each one of these has an id="facWrapper_{{f.ID}}"
    this.currentFacOrder = facID + "," + this.currentFacOrder;
    //if (this.currentFacOrder != "") { this.currentFacOrder = this.currentFacOrder.slice(0, -1); }
    console.log(this.currentFacOrder); console.log(this.origFacOrder);
    console.log(this.Provider.ProviderFacilities);
  }

  private onRowSelect(indexes: number[]): void {
    var providerId = indexes[0];
    console.log("Provider ID: " + providerId);
    console.log($("tr#" + providerId));
    var row = this.networksDT.row($("tr#" + providerId)[0]);
    console.log($("tr#" + providerId).hasClass("directory-inactive"));
    if (row.child.isShown()) {
      row.child.hide();
    } else {
      row.child(format(row.data()), 'expandedFP').show();
    }
    function format(d) {
      let inactiveChild: string = $("tr#" + providerId).hasClass("directory-inactive") ? " directory-inactive" : "";
      return "<table class='plainjane" + inactiveChild + "'>" + "<tr><td><b>DIRECTORY ID:" + d.ID + "</b></td>"
        + "<td><span class='childRowHeader'>External Provider</span> <br/>" + new BoolPipe().transform(d.FPRelationship.ExternalProviderIndicator) + "</td>"
        + "<td>Accepting New Patients <br/>" + new BoolPipe().transform(d.FPRelationship.AcceptingNewPatientIndicator) + "</td>"
        + "<td>Prescriber <br/>" + new BoolPipe().transform(d.FPRelationship.PrescriberIndicator) + "</td>"
        + "<td>Referral <br/>" + new BoolPipe().transform(d.FPRelationship.ReferralIndicator) + "</td>"
        + "<td>Directory Effective Date <br/>" + new PHDatePipe().transform(d.DirectoryEffectiveDate.replace(/\D/g, '').slice(0, -4)) + "</td>"
        + "<td>Directory Termination Date <br/>" + new PHDatePipe().transform(d.DirectoryTerminationDate.replace(/\D/g, '').slice(0, -4)) + "</td>"
        + "<td>PCP Eligible <br/>" + new BoolPipe().transform(d.PCPEligibleIndicator) + "</td>" 
        + "<td>Float Provider <br/>" + new BoolPipe().transform(d.FPRelationship.FloatProviderIndicator) + "</td></tr>";
    }
  }
  private onEmploymentsRowSelect(indexes: number[]): void {
    var providerId = indexes[0];
    console.log("Employment ID: " + providerId);
    console.log($("tr#" + providerId));
    var row = this.employmentDT.row($("tr#" + providerId)[0]);
    if (row.child.isShown()) {
      row.child.hide();
    } else {
      console.log(row.data());
      row.child(format(row.data()), 'expandedFP').show();
    }
    function format(e) {
      return "<table class='plainjane'><tr>"
        + "<td><b>EMPLOYMENT ID:" + e.ID + "</b></td>"
        + "<td><b>Effective Date</b><br/>" + new PHDatePipe().transform(e.EffectiveDate) + "</td>"
        + "<td><b>Termination Date</b><br/>" + new PHDatePipe().transform(e.TerminationDate) + "</td>"
        + "</tr></table>";
    }
  }

  public onSpecClick(event: any) {
    $(event.target).parent().children("table.specTable").toggle();
    $(event.target).parent().parent().children(".provSpecFooter,.provFacFooter").toggle();
  }

  public setSortable(event: any) {
    alert("set sortable!");
  }

  //$(getEditDivsSelector(type_of_edit_desired, optional_id_of_editable_object, optional_mapping_id_assoc_with_editable_objet)
  public getEditDivsSelector(type: number, ID?: number, MappingID?: number) {
    switch (type) {
      case 0:
      case 1: //Header, Demo
        return this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing";
      case 2: //Spec
        return "#specWrapper_" + ID + " .is-editing";
      case 3: //FPRelationship
        var toReturn=""; for (var i = 0; i < 6; i++) { toReturn += "#row"+i+"FP_" + MappingID + " .is-editing,"; }//can extend rows if needed
        return toReturn.slice(0, -1);
      default: break;/*log invalid type*/
    }
  }

  public getNotEditDivsSelector(type: number, ID?: number, MappingID?: number) {
    switch (type) {
      case 0:
      case 1: //Header, Demo
        return this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing";
      case 2: //Spec
        return "#specWrapper_" + ID + " .not-editing";
      case 3: //FPRelationship
        var toReturn = ""; for (var i = 0; i < 6; i++) { toReturn += "#row" + i + "FP_" + MappingID + " .not-editing,"; }//can extend rows if needed
        return toReturn.slice(0, -1);
      default: break;/*log invalid type*/
    }
  }

  public saveProvider(type: number, event: any, entityID?: number, entityRelationshipID?: number) {
    if (!this.canEdit()) { alert("You think you smart?"); return; } //can only get here when inspect hacking, request hacking, network packet sniff/insert hacking, etc...
    let _editDivs: any = $(this.getEditDivsSelector(type, entityID, entityRelationshipID)); _editDivs.hide(); _editDivs = null;
    let _notEditDivs: any = $(this.getNotEditDivsSelector(type, entityID, entityRelationshipID)); _notEditDivs.show(); _notEditDivs = null;
    this.loading(true, type);//load starting, can implement overlay w/ loading gif or animation of your choice (or completion bar) in the loading() function, or build a component/service
    //...since 'val' and 'val2' are used only in the below 5 lines of code I didn't want to make function names long. If you do for more readability that's okay.They are only used in case 0 and case 1 of switch statement below
    function val(which) { let _e: any = $("#edit_Provider_" + which); return _e.val(); }//MULTI SELECT VALS ARE ID ARRAYS, see SQL function intlist_to_tbl
    function val2(which) { let _e: any = $("#edit_ProviderDemo_" + which); return _e.val(); }
    //console.log((data.POSTvars.MedicareIndicator == "Yes") ? true : (data.POSTvars.MedicareIndicator == "No") ? false : null); console.log(this.Provider.MedicareIndicator);
    function val3(which, id) { let _e: any = $("#edit_ProviderFP_" + id + "_" + which + " option:selected"); return _e.val()=="Yes"?true:_e.val()=="No"?false:null; }
    let body: any;
    switch (type) {
      case 0: //"Main" Provider-Header Info
        body = { FirstName: val("FirstName"), LastName: val("LastName"), Credentials: val("Credentials"), NPI: val("NPI"), EpicProviderID: val("EpicProviderID"), Gender: val("Gender"), User: environment.authUser.username };
        break;
      case 1: //Demographics
        body = { Languages: val2("Language"), MedicareIndicator: val2("MedicareIndicator"), MedicaidIndicator: val2("MedicaidIndicator"), User: environment.authUser.username };
        if (body.MedicareIndicator == "Yes") { body.MedicarePTAN = val2("MedicarePTAN"); body.MedicareEffectiveDate = this.transformDateForPHDB("edit_ProviderDemo_MedicareEffectiveDate"); body.MedicareTerminationDate = this.transformDateForPHDB("edit_ProviderDemo_MedicareTerminationDate"); }
        //val2("MedicareEffectiveDate"), val2("MedicareTerminationDate")
        if (body.MedicaidIndicator == "Yes") { body.MedicaidProviderID = val2("MedicaidProviderID"); }
        break;
      case 2: //Specialties
        //reorganize specs first: DEEP CLONE array, then re-order by pushing elements back to old one from new one. then use re-organized old one (auto-updating HTML bindings in the process :D )
        var specOrderArr = this.currentSpecOrder.split(",");
        console.log(this.Provider.ProviderSpecialties);
        var _ProviderSpecialties = JSON.parse(JSON.stringify(this.Provider.ProviderSpecialties));/*<--DEEP CLONE, so no circular references*/ this.Provider.ProviderSpecialties = [];
        var _inactiveIndexes = [];
        for (var i = 0; i < specOrderArr.length; i++) {
          for (var j = 0; j < _ProviderSpecialties.length; j++) {
            if (_ProviderSpecialties[j].ID == specOrderArr[i]) {
              var TermTime = new Date((document.getElementById("edit_ProviderSpec" + _ProviderSpecialties[j].ID + "_TerminationDate") as HTMLFormElement).value);
              if (TermTime == null || TermTime > new Date()) { this.Provider.ProviderSpecialties.push(_ProviderSpecialties[j]); break; }//ACTIVE push
              if (TermTime != null && TermTime <= new Date()) { _inactiveIndexes.push(specOrderArr[i]); break; }//INACTIVE don't push til end
            }
          }
        }
        for (var i = 0; i < _inactiveIndexes.length; i++) {
          for (var j = 0; j < _ProviderSpecialties.length; j++) {
            if (_ProviderSpecialties[j].ID == _inactiveIndexes[i]) { this.Provider.ProviderSpecialties.push(_ProviderSpecialties[j]); break; }//INACTIVE push
          }
        }
        //Stored Proc needs: (@SpecialtyID VARCHAR(10),@User VARCHAR(20),@ID INT, @SEQ INT, @EDATE DATE, @TDATE DATE = NULL, @First BIT = 0) for each Specialty
        console.log(this.Provider.ProviderSpecialties);
        body = { type: type, id: this.providerId }; body.ProviderSpecialties = JSON.parse(JSON.stringify(this.Provider.ProviderSpecialties));
        for (var i = 0; i < body.ProviderSpecialties.length; i++) {
          var provSpec = body.ProviderSpecialties[i]; var pSpecLocal = this.Provider.ProviderSpecialties[i];
          provSpec.EffectiveDate = this.transformDateForPHDB("edit_ProviderSpec" + provSpec.ID + "_EffectiveDate");
          provSpec.TerminationDate = this.transformDateForPHDB("edit_ProviderSpec" + provSpec.ID + "_TerminationDate");
          var _e = document.getElementById("edit_ProviderSpec"+provSpec.ID+"_EffectiveDate") as HTMLFormElement; var d = new Date(_e.value);
          pSpecLocal.EffectiveDate = d.getTime(); _e = document.getElementById("edit_ProviderSpec" + provSpec.ID + "_TerminationDate") as HTMLFormElement; d = new Date(_e.value);
          pSpecLocal.TerminationDate = d.getTime(); provSpec.SequenceNumber = i + 1; pSpecLocal.SequenceNumber = provSpec.SequenceNumber;// (d != null && d <= new Date()) ? 999 : i+1
          pSpecLocal.Status = new SpecStatusPipe().transform(pSpecLocal);
        }
        console.log(this.Provider.ProviderSpecialties);
        break;
      case 3: //FPRelationship aka Facility-Provider Relationship
        //reorganize facs first: DEEP CLONE array then re-order just like spec
        var facOrderArr = this.currentFacOrder.split(","); var _inactiveIndexes = [];
        var _ProviderFacilities = JSON.parse(JSON.stringify(this.Provider.ProviderFacilities)); this.Provider.ProviderFacilities = [];
        for (var i = 0; i < facOrderArr.length; i++) {
          for (var j = 0; j < _ProviderFacilities.length; j++) {
            if (_ProviderFacilities[j].ID == facOrderArr[i]) {
              var TermTime = new Date((document.getElementById("edit_ProviderFP_" + _ProviderFacilities[j].FPRelationship.RelationshipID + "_TerminationDate") as HTMLFormElement).value);
              if (TermTime == null || TermTime > new Date()) { this.Provider.ProviderFacilities.push(_ProviderFacilities[j]); break; }//ACTIVE push
              if (TermTime != null && TermTime <= new Date()) { _inactiveIndexes.push(facOrderArr[i]); break; }//INACTIVE don't push til end
            }
          }
        }
        for (var i = 0; i < _inactiveIndexes.length; i++) {
          for (var j = 0; j < _ProviderFacilities.length; j++) {
            if (_ProviderFacilities[j].ID == _inactiveIndexes[i]) { this.Provider.ProviderFacilities.push(_ProviderFacilities[j]); break; }//INACTIVE push
          }
        }
        //nothing to really modify per object, since we only can add FacID's and add/edit FPR indicators (all nullable BITs)
        body = { type: type, id: this.providerId }; body.ProviderFacilities = JSON.parse(JSON.stringify(this.Provider.ProviderFacilities)); console.log(body);
        for (var i = 0; i < body.ProviderFacilities.length; i++) {
          //indicator values from UI
          var toSet = body.ProviderFacilities[i].FPRelationship; var pFacLocal = this.Provider.ProviderFacilities[i].FPRelationship;
          //    function val3(which,id) { let _e: any = $("edit_ProviderFP_"+id+"_"+which); return _e.val(); }
          var _entityRelationshipID = toSet.RelationshipID;
          toSet.ExternalProviderIndicator = val3("ExternalProviderIndicator", _entityRelationshipID); toSet.PrescriberIndicator = val3("PrescriberIndicator", _entityRelationshipID);
          toSet.AcceptingNewPatientIndicator = val3("AcceptingNewPatientIndicator", _entityRelationshipID); toSet.ReferralIndicator = val3("ReferralIndicator", _entityRelationshipID);
          toSet.FloatProviderIndicator = val3("FloatProviderIndicator", _entityRelationshipID);
          toSet.ProviderPhone = val3("PhoneNumber", _entityRelationshipID); toSet.ProviderExtension = val3("PhoneExtension", _entityRelationshipID);
          let _there_are_dates_to_update: any = $("#edit_ProviderFP_" + _entityRelationshipID + "_EffectiveDate").length > 0;
          if (_there_are_dates_to_update) {
            toSet.EffectiveDate = this.transformDateForPHDB("edit_ProviderFP_" + _entityRelationshipID + "_EffectiveDate");
            toSet.TerminationDate = this.transformDateForPHDB("edit_ProviderFP_" + _entityRelationshipID + "_TerminationDate");
            var _e = document.getElementById("edit_ProviderFP_" + _entityRelationshipID + "_EffectiveDate") as HTMLFormElement; var d = new Date(_e.value);
            pFacLocal.EffectiveDate = d.getTime(); _e = document.getElementById("edit_ProviderFP_" + _entityRelationshipID + "_TerminationDate") as HTMLFormElement; d = new Date(_e.value);
            pFacLocal.TerminationDate = d.getTime(); toSet.SequenceNumber = (d != null && d <= new Date()) ? 999 : i + 1; pFacLocal.SequenceNumber = toSet.SequenceNumber;
            pFacLocal.status = new TermStatusPipe().transform(pFacLocal.TerminationDate);
          }
          console.log(toSet.EffectiveDate); console.log(toSet.TerminationDate);//should be current values if !_there_are_dates_to_update
          console.log(pFacLocal.EffectiveDate); console.log(pFacLocal.TerminationDate);
        }
        break;
      default: //log error + weird behavior
        break;
    }    
    console.log(body);
    this.service.hitAPI(this.apiRoot + "Provider/Save/" + type + "/" + this.providerId, JSON.stringify(body)).subscribe(
      data => {
        console.log(data); this.loading(false, data.POSTvars.type);//load finished, hide overlay
        if (this.providerId == 0) { document.location.href = "provider/"+data.result; }
        switch (data.POSTvars.type) {
          case 0:
            //header data to just transform over (replace with partial array matching function)
            this.Provider.FirstName = data.POSTvars.FirstName; this.Provider.LastName = data.POSTvars.LastName;
            this.Provider.NPI = data.POSTvars.NPI; this.Provider.EpicProviderID = data.POSTvars.EpicProviderID;
            this.Provider.Gender = data.POSTvars.Gender;
            this.Provider.LastUpdatedDate = new Date(); this.Provider.LastUpdatedBy = data.POSTvars.User;
            //break down credentials arr and build credentials string
            var _c = ""; for (var i = 0; i < data.POSTvars.Credentials.length; i++) {
              let _e: any = $("#edit_Provider_Credentials option[value='" + data.POSTvars.Credentials[i] + "']");
              _c += _e.text().substring(0, _e.text().indexOf(" - ")); _c += (i==data.POSTvars.Credentials.length-1)? "" : "," ;
            }
            this.Provider.Credentials = _c;
            break;
          case 1:
            //demographics data to just transform over (also replace with partial arr matching fxn)
            this.Provider.MedicareIndicator = new ReverseBoolPipe().transform(data.POSTvars.MedicareIndicator);
            //console.log((data.POSTvars.MedicareIndicator == "Yes") ? true : (data.POSTvars.MedicareIndicator == "No") ? false : null); console.log(this.Provider.MedicareIndicator);
            this.Provider.MedicaidIndicator = new ReverseBoolPipe().transform(data.POSTvars.MedicaidIndicator);//(data.POSTvars.MedicaidIndicator == "Yes") ? true : (data.POSTvars.MedicaidIndicator == "No") ? false : null
            this.Provider.MedicarePTAN = (this.Provider.MedicareIndicator === true) ? data.POSTvars.MedicarePTAN : null;//need ===true since it's nullable bit
            this.Provider.MedicareEffectiveDate = (this.Provider.MedicareIndicator === true) ? data.POSTvars.MedicareEffectiveDate : null;// TODO: format dates for datepipe/display, see what UI currently does first
            this.Provider.MedicareTerminationDate = (this.Provider.MedicareIndicator === true) ? data.POSTvars.MedicareTerminationDate : null;// TODO: format dates for datepipe/display, see what UI currently does first
            this.Provider.MedicaidProviderID = (this.Provider.MedicareIndicator === true) ? data.POSTvars.MedicaidProviderID : null;
            this.Provider.LastUpdatedDate = new Date(); this.Provider.LastUpdatedBy = data.POSTvars.User;
            //break down lang arr like we did the creds arr
            var _l = ""; for (var i = 0; i < data.POSTvars.Languages.length; i++) {
              let _e: any = $("#edit_ProviderDemo_Language option[value='" + data.POSTvars.Languages[i] + "']");
              _l += _e.text(); _l += (i == data.POSTvars.Languages.length - 1) ? "" : ",";
            }
            this.Provider.Languages = _l;
            break;
          case 2: //specialty save return
            for (var i = 0; i < this.Provider.ProviderSpecialties.length; i++) {
              var _ps = this.Provider.ProviderSpecialties[i]; _ps.LastUpdatedDate = new Date();
              _ps.LastUpdatedBy = data.POSTvars.User;
              _ps.SequenceNumber = i + 1;
            }
            console.log(this.Provider.ProviderSpecialties);
            this.origSpecOrder = this.currentSpecOrder.replace(/\,/g, "|"); this.specsEdited = false;
            break;
          case 3: //provider facility save
            //update FPR nullable bits like so
            //EXAMPLE: this.Provider.MedicareIndicator = (data.POSTvars.MedicareIndicator == "Yes") ? true : (data.POSTvars.MedicareIndicator == "No") ? false : null;
            for (var i = 0; i < this.Provider.ProviderFacilities.length; i++) {
              //var _pf = this.Provider.ProviderFacilities[i]; _pf.LastUpdatedDate = new Date(); _pf.LastUpdatedBy = data.POSTvars.User;
              var _pfr = this.Provider.ProviderFacilities[i].FPRelationship; _pfr.LastUpdatedDate = new Date(); _pfr.LastUpdatedBy = data.POSTvars.User;
              _pfr.ExternalProviderIndicator = data.POSTvars.ProviderFacilities[i].FPRelationship.ExternalProviderIndicator;
              _pfr.AcceptingNewPatientIndicator = data.POSTvars.ProviderFacilities[i].FPRelationship.AcceptingNewPatientIndicator; _pfr.PrescriberIndicator = data.POSTvars.ProviderFacilities[i].FPRelationship.PrescriberIndicator;
              _pfr.ReferralIndicator = data.POSTvars.ProviderFacilities[i].FPRelationship.ReferralIndicator; _pfr.FloatProviderIndicator = data.POSTvars.ProviderFacilities[i].FPRelationship.FloatProviderIndicator;
              //seq number, Phone, PhoneExt, EDATE, TDATE
              _pfr.SequenceNumber = i + 1; _pfr.ProviderPhoneNumber = data.POSTvars.ProviderPhone; _pfr.ProviderPhoneExtension = data.POSTvars.ProviderExtension;
              //_pfr.EffectiveDate  and TermDate already set in pFacLocal, like with Specs (and pSpecLocal) pre save
            }
            this.origFacOrder = this.currentFacOrder.replace(/\,/g, "|"); this.facsEdited = false;
            break;
          default: //log error + weird behavior
            break;
        }
      }
    );
  }

  public editProvider(type: number, event: any, entityID?: number, entityRelationshipID?: number) { //add optional parameters just for spec case, doesn't warrant a separate function right now. If that changes, make a separate function. Also can try to make the param an object with 2 fields.
    if (!this.canEdit()) { alert("You think you smart?"); return; } //can only get here when inspect hacking, request hacking, network packet sniff/insert hacking, etc...
    //every case except specialty edit will be the same, so we don't need a switch like with Save. if this changes, use a switch like with Save.
    let _editDivs: any = $(this.getEditDivsSelector(type, entityID, entityRelationshipID)); _editDivs.show(); _editDivs = null;
    let _notEditDivs: any = $(this.getNotEditDivsSelector(type, entityID, entityRelationshipID)); _notEditDivs.hide(); _notEditDivs = null;
    //combine handling for any case with datepickers: TODO
    if (type == 2) { //specialty-specific edit: show card if not expanded + init Datepickers if not already init
      let _effDate: any = document.getElementById("edit_ProviderSpec" + entityID + "_EffectiveDate"); let _termDate: any = document.getElementById("edit_ProviderSpec" + entityID + "_TerminationDate");
      if (_effDate.getAttribute("ph-initialized") == "false") {
        let jQ_effDate: any = $(_effDate); jQ_effDate.datepicker({ showOtherMonths: true, selectOtherMonths: true, changeMonth: true, changeYear: true, dateFormat: "M d, yy", prevText: "<", nextText: ">" });
        _effDate.setAttribute("ph-initialized", "true");
      }
      if (_termDate.getAttribute("ph-initialized") == "false") {
        let jQ_termDate: any = $(_termDate); jQ_termDate.datepicker({ showOtherMonths: true, selectOtherMonths: true, changeMonth: true, changeYear: true, dateFormat: "M d, yy", prevText: "<", nextText: ">" });
        _termDate.setAttribute("ph-initialized", "true");
      }
      if (document.getElementById('specialtyTable_' + entityID).style.display == "" || document.getElementById('specialtyTable_'+entityID).style.display != "table") {
          //$(event.target).parent().parent().children("table.specTable").toggle();
          //$(event.target).parent().parent().parent().children(".provSpecFooter,.provFacFooter");
          let cardfooter: any = $("#specFooter_" + entityID);cardfooter.toggle();
          let cardtable: any = $("#specialtyTable_" + entityID); cardtable.toggle();
      }
    }
    if (type == 3) {
      let _effDate: any = document.getElementById("edit_ProviderFP_" + entityRelationshipID + "_EffectiveDate"); let _termDate: any = document.getElementById("edit_ProviderFP_" + entityRelationshipID + "_TerminationDate");
      if (_effDate.getAttribute("ph-initialized") == "false") {
        let jQ_effDate: any = $(_effDate); jQ_effDate.datepicker({ showOtherMonths: true, selectOtherMonths: true, changeMonth: true, changeYear: true, dateFormat: "M d, yy", prevText: "<", nextText: ">" });
        _effDate.setAttribute("ph-initialized", "true");
      }
      if (_termDate.getAttribute("ph-initialized") == "false") {
        let jQ_termDate: any = $(_termDate); jQ_termDate.datepicker({ showOtherMonths: true, selectOtherMonths: true, changeMonth: true, changeYear: true, dateFormat: "M d, yy", prevText: "<", nextText: ">" });
        _termDate.setAttribute("ph-initialized", "true");
      }
    }
  }

  public cancelEdit(type: number, event: any, entityID?: number, entityRelationshipID?: number) { //add optional parameters just for spec case, doesn't warrant a separate function right now. If that changes, make a separate function. Also can try to make the param an object with 2 fields.
    if (!this.canEdit()) { alert("You think you smart?"); return; } //can only get here when inspect hacking, request hacking, network packet sniff/insert hacking, etc...

    $("#edit_Provider_LastName").val(this.Provider.LastName);
    $("#edit_Provider_FirstName").val(this.Provider.FirstName);
    $("#edit_Provider_EpicProviderID").val(this.Provider.EpicProviderID);
    $("#edit_Provider_NPI").val(this.Provider.NPI);
    $("#edit_Provider_Gender").val(this.Provider.Gender);
    $("#edit_ProviderDemo_MedicarePTAN").val(this.Provider.MedicarePTAN);
    var transformDate = new DatePipe('en-us');
    $("#edit_ProviderDemo_MedicareEffectiveDate").val(transformDate.transform(this.Provider.MedicareEffectiveDate));
    $("#edit_ProviderDemo_MedicareTerminationDate").val(transformDate.transform(this.Provider.MedicareTerminationDate));

    let _editDivs: any = $(this.getEditDivsSelector(type, entityID, entityRelationshipID)); _editDivs.hide(); _editDivs = null;
    let _notEditDivs: any = $(this.getNotEditDivsSelector(type, entityID, entityRelationshipID)); _notEditDivs.show(); _notEditDivs = null;

    if (type == 2) { //specialty-specific cancel: hide card if not hidden
      if (document.getElementById('specialtyTable_'+entityID).style.display == "table") {
        let cardfooter: any = $("#specFooter_" + entityID); cardfooter.toggle();
        let cardtable: any = $("#specialtyTable_" + entityID); cardtable.toggle();
      }
    }
  }

  public loading(isLoading, saveType) {
    let loadOverlay: any = $("#loadOverlay" + saveType);
    if (isLoading) { loadOverlay.show(); } else { loadOverlay.hide(); }
  }

}
