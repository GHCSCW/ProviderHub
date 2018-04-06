import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddressComponent } from './address.component';

const addressRoutes: Routes = [
  { path: 'address', component: AddressComponent }
];

@NgModule({
  imports: [
    RouterModule.forChild(addressRoutes)
  ],
  exports: [
    RouterModule
  ]
})
export class AddressRoutingModule { }
