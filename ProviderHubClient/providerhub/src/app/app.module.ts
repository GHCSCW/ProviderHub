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

const routes: Routes = [
  /*Root: Provider List & Basic Search (Name/NPI)*/
  { path: 'providers', component: DatatableComponent, data: { apiRoot: (environment.production) ? API.prod : API.dev } },
  { path: '', redirectTo: '/providers', pathMatch: 'full' },
  /*Individual Provider View*/
  { path: 'provider/:id', component: ProviderComponent, data: { apiRoot: (environment.production) ? API.prod : API.dev } }
];

@NgModule({
  declarations: [
    AppComponent,
    DatatableComponent,
    HeaderComponent,
    ProviderComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    RouterModule.forRoot(routes, { enableTracing: true })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
