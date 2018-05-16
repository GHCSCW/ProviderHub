import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material';

import { AppMaterialModule } from '../app-material/app-material.module';
import { ProviderComponent } from './provider.component';
import { ProviderRoutingModule } from './provider.routing.module';
import { DialogProviderDetails, DialogProviderDetailsDialog } from './dialog-provider-details';
import { DialogFacilityProviderRelationship, DialogFacilityProviderRelationshipDialog } from './dialog-facility-provider-relationship';
import { FacilityProviderRelationshipComponent } from './facility-provider-relationship.component';
import { MentalHealthService } from '../services/mental.health.service';
import { CreateProvider } from './create-provider.component';
import { MapProviderToClinic } from './mapProviderToFacility/map-provider-to-clinic.component';
import { DialogEditbhAttribute, DialogEditbhAttributeDialog } from './editbhAttributes/dialog-edit-bhAttributes';
import { AngularMultiSelectModule } from 'angular2-multiselect-dropdown/angular2-multiselect-dropdown';
import { SharedModule } from '../SharedModule';

@NgModule({
  imports: [
    ProviderRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppMaterialModule,
    AngularMultiSelectModule,
    SharedModule
  ],
  declarations: [
    ProviderComponent,
    FacilityProviderRelationshipComponent,
    DialogProviderDetails,
    DialogProviderDetailsDialog,
    DialogFacilityProviderRelationship,
    DialogFacilityProviderRelationshipDialog,
    DialogEditbhAttribute,
    DialogEditbhAttributeDialog,
    CreateProvider,
    MapProviderToClinic
  ],
  entryComponents: [
    DialogProviderDetails,
    DialogProviderDetailsDialog,
    DialogFacilityProviderRelationship,
    DialogFacilityProviderRelationshipDialog,
    DialogEditbhAttribute,
    DialogEditbhAttributeDialog
  ],
  exports: [
    ProviderComponent,
    FacilityProviderRelationshipComponent,
    DialogProviderDetails,
    DialogProviderDetailsDialog,
    DialogFacilityProviderRelationship,
    DialogFacilityProviderRelationshipDialog,
    DialogEditbhAttribute,
    DialogEditbhAttributeDialog,
    CreateProvider,
    MapProviderToClinic

  ],
  providers: []
})

export class ProviderModule {
}
