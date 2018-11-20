import { BrowserModule } from '@angular/platform-browser';
import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { DatatableComponent } from './datatable/datatable.component';
import { DatatableModule } from './datatable/datatable.module';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { API } from './globals';
import { environment } from '../environments/environment';
import { HeaderComponent } from './header/header.component';
import { ProviderComponent } from './provider/provider.component';

import { ProviderHubService } from './app.service';
import { CommonModule } from '@angular/common';
import { NgSelectizeModule } from 'ng-selectize';
import { Http, Response, HttpModule } from '@angular/http';
import { HttpClientModule, HttpInterceptor, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CustomInterceptor } from './app.service';
import { FacilitysearchComponent } from './facilitysearch/facilitysearch.component';
import { FacilityComponent } from './facility/facility.component';
import { PresetValueExampleComponent } from './selectize/preset-value-example/presetValueExample.component';
import { GenderPipe, NullablePipe, BoolPipe, SpecialtyTypePipe, ParentSpecialtyPipe, PHDatePipe, NoValuePipe, SpecStatusPipe, PhoneFromDBPipe, PhoneToDBPipe, ReverseBoolPipe, TermStatusPipe } from './pipes';
import { VendorsearchComponent } from './vendorsearch/vendorsearch.component';
import { VendorComponent } from './vendor/vendor.component';
import { UwimportComponent } from './uwimport/uwimport.component';

const routes: Routes = [
  /*misc*/
  { path: 'uwimport', component: UwimportComponent },
  /*Root: Provider List & Basic Search (Name/NPI)*/
  //, data: { apiRoot: (environment.production) ? API.prod : API.dev }
  { path: 'Provider/Search', component: DatatableComponent },
  { path: '', redirectTo: '/Provider/Search', pathMatch: 'full' },
  /*Individual Provider View*/
  { path: 'provider/:id', component: ProviderComponent },
  { path: 'Provider/:tabURL/:id', component: ProviderComponent },
  /*Facilities Search*/
  { path: 'Facility/Search', component: FacilitysearchComponent },
  /*Individual Facility View*/
  { path: 'Facility/:tabURL/:id', component: FacilityComponent },
  { path: 'facility/:id', component: FacilityComponent },
  /*Vendor*/
  { path: 'vendor/:id', component: VendorComponent },
  { path: 'Vendor/:tabURL/:id', component: VendorComponent },
  { path: 'Vendor/Search', component: VendorsearchComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    //DatatableComponent,
    HeaderComponent,
    ProviderComponent,
    FacilitysearchComponent,
    FacilityComponent,
    VendorsearchComponent,
    VendorComponent,
    PresetValueExampleComponent,
    GenderPipe,
    NullablePipe,
    BoolPipe,
    SpecialtyTypePipe,
    ParentSpecialtyPipe,
    SpecStatusPipe,
    PHDatePipe,
    NoValuePipe,
    PhoneToDBPipe,
    ReverseBoolPipe,
    PhoneFromDBPipe,
    TermStatusPipe,
    VendorsearchComponent,
    VendorComponent,
    UwimportComponent
  ],
  imports: [
    DatatableModule,
    NgSelectizeModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    HttpClientModule,
    HttpModule,
    RouterModule.forRoot(routes, { enableTracing: true })
  ],
  providers: [
    ProviderHubService,
    { provide: HTTP_INTERCEPTORS, useClass: CustomInterceptor, multi:true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
