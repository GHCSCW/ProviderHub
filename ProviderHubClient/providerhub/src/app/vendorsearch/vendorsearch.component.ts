import { Component, Output, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { environment } from '../../environments/environment';
import * as $ from 'jquery';
import 'datatables.net';
import 'datatables.net-bs4';
import 'datatables.net-select-bs4';

@Component({
  selector: 'app-vendorsearch',
  templateUrl: './vendorsearch.component.html',
  styleUrls: ['./vendorsearch.component.scss']
})
export class VendorsearchComponent implements OnInit {

  public tableWidget: any;
  public apiRoot: string;
  public title: string = "ProviderHub - Vendor Search";

  constructor(private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.apiRoot = environment.apiRoot;
    document.getElementById("page-title").innerHTML = this.title;
  }

  ngAfterViewInit() {
    this.initDatatable();
  }

  @Output() rowSelected: EventEmitter<number> = new EventEmitter();
  private initDatatable(): void {
    let exampleId: any = $("#vendors");
    this.tableWidget = exampleId.DataTable({
      select: true,
      paging: false,
      language: { search: "", searchPlaceholder: "Begin typing a Vendor Name, NPI, City, or Zip Code to filter search results" },
      ajax: {
        url: this.apiRoot + "Search/GetVendorList",
        type: "POST",
        xhrFields: { withCredentials: true },
        dataSrc: ''
      },
      /*th>VendorName</th>
      <th>NPI</th>
      <th>TaxID</th>
      <th>EPICVendorID</th> then City State ZIP
      <th>InternalNotes</th>*/
      columns: [
        { data: "VendorName" }, { data: "NPI" }, { data: "TaxID", searchable: false }, { data: "EPICVendorID", searchable: false },
        { data: "City" }, { data: "State", searchable: false }, { data: "ZipCode" },
        { data: "InternalNotes", searchable: false }/*, { data: "FacilityAddress.Region", visible:false, searchable:true}*/
        /*{
          data: null, render: function (data, type, row) {
            var d = data.VendorAddress; var a1 = d.AddressLine1 == null ? "" : d.AddressLine1; var a2 = d.AddressLine2 == null ? "" : d.AddressLine2;
            return a1 + "<br/>" + a2;
          }, searchable: false
        }*/
      ],
      order: [[0, "asc"]],
      rowId: 'ID',
      initComplete: function (settings, json) { }
    });
    this.tableWidget.on('select',
      (e, dt, type, indexes) => {
        console.log(this.tableWidget.rows(indexes).data().pluck("ID"));
        this.onRowSelect(this.tableWidget.rows(indexes).data().pluck("ID"));
      }
    );
    this.tableWidget.on('search.dt', () => {
      document.getElementById('vendors').getElementsByTagName('tbody')[0].style.visibility = (document.getElementById('vendors_filter').getElementsByTagName('input')[0].value.length < 2) ? "hidden" : "visible";
    });
  }
  private onRowSelect(indexes: number[]): void {
    var facilityId = indexes[0];
    var full_name = $("#" + facilityId + " td:nth-child(1)").text();
    // + " at" + $("#" + facilityId + " td:nth-child(2)").text(); alert(full_name);
    API.selectedVendor = full_name;
    this.router.navigate(['/vendor', facilityId]);
  }

}
