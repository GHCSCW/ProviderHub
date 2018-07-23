import { BrowserModule } from '@angular/platform-browser';
import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { DatatableComponent } from './datatable/datatable.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { API } from './globals';
import { environment } from '../environments/environment';
import { HeaderComponent } from './header/header.component';
import { ProviderComponent } from './provider/provider.component';

import { ProviderHubService } from './app.service';
import { CommonModule } from '@angular/common';
import { Http, Response, HttpModule } from '@angular/http';
import { HttpClientModule, HttpInterceptor, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CustomInterceptor } from './app.service';
import { FacilitysearchComponent } from './facilitysearch/facilitysearch.component';
import { FacilityComponent } from './facility/facility.component';

const routes: Routes = [
  /*Root: Provider List & Basic Search (Name/NPI)*/
  { path: 'providers', component: DatatableComponent, data: { apiRoot: (environment.production) ? API.prod : API.dev } },
  { path: '', redirectTo: '/providers', pathMatch: 'full' },
  /*Individual Provider View*/
  { path: 'provider/:id', component: ProviderComponent, data: { apiRoot: (environment.production) ? API.prod : API.dev } },
  /*Facilities Search*/
  { path: 'facilities', component: FacilitysearchComponent, data: { apiRoot: (environment.production) ? API.prod : API.dev } },
  /*Individual Facility View*/
  { path: 'facility/:id', component: FacilityComponent, data: { apiRoot: (environment.production) ? API.prod : API.dev } }
];

@NgModule({
  declarations: [
    AppComponent,
    DatatableComponent,
    HeaderComponent,
    ProviderComponent,
    FacilitysearchComponent,
    FacilityComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
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
