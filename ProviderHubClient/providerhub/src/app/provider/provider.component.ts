import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { environment } from '../../environments/environment';
import { ProviderHubService } from '../app.service';
import { CommonModule, Location } from '@angular/common';
import { GenderPipe, NullablePipe, BoolPipe, SpecialtyTypePipe, ParentSpecialtyPipe, NoValuePipe, PHDatePipe, SpecStatusPipe } from '../pipes';
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
  public editingDivWrappers: any;
  public editingDivHeaderWrappers: any;
  public editingHeaderDivs: any;
  public _specsList: any;
  public origSpecOrder: any = null;
  public currentSpecOrder: any = null;
  public defaultSpecDate: any = null;
  public networksDT: any;


  constructor(private route: ActivatedRoute, private router: Router,
              private service: ProviderHubService, private location: Location) {
    this.Service = service; this.Provider = {};
    //"(Primary)" to mark primary credential takes up too much space for no benefit
    this.nav = 'Networks'; //default tab should be Demographics ***SKP 11/6: NOW NETWORKS***
    this.specsEdited = false;
  }

  ngOnInit() {
    this.apiRoot = environment.apiRoot;
    this.route.params.subscribe(params => {
      this.providerId = +params['id'];
      if (params['tabURL']) { this.initialTab = params['tabURL']; } else {
        this.location.replaceState("/Provider/Demographics/" + this.providerId);
      }
    });
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
          _dis.editProvider(_i, event);
        });
        divs.getElementsByClassName("is-editing")[0].addEventListener("click", function (event) { //save
          _dis.saveProvider(_i, event);
        });
        divs.getElementsByClassName("is-editing")[1].addEventListener("click", function (event) { //cancel
          _dis.cancelEdit(_i, event);
        });
      })();
    }
    //PROVIDER SPEC EDIT, could be incorporated into above to save code, but enough is different about it that I separated it. - SKP
    //POTENTIAL OPTIMIZE, but makes code harder to read: convert to plain jane JS (like above click events), might save 100-300ms
    let _addSpec: any = $("#addSpec"); let _resetSpec: any = $("#resetSpecs"); let _saveSpec: any = $("#saveSpecs"); let _addSpecBody: any = $("#addSpecBody");
    _addSpec.click(function () {
      _addSpecBody.show(); //_addSpec.hide(); _resetSpec.show(); _saveSpec.show();
      _dis.specsEdited = true;
    });
    _saveSpec.click(function (e) {
      var forIDs = _dis.Provider.ProviderSpecialties[0];
      _dis.saveProvider(2, e, forIDs.ID, forIDs.MappingID);
    });
    _resetSpec.click(function () {
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
    });
    
    this.service.hitAPI(this.apiRoot + "Provider/ByID/" + this.providerId).subscribe(
      data => {
        //0. Edit/Save buttons and Misc UI
        for (var i = 0; i < this.editingDivWrappers.length; i++) {
          let _editDivs: any = $(this.editingDivHeaderWrappers[i] + " i.is-editing," + this.editingDivWrappers[i] + " .is-editing"); _editDivs.hide(); _editDivs = null;
          let _notEditDivs: any = $(this.editingDivHeaderWrappers[i] + " i.not-editing," + this.editingDivWrappers[i] + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
        }
        //1. Main and Demo
        this.Provider = data.p; var _c = this.Provider.CredentialListStr; this.Provider.NetworkTab = data.net;
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
        let credsList: any = data.c; let languagesList: any = data.l; let specsList: any = data.s; var lSelectHTML, cSelectHTML, sSelectHTML = ""; this._specsList = [];
        let lSelect: any = $("#edit_ProviderDemo_Language"); let cSelect: any = $("#edit_Provider_Credentials"); let sSelect: any = $("#add_Provider_Specialty");
        for (var i = 0; i < credsList.length; i++) { var selected = (_credArr.includes(credsList[i].Value)) ? "selected" : ""; cSelectHTML += "<option value='" + credsList[i].ID + "' " + selected + ">" + credsList[i].Value + " - " + credsList[i].Description + "</option>"; }
        for (var i = 0; i < languagesList.length; i++) { var selected = (_langArr.includes(languagesList[i].Name)) ? "selected" : ""; lSelectHTML += "<option value='" + languagesList[i].ID + "' " + selected + ">" + languagesList[i].Name + "</option>"; }
        for (var i = 0; i < specsList.length; i++) {
          sSelectHTML += "<option value='" + specsList[i].ID + "'>" + specsList[i].Name + "</option>";
          var toAdd = specsList[i]; toAdd.SequenceNumber = this.Provider.ProviderSpecialties.length; toAdd.MappingID = 0; toAdd.ID =specsList[i].ID; toAdd.LastUpdatedBy = environment.authUser.username;
          toAdd.EffectiveDate = "/Date(1451628000000-0600)/".replace(/\D/g, '').slice(0, -4); toAdd.TerminationDate = ''; toAdd.LastUpdatedDate = new Date();//use datepipe if needed
          toAdd.Status = "ACTIVE"; toAdd.ParentName = ''; toAdd.ParentSpecialtyID = 0; this._specsList[toAdd.ID]=toAdd;
        }
        sSelect.html("<select>" + sSelectHTML + "</select>"); lSelect.html("<select>"+lSelectHTML+"</select>"); cSelect.html("<select>"+cSelectHTML+"</select>");
        //SELECTIZE ALL SELECTS, SEND VAR TO GARBAGE COLLECTOR. IF DRAG_DROP DOESNT WORK WITH SINGLE SELECTS, MOVE SPEC SELECTIZE TO ITS OWN INITIALIZATION
        toSelectize.selectize({plugins:['drag_drop']}); toSelectize = null;
        //"BOOL" SELECTS (YES/NO/UNKNOWN) NOT TO BE SELECTIZED. JUST SET PROVIDER'S CURRENT VALUE AS SELECTED
        //2. Specialties
        // Object spec (per Provider):
        //  ProviderSpecialties:[{Specialty},{Specialty},...]
        // Each Specialty = {ID,Name,Description,MappingID,SequenceNumber,CreatedDate,
        // CreatedBy,EffectiveDate,TerminationDate,SpecialtyType,ParentSpecialtyID,ParentName}
        //this.origSpecOrder.split("|");
        this.origSpecOrder = ""; this.currentSpecOrder = "";
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
          initComplete: function (settings, json) {
            if (_dis.nav != 'Providers') { $("#providersTableDT_wrapper").hide(); } else { $("#providersTableDT_wrapper").show(); } /*cant do in Angular since generated by DT*/
            if (typeof (Event) === 'function') { window.dispatchEvent(new Event('resize')); }
            else { var evt = window.document.createEvent('UIEvents'); evt.initUIEvent('resize', true, false, window, 0); window.dispatchEvent(evt); }
          }
        });
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
        //3. Additional properties for UI conditionals ('novalue' pipe doesn't work in HTML??)
        for (var i = 0; i < this.Provider.ProviderFacilities.length; i++) {
          var f = this.Provider.ProviderFacilities[i];
          f.FacilityAddress.HidePhoneExtension = new NoValuePipe().transform(f.FacilityAddress.PhoneExtension);
          f.FacilityAddress.HideAlternatePhoneNumber = new NoValuePipe().transform(f.FacilityAddress.AlternatePhoneNumber);
          var fp = f.FPRelationship; fp.LastUpdatedDate = new PHDatePipe().transform(fp.LastUpdatedDate.replace(/\D/g, '').slice(0, -4));
          //console.log(f);
        }
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
    window.addEventListener('resize', function (event) {
      for (var j = 0; j < document.getElementsByClassName('provFacFooter').length; j++) {
        (document.getElementsByClassName('provFacFooter')[j] as HTMLElement).style.paddingRight = document.getElementsByClassName('provFacTitle')[0].clientWidth - document.getElementsByClassName('midHead')[0].clientWidth - 45 + "px";
      }
    });
  }

  public transformDateForPHDB(datepicker_element_id: any) {
    var _e = document.getElementById(datepicker_element_id) as HTMLFormElement; var d = new Date(_e.value); var _jqe: any = $("#"+datepicker_element_id);
    var datestring = (_e.value=="" || _jqe.val().trim()=="")? null : d.getFullYear() + "-" + ("0" + (d.getMonth() + 1)).slice(-2) + "-" + ("0" + d.getDate()).slice(-2); return datestring;
  }

  public toggleInactiveSpecialties() {
    let inactiveSpecs: any = $("div.status_INACTIVE");
    inactiveSpecs.toggle();
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
    console.log(this.Provider.ProviderSpecialties);
  }

  private onRowSelect(indexes: number[]): void {
    var providerId = indexes[0];
    console.log(providerId);
    console.log($("tr#" + providerId));
    var row = this.networksDT.row($("tr#" + providerId)[0]);
    if (row.child.isShown()) {
      row.child.hide();
    } else {
      row.child(format(row.data()), 'expandedFP').show();
    }
    function format(d) {
      return "<table class='plainjane'>" + "<tr><td><b>DIRECTORY ID:" + d.ID + "</b></td>"
        + "<td><span class='childRowHeader'>External Provider</span> <br/>" + new BoolPipe().transform(d.FPRelationship.ExternalProviderIndicator) + "</td>"
        + "<td>Accepting New Patients <br/>" + new BoolPipe().transform(d.FPRelationship.AcceptingNewPatientIndicator) + "</td>"
        + "<td>Prescriber <br/>" + new BoolPipe().transform(d.FPRelationship.PrescriberIndicator) + "</td>"
        + "<td>Referral <br/>" + new BoolPipe().transform(d.FPRelationship.ReferralIndicator) + "</td>"
        + "<td>Directory Effective Date <br/>" + new PHDatePipe().transform(d.DirectoryEffectiveDate.replace(/\D/g, '').slice(0, -4)) + "</td>"
        + "<td>Directory Termination Date <br/>" + new PHDatePipe().transform(d.DirectoryTerminationDate.replace(/\D/g, '').slice(0, -4)) + "</td>"
        + "<td>PCP Eligible <br/>" + new BoolPipe().transform(d.PCPEligibleIndicator) + "</td>" //BRANDON - When you create new PCPEligible in Directory table, pull from that field instead of FPRelationship
        + "<td>Float Provider <br/>" + new BoolPipe().transform(d.FPRelationship.FloatProviderIndicator) + "</td></tr>";
    }
  }

  public onSpecClick(event: any) {
    $(event.target).parent().children("table.specTable").toggle();
    $(event.target).parent().parent().children(".provSpecFooter,.provFacFooter").toggle();
  }

  public setSortable(event: any) {
    alert("set sortable!");
  }

  public saveProvider(type: number, event: any, specID?: number, specRelationshipID?: number) {
    let _editDivs: any = $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing"); _editDivs.hide(); _editDivs = null;
    let _notEditDivs: any = $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
    this.loading(true, type);//load starting, show overlay
    function val(which) { let _e: any = $("#edit_Provider_" + which); return _e.val(); }//MULTI SELECT VALS ARE ID ARRAYS
    function val2(which) { let _e: any = $("#edit_ProviderDemo_" + which); return _e.val(); } let body: any;
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
        //reorganize specs first
        var specOrderArr = this.currentSpecOrder.split(",");
        var _ProviderSpecialties = JSON.parse(JSON.stringify(this.Provider.ProviderSpecialties));/*<--DEEP CLONE, so no circular references*/ this.Provider.ProviderSpecialties = [];
        for (var i = 0; i < specOrderArr.length; i++) {
          for (var j = 0; j < _ProviderSpecialties.length; j++) {
            if (_ProviderSpecialties[j].ID == specOrderArr[i]) { this.Provider.ProviderSpecialties.push(_ProviderSpecialties[j]); break; }
          }
        }
        //Stored Proc needs: (@SpecialtyID VARCHAR(10),@User VARCHAR(20),@ID INT, @SEQ INT, @EDATE DATE, @TDATE DATE = NULL, @First BIT = 0) for each Specialty
        for (var i = 0; i < this.Provider.ProviderSpecialties.length; i++) {
          var provSpec = this.Provider.ProviderSpecialties[i];
          provSpec.EffectiveDate = this.transformDateForPHDB("edit_ProviderSpec" + provSpec.ID + "_EffectiveDate");
          provSpec.TerminationDate = this.transformDateForPHDB("edit_ProviderSpec" + provSpec.ID + "_TerminationDate");
        }
        body = { type: type, id: this.providerId }; body.ProviderSpecialties = this.Provider.ProviderSpecialties;
        break;
      default: //log error + weird behavior
        break;
    }    
    console.log(body);
    this.service.hitAPI(this.apiRoot + "Provider/Save/" + type + "/" + this.providerId, JSON.stringify(body)).subscribe(
      data => {
        console.log(data); this.loading(false, data.POSTvars.type);//load finished, hide overlay
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
            this.Provider.MedicareIndicator = (data.POSTvars.MedicareIndicator == "Yes") ? true : (data.POSTvars.MedicareIndicator == "No") ? false : null;
            this.Provider.MedicaidIndicator = (data.POSTvars.MedicaidIndicator == "Yes") ? true : (data.POSTvars.MedicaidIndicator == "No") ? false : null;
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
          case 2:
            let _editDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing") : $("#specWrapper_" + specID + " .is-editing"); _editDivs.hide(); _editDivs = null;
            let _notEditDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing") : $("#specWrapper_" + specID + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
            for (var i = 0; i < this.Provider.ProviderSpecialties.length; i++) { var _ps = this.Provider.ProviderSpecialties[i]; _ps.LastUpdatedDate = new Date(); _ps.LastUpdatedBy = data.POSTvars.User;}
            let _addSpec: any = $("#addSpec"); let _resetSpec: any = $("#resetSpecs"); let _saveSpec: any = $("#saveSpecs"); let _addSpecBody: any = $("#addSpecBody");
            this.origSpecOrder = this.currentSpecOrder.replace(/\,/g, "|"); this.specsEdited = false;
            break;
          default: //log error + weird behavior
            break;
        }
      }
    );
  }

  public editProvider(type: number, event: any, specID?: number, specRelationshipID?: number) { //add optional parameters just for spec case, doesn't warrant a separate function right now. If that changes, make a separate function. Also can try to make the param an object with 2 fields.
    //every case except specialty edit will be the same, so we don't need a switch like with Save. if this changes, use a switch like with Save.
    let _editDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing") : $("#specWrapper_" + specID + " .is-editing"); _editDivs.show(); _editDivs = null;
    let _notEditDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing") : $("#specWrapper_" + specID + " .not-editing"); _notEditDivs.hide(); _notEditDivs = null;
    if (type == 2) { //specialty-specific edit: show card if not expanded + init Datepickers if not already init
      let _effDate: any = document.getElementById("edit_ProviderSpec" + specID + "_EffectiveDate"); let _termDate: any = document.getElementById("edit_ProviderSpec" + specID + "_TerminationDate");
      if (_effDate.getAttribute("ph-initialized") == "false") {
        let jQ_effDate: any = $(_effDate); jQ_effDate.datepicker({ showOtherMonths: true, selectOtherMonths: true, changeMonth: true, changeYear: true, dateFormat: "M d, yy", prevText: "<", nextText: ">" }); _effDate.setAttribute("ph-initialized", "true");
      }
      if (_termDate.getAttribute("ph-initialized") == "false") {
        let jQ_termDate: any = $(_termDate); jQ_termDate.datepicker({ showOtherMonths: true, selectOtherMonths: true, changeMonth: true, changeYear: true, dateFormat: "M d, yy", prevText: "<", nextText: ">" }); _effDate.setAttribute("ph-initialized", "true");
      }
      if (document.getElementById('specialtyTable').style.display != "table") {
          $(event.target).parent().parent().children("table.specTable").toggle();
          $(event.target).parent().parent().parent().children(".provSpecFooter,.provFacFooter").toggle();
      }
    }
  }

  public cancelEdit(type: number, event: any, specID?: number, specRelationshipID?: number) { //add optional parameters just for spec case, doesn't warrant a separate function right now. If that changes, make a separate function. Also can try to make the param an object with 2 fields.
    let _editDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing") : $("#specWrapper_" + specID + " .is-editing"); _editDivs.hide(); _editDivs = null;
    let _notEditDivs: any = (type != 2) ? $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing") : $("#specWrapper_" + specID + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
    if (type == 2) { //specialty-specific cancel: hide card if not hidden
      if (document.getElementById('specialtyTable').style.display == "table") {
        $(event.target).parent().parent().children("table.specTable").toggle();
        $(event.target).parent().parent().parent().children(".provSpecFooter,.provFacFooter").toggle();
      }
    }
  }

  public loading(isLoading, saveType) {
    let loadOverlay: any = $("#loadOverlay" + saveType);
    if (isLoading) { loadOverlay.show(); } else { loadOverlay.hide(); }
  }

}
