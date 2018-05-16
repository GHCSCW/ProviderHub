import { Component, Inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl } from "@angular/forms";
import { FormBuilder, Validators, FormsModule, NgForm, FormGroup } from '@angular/forms';
import { MentalHealthService } from '../../services/mental.health.service';

import 'rxjs/Rx';
import { Router } from '@angular/router';
import { NavbarService } from '../../services/navbarservice';
import { catchError, map, tap, filter, startWith, switchMap, debounceTime, distinctUntilChanged, takeWhile, first } from 'rxjs/operators';
import { Observable } from 'rxjs/Observable';
export class Language {
  constructor(public name: string) { }
}


@Component({
  selector: 'mapprovidertoclinic',
  styleUrls: ['./map-provider-to-clinic.component.css'],
  templateUrl: './map-provider-to-clinic.component.html'

})

export class MapProviderToClinic implements OnInit {

  filteredOptions: Observable<any[]>;
  clinicList: any = [];
  myControl = new FormControl();

  constructor(
    private mentalHealthService: MentalHealthService,
    private router: Router,
    public nav: NavbarService) {

  }


  ngOnInit() {

     this.mentalHealthService.getFacilityList().subscribe(data => {
      this.clinicList = data;
    });
  

  //this.filteredOptions = this.myControl.valueChanges
  //  .pipe(
  //  startWith(null),
  //  debounceTime(200),
  //  distinctUntilChanged(),
  //  switchMap(val => {
  //    return this.filter(val || '')
  //  })
  //  );

}

  //filter(val: string): Observable<any[]> {
  //  return this.mentalHealthService.getFacilityList()
  //    .pipe(
  //    map(response => response.filter(option => {
  //      return option.FacilityName.toLowerCase().indexOf(val.toLowerCase()) >= 0
  //    }))
  //    )
  //}

     

}
