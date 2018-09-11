import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { environment } from '../../environments/environment';
import { ProviderHubService } from '../app.service';
import { CommonModule, Location } from '@angular/common';
import { GenderPipe, NullablePipe, BoolPipe, SpecialtyTypePipe, ParentSpecialtyPipe, NoValuePipe, PHDatePipe, SpecStatusPipe } from '../pipes';
import * as $ from 'jquery';
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
  public editingDivWrappers: any;
  public editingDivHeaderWrappers: any;
  public editingHeaderDivs: any;

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
    
    this.service.hitAPI(this.apiRoot + "Provider/ByID/" + this.providerId).subscribe(
      data => {
        //0. Edit/Save buttons and Misc UI
        for (var i = 0; i < this.editingDivWrappers.length; i++) {
          let _editDivs: any = $(this.editingDivHeaderWrappers[i] + " i.is-editing," + this.editingDivWrappers[i] + " .is-editing"); _editDivs.hide(); _editDivs = null;
          let _notEditDivs: any = $(this.editingDivHeaderWrappers[i] + " i.not-editing," + this.editingDivWrappers[i] + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
        }
        //1. Main and Demo
        this.Provider = data.p; var _c = this.Provider.CredentialListStr; console.log(this.Provider);
        var _credArr = this.Provider.CredentialListStr.split(","); var _langArr = this.Provider.Languages.split(",");
        for (var i = 0; i < _credArr.length; i++) { _credArr[i] = _credArr[i].trim(); } for (var i = 0; i < _langArr.length; i++) { _langArr[i] = _langArr[i].trim(); }
        this.Provider.LastUpdatedDate = this.Provider.LastUpdatedDate.replace(/\D/g, '').slice(0,-4);
        this.Provider.Credentials = (_c==null)? "" : _c.slice(0, -1).replace(/,/g,", ");//trailing comma
        document.getElementById("page-title").innerHTML = this.Provider.FirstName + " " + this.Provider.LastName + ", " + this.Provider.Credentials;
        //MEDICARE DATES
        this.Provider.MedicareEffectiveDate = (this.Provider.MedicareEffectiveDate && this.Provider.MedicareEffectiveDate != null) ? this.Provider.MedicareEffectiveDate.replace(/\D/g, '').slice(0, -4) : "";
        this.Provider.MedicareTerminationDate = (this.Provider.MedicareTerminationDate && this.Provider.MedicareTerminationDate != null) ? this.Provider.MedicareTerminationDate.replace(/\D/g, '').slice(0, -4) : "";
        //FAKE MEDICARE+MEDICAID INFO TO TEST MEDICARE+MEDICAID UI WITH MARY KARLS (id:231)
        if (this.Provider.FirstName == "Mary " && this.Provider.LastName == "Karls")
        {
          this.Provider.MedicareIndicator = true; this.Provider.MedicaidIndicator = true; this.Provider.MedicarePTAN = "9284654";
          this.Provider.MedicareEffectiveDate = "Jan 1, 2015"; this.Provider.MedicareTerminationDate = "Jan 1, 2019"; this.Provider.MedicaidProviderID = "1825465";
        }
        //1b. SELECTIZE from 'full entity' lists: Credentials and Languages
        let toSelectize: any = $("#edit_Provider_Credentials,#edit_ProviderDemo_Language");
        //POPULATE CREDENTIALS AND LANGUAGES SELECT TAG, THEN SET PROVIDER'S CURRENT VALUES AS SELECTED OPTION(S)
        let credsList: any = data.c; let languagesList: any = data.l; var lSelectHTML, cSelectHTML = "";
        let lSelect: any = $("#edit_ProviderDemo_Language"); let cSelect: any = $("#edit_Provider_Credentials");
        for (var i = 0; i < credsList.length; i++) { var selected = (_credArr.includes(credsList[i].Value)) ? "selected" : ""; cSelectHTML += "<option value='" + credsList[i].ID + "' " + selected + ">" + credsList[i].Value + " - " + credsList[i].Description + "</option>"; }
        for (var i = 0; i < languagesList.length; i++) { var selected = (_langArr.includes(languagesList[i].Name)) ? "selected" : ""; lSelectHTML += "<option value='" + languagesList[i].ID +"' " + selected + ">"+languagesList[i].Name+"</option>"; }
        lSelect.html("<select>"+lSelectHTML+"</select>"); cSelect.html("<select>"+cSelectHTML+"</select>");
        //SELECTIZE ALL SELECTS, SEND VAR TO GARBAGE COLLECTOR
        toSelectize.selectize(); toSelectize = null;
        //"BOOL" SELECTS (YES/NO/UNKNOWN) NOT TO BE SELECTIZED. JUST SET PROVIDER'S CURRENT VALUE AS SELECTED
        //2. Specialties
        // Object spec (per Provider):
        //  ProviderSpecialties:[{Specialty},{Specialty},...]
        // Each Specialty = {ID,Name,Description,MappingID,SequenceNumber,CreatedDate,
        // CreatedBy,EffectiveDate,TerminationDate,SpecialtyType,ParentSpecialtyID,ParentName}
        for (var i = 0; i < this.Provider.ProviderSpecialties.length; i++) {
          var s = this.Provider.ProviderSpecialties[i];
          s.EffectiveDate = s.EffectiveDate.replace(/\D/g, '').slice(0, -4);
          s.TerminationDate = (s.TerminationDate == null) ? '' : s.TerminationDate.replace(/\D/g, '').slice(0, -4);
          s.LastUpdatedDate = (s.LastUpdatedDate == null) ? '' : s.LastUpdatedDate.replace(/\D/g, '').slice(0, -4);
          s.Status = new SpecStatusPipe().transform(s);
          //console.log(s);
        }
        let list: any = $('#specList'); list.sortable();
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

  public onSpecClick(event: any) {
    $(event.target).parent().children("table.specTable").toggle();
    $(event.target).parent().parent().children(".provSpecFooter,.provFacFooter").toggle();
  }

  public setSortable(event: any) {
    alert("set sortable!");
  }

  public saveProvider(type: number, event: any) {
    let _editDivs: any = $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing"); _editDivs.hide(); _editDivs = null;
    let _notEditDivs: any = $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
    this.loading(true, type);//load starting, show overlay
    function val(which) { let _e: any = $("#edit_Provider_" + which); return _e.val(); }//MULTI SELECT VALS ARE ID ARRAYS
    function val2(which) { let _e: any = $("#edit_ProviderDemo_" + which); return _e.val(); } let body: any;
    switch (type) {
      case 0: //"Main" Provider-Header Info
        body = { FirstName: val("FirstName"), LastName: val("LastName"), Credentials: val("Credentials"), NPI: val("NPI"), EpicProviderID: val("EpicProviderID"), Gender: val("Gender"), User: "GHC-HMO\\spillai" };
        break;
      case 1: //Demographics
        body = { Languages: val2("Language"), MedicareIndicator: val2("MedicareIndicator"), MedicaidIndicator: val2("MedicaidIndicator"), User: "GHC-HMO\\spillai" };
        if (body.MedicareIndicator == "Yes") { body.MedicarePTAN = val2("MedicarePTAN"); body.MedicareEffectiveDate = this.transformDateForPHDB("edit_ProviderDemo_MedicareEffectiveDate"); body.MedicareTerminationDate = this.transformDateForPHDB("edit_ProviderDemo_MedicareTerminationDate"); }
        //val2("MedicareEffectiveDate"), val2("MedicareTerminationDate")
        if (body.MedicaidIndicator == "Yes") { body.MedicaidProviderID = val2("MedicaidProviderID"); }
        break;
      case 2: //Specialties
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
            //header data to just transform over (replace with partial arr matching function)
            this.Provider.FirstName = data.POSTvars.FirstName; this.Provider.LastName = data.POSTvars.LastName;
            this.Provider.NPI = data.POSTvars.NPI; this.Provider.EpicProviderID = data.POSTvars.EpicProviderID;
            this.Provider.Gender = data.POSTvars.Gender; this.Provider.LastUpdatedBy = data.POSTvars.User;
            this.Provider.LastUpdatedDate = new Date();//<-- TODO: test and if it doesn't auto-pipe to today's date, manually format like API return's date
            //break down creds arr and build creds string
            var _c = ""; for (var i = 0; i < data.POSTvars.Credentials.length; i++) {
              let _e: any = $("#edit_Provider_Credentials option[value='" + data.POSTvars.Credentials[i] + "']");
              _c += _e.text().substring(0, _e.text().indexOf(" - ")); _c += (i==data.POSTvars.Credentials.length-1)? "" : "," ;
            }
            this.Provider.Credentials = _c;
            break;
          case 1:
            //demo data to just transform over (also replace with partial arr matching fxn)
            this.Provider.MedicareIndicator = (data.POSTvars.MedicareIndicator == "Yes") ? true : (data.POSTvars.MedicareIndicator == "No") ? false : null;
            this.Provider.MedicaidIndicator = (data.POSTvars.MedicaidIndicator == "Yes") ? true : (data.POSTvars.MedicaidIndicator == "No") ? false : null;
            this.Provider.MedicarePTAN = (this.Provider.MedicareIndicator === true) ? data.POSTvars.MedicarePTAN : null;//need ===true since it's nullable bit
            this.Provider.MedicareEffectiveDate = (this.Provider.MedicareIndicator === true) ? data.POSTvars.MedicareEffectiveDate : null;// TODO: format dates for datepipe/display, see what UI currently does first
            this.Provider.MedicareTerminationDate = (this.Provider.MedicareIndicator === true) ? data.POSTvars.MedicareTerminationDate : null;// TODO: format dates for datepipe/display, see what UI currently does first
            this.Provider.MedicaidProviderID = (this.Provider.MedicareIndicator === true) ? data.POSTvars.MedicaidProviderID : null;
            //break down lang arr like we did the creds arr
            var _l = ""; for (var i = 0; i < data.POSTvars.Languages.length; i++) {
              let _e: any = $("#edit_ProviderDemo_Language option[value='" + data.POSTvars.Languages[i] + "']");
              _l += _e.text(); _l += (i == data.POSTvars.Languages.length - 1) ? "" : ",";
            }
            this.Provider.Languages = _l;
            break;
          case 2:
            break;
          default: //log error + weird behavior
            break;
        }
      }
    );
  }

  public editProvider(type: number, event: any) {
    let _editDivs: any = $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing"); _editDivs.show(); _editDivs = null;
    let _notEditDivs: any = $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing"); _notEditDivs.hide(); _notEditDivs = null;
  }

  public cancelEdit(type: number, event: any) {
    let _editDivs: any = $(this.editingDivHeaderWrappers[type] + " i.is-editing," + this.editingDivWrappers[type] + " .is-editing"); _editDivs.hide(); _editDivs = null;
    let _notEditDivs: any = $(this.editingDivHeaderWrappers[type] + " i.not-editing," + this.editingDivWrappers[type] + " .not-editing"); _notEditDivs.show(); _notEditDivs = null;
  }

  public loading(isLoading, saveType) {
    let loadOverlay: any = $("#loadOverlay" + saveType);
    if (isLoading) { loadOverlay.show(); } else { loadOverlay.hide(); }
  }

}
