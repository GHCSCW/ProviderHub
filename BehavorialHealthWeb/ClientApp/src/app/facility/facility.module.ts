import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppMaterialModule } from '../app-material/app-material.module';

import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { FacilityComponent } from './facility.component';
import { CreateFacility } from './create-facility.component';
import { FacilityRoutingModule } from './facility.routing.module';

import { DialogFacilityDetailsDialog, DialogFacilityDetails } from './edit-facility-details'
import { ProviderModule } from "../provider/provider.module";
import { AddressModule } from "../address/address.module";
import { AddressComponent } from '../address/address.component';
import { MentalHealthService } from '../services/mental-health.service'
import { SharedModule } from '../shared.module';

@NgModule({
  imports: [
    FacilityRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppMaterialModule,
    ProviderModule,
    AddressModule,
    SharedModule
    
  ],
  declarations: [
    FacilityComponent,
    DialogFacilityDetailsDialog,
    DialogFacilityDetails,
    CreateFacility
  ],
  entryComponents: [
    DialogFacilityDetails,
    DialogFacilityDetailsDialog
  ],
  exports: [
    DialogFacilityDetails,
    DialogFacilityDetailsDialog,
    CreateFacility,
    AddressComponent
  ],
  providers: []
})

export class FacilityModule {

}
