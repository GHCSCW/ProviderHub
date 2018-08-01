import { Component, Output, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { environment } from '../../environments/environment';
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
  public title: string = "Welcome to ProviderHub!";

  constructor(private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    //this.route.data.subscribe(v => this.componentProperty = v.routeDataProperty);
    this.apiRoot = environment.apiRoot;
    document.getElementById("page-title").innerHTML = this.title;
  }

  ngAfterViewInit() {
    this.initDatatable();
  }

  @Output() rowSelected: EventEmitter<number> = new EventEmitter();
  private initDatatable(): void {
    let exampleId: any = $('#providers');
    let searchId: any = $('#providers_filter label input[type="search"]');
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
        { data: null, render: function (data, type, row) { var d = data.CredentialListStr; return (d==null)? "" : d.slice(0, -1); }, searchable:false },
        { data: null, render: function (data, type, row) { var d = data; var r; switch (d.Gender) { case 1: r = "Female"; break; case 2: r = "Male"; break; default: r = "Other"; break; } return r; }, searchable: false },
        { data: null, render: function (data, type, row) { var d = data.PrimarySpecialty; return d; }, searchable: false  },
        { data: null, render: function (data, type, row) { var d = data; return ""; }, searchable:false }
      ],
      order: [[1, "asc"]],
      rowId: 'ID',
      initComplete: function (settings, json) {
        //console.log("loaded");
      }
    });
    this.tableWidget.on('select',
      (e, dt, type, indexes) => {
        console.log(this.tableWidget.rows(indexes).data().pluck("ID"));
        this.onRowSelect(this.tableWidget.rows(indexes).data().pluck("ID"));
      }
    );
    this.tableWidget.on('search.dt', () => {
      document.getElementById('providers').getElementsByTagName('tbody')[0].style.visibility = (document.getElementById('providers_filter').getElementsByTagName('input')[0].value.length < 2) ? "hidden" : "visible";
    });
  }
  private onRowSelect(indexes: number[]): void {
    var providerId = indexes[0];
    var full_name = $("#" + providerId + " td:nth-child(3)").text() + " " + $("#" + providerId + " td:nth-child(2)").text();
    //console.log(full_name);
    API.selectedProvider = full_name;
    this.router.navigate(['/provider',providerId]);
  }

}
