import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material';

import { AppMaterialModule } from '../app-material/app-material.module';
import { ProviderComponent } from './provider.component';
import { ProviderRoutingModule } from './provider.routing.module';
import { DialogProviderDetails, DialogProviderDetailsDialog } from './edit-provider-details';
import { DialogFacilityProviderRelationship, DialogFacilityProviderRelationshipDialog } from './edit-facility-provider-relationship.component/edit-facility-provider-relationship.component';
import { FacilityProviderRelationshipComponent } from './facility-provider-relationship.component/facility-provider-relationship.component';
import { MentalHealthService } from '../services/mental-health.service';
import { CreateProvider } from './create-provider-component/create-provider.component';
import { MapProviderToFacility } from './map-provider-to-facility-component/map-provider-to-facility.component';
import { DialogEditbhAttribute, DialogEditbhAttributeDialog } from './edit-bh-attributes-component/dialog-edit-bhAttributes';
import { AngularMultiSelectModule } from 'angular2-multiselect-dropdown/angular2-multiselect-dropdown';
import { SharedModule } from '../shared.module';

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
    MapProviderToFacility
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
    MapProviderToFacility

  ],
  providers: []
})

export class ProviderModule {
}
