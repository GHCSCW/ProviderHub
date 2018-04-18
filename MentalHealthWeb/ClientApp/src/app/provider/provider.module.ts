import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material';

import { AppMaterialModule } from '../app-material/app-material.module';
import { ProviderComponent } from './provider.component';
import { ProviderRoutingModule } from './provider.routing.module';
import { DialogProviderDetails, DialogProviderDetailsDialog } from './dialog-provider-details'
import { DialogFacilityProviderRelationship, DialogFacilityProviderRelationshipDialog } from './dialog-facility-provider-relationship'
import { FacilityProviderRelationshipComponent } from './facility-provider-relationship.component'
import { MentalHealthService } from '../services/mental.health.service'
import { BooleanPipe } from '../pipes/BooleanPipe';
import { ActiveInactivePipe } from '../pipes/ActiveInactivePipe';
import { CreateProvider } from './create-provider.component';
import { DialogEditbhAttribute, DialogEditbhAttributeDialog } from './editbhAttributes/dialog-edit-bhAttributes';

@NgModule({
  imports: [
    ProviderRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppMaterialModule
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
    BooleanPipe,
    ActiveInactivePipe
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
    CreateProvider
  ],
  providers: []
})

export class ProviderModule {
}
