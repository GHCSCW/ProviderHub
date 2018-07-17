import { Component, Output, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as $ from 'jquery';
import 'datatables.net';
import 'datatables.net-bs4';
import 'datatables.net-select-bs4';

@Component({
  selector: 'app-datatable',
  templateUrl: './datatable.component.html',
  styleUrls: ['./datatable.component.scss']
})
export class DatatableComponent implements OnInit {
  public tableWidget: any;
  public apiRoot: string;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route
      .data
      .subscribe(v => this.apiRoot = v.apiRoot);
    console.log(this.apiRoot);
  }

  ngAfterViewInit() {
    this.initDatatable();
  }

  @Output() rowSelected: EventEmitter<number> = new EventEmitter();
  private initDatatable(): void {
    let exampleId: any = $('#providers');
    this.tableWidget = exampleId.DataTable({
      select: true,
      paging: false,
      language: { search: "", searchPlaceholder: "Begin typing in a Provider Name or NPI to filter search results" },
      ajax: {
        url: this.apiRoot + "Search/GetProviderList",
        type: "POST",
        xhrFields: { withCredentials: true },
        dataSrc: ''
      },
      columns: [{ data: "NPI" }, { data: "LastName" }, { data: "FirstName" },
        { data: null, render: function (data, type, row) { var d = data.CredentialList; var r=""; for (var i = 0; i < d.length; i++) { r += d[i].Value + ","; } if (r.length > 0) { r=r.substring(0, r.length - 1) } return r; }, searchable:false },
        { data: null, render: function (data, type, row) { var d = data; var r; switch (d.Gender) { case 1: r = "Female"; break; case 2: r = "Male"; break; default: r = "Other"; break; } return r; }, searchable: false },
        { data: null, render: function (data, type, row) { var d = data.ProviderSpecialties; for (var i = 0; i < d.length; i++) { if (d[i].SequenceNumber == 1) { return d[i].Value; } } return ""; }, searchable: false  },
        { data: null, render: function (data, type, row) { var d = data; return ""; }, searchable:false }
      ],
      order: [[1, "asc"]],
      initComplete: function (settings, json) {
        //alert("loaded");
      }
      //NPI, Last name, First Name, Credential, Gender, Primary Specialty, Vendor.
      //Full name:{ data: null, render: function (data, type, row) { var d = data; return d.FirstName + " " + d.LastName; } }
      /*columns: [{data:"firstName"}, {data:"lastName"}],
      data: [{ firstName: "sree", lastName: "pill" }, { firstName: "sree2", lastName: "pill2" }]*/
    });
    this.tableWidget.on('select',
      (e, dt, type, indexes) => this.onRowSelect(indexes));
  }
  private onRowSelect(indexes: number[]): void {
    this.rowSelected.emit(indexes[0]);
    console.log(indexes[0]);
    //load addtional data for provider @ index
  }

}
