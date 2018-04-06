import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VendorComponent } from './vendor.component';
import { CreateVendorComponent } from './create.vendor.component';

const VendorRoutes: Routes = [
  { path: 'vendor', component: VendorComponent },
  { path: 'vendor/:id', component: VendorComponent },
  { path: 'createvendor', component: CreateVendorComponent }
];

@NgModule({
  imports: [
    RouterModule.forChild(VendorRoutes)
  ],
  exports: [
    RouterModule
  ]
})
export class VendorRoutingModule { }
