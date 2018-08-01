import { Component, Output, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';
import { environment } from '../../environments/environment';
import * as $ from 'jquery';
import 'datatables.net';
import 'datatables.net-bs4';
import 'datatables.net-select-bs4';

@Component({
  selector: 'app-facilitysearch',
  templateUrl: './facilitysearch.component.html',
  styleUrls: ['./facilitysearch.component.scss']
})
export class FacilitysearchComponent implements OnInit {
  public tableWidget: any;
  public apiRoot: string;
  public title: string = "ProviderHub - Facility Search";

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
    let exampleId: any = $("#facilities");
    this.tableWidget = exampleId.DataTable({
      select: true,
      paging: false,
      language: { search: "", searchPlaceholder: "Begin typing a Facility Name, City, or Zip Code to filter search results" },
      ajax: {
        url: this.apiRoot + "Search/GetFacilityList",
        type: "POST",
        xhrFields: { withCredentials: true },
        dataSrc: ''
      },
      columns: [{ data: "FacilityName" }, {
        data: null, render: function (data, type, row) {
          var d = data.FacilityAddress; var a1 = d.AddressLine1 == null ? "" : d.AddressLine1; var a2 = d.AddressLine2 == null ? "" : d.AddressLine2;
          return a1 + "<br/>" + a2;
        }, searchable: false
      },
        { data: "FacilityAddress.City" }, { data: "FacilityAddress.State", searchable: false }, { data: "FacilityAddress.ZipCode" }, { data: "FacilityAddress.PhoneNumber" }/*, { data: "FacilityAddress.Region", visible:false, searchable:true}*/
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
      document.getElementById('facilities').getElementsByTagName('tbody')[0].style.visibility = (document.getElementById('facilities_filter').getElementsByTagName('input')[0].value.length < 2) ? "hidden" : "visible";
    });
  }
  private onRowSelect(indexes: number[]): void {
    var facilityId = indexes[0];
    var full_name = $("#" + facilityId + " td:nth-child(1)").text();
    // + " at" + $("#" + facilityId + " td:nth-child(2)").text(); alert(full_name);
    API.selectedFacility = full_name;
    this.router.navigate(['/facility',facilityId]);
  }

}
