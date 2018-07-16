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
      columns: [{ data: "ID", searchable: false }, { data: "NPI" }, { data: null, render: function (data, type, row) { var d = data; return d.FirstName + " " + d.LastName; } }],
      initComplete: function (settings, json) {
        //alert("loaded");
      }
      /*columns: [{data:"firstName"}, {data:"lastName"}],
      data: [{ firstName: "sree", lastName: "pill" }, { firstName: "sree2", lastName: "pill2" }]*/
    });
    this.tableWidget.on('select',
      (e, dt, type, indexes) => this.onRowSelect(indexes));
  }
  private onRowSelect(indexes: number[]): void {
    this.rowSelected.emit(indexes[0]);
    console.log(indexes[0]);
  }

}
