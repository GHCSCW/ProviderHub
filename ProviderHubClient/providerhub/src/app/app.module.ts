import { BrowserModule } from '@angular/platform-browser';
import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { DatatableComponent } from './datatable/datatable.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { API } from './globals';
import { environment } from '../environments/environment';
import { HeaderComponent } from './header/header.component';

const routes: Routes = [
  { path: 'providers', component: DatatableComponent, data: { apiRoot: (environment.production) ? API.prod : API.dev } },
  { path: '', redirectTo: '/providers', pathMatch: 'full' }
];

@NgModule({
  declarations: [
    AppComponent,
    DatatableComponent,
    HeaderComponent
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
