import { NgModule } from '@angular/core';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AppMaterialModule } from '../app-material/app-material.module';
import { MatDialogRef } from '@angular/material';
import { VendorComponent } from './vendor.component';
import { CreateVendorComponent } from './create-vendor.component';
import { VendorRoutingModule } from './vendor.routing.module';
import { CommonModule } from '@angular/common';
import { ProviderModule } from "../provider/provider.module";
import { DialogVendor, DialogVendorDialog } from './dialog-vendor.component';

@NgModule({
  imports: [
    VendorRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ProviderModule,
    AppMaterialModule
],
  declarations: [
    VendorComponent,
    CreateVendorComponent,
    DialogVendor,
    DialogVendorDialog 
  ],
  entryComponents: [
    DialogVendor,
    DialogVendorDialog
  ],
  exports: [
    VendorComponent,
    CreateVendorComponent, 
    DialogVendor,
    DialogVendorDialog
  ],
  providers: []
})

export class VendorModule {

}
