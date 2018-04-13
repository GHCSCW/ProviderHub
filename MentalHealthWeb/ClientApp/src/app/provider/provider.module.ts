import { NgModule } from '@angular/core';
import { MentalHealthService } from '../service/mental.health.service'
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ProviderComponent } from './provider.component';
import { AppModule } from "../app.module";
import { ProviderRoutingModule } from './provider.routing.module';
import { CommonModule } from '@angular/common';
import { DialogProviderDetails, DialogProviderDetailsDialog } from './dialog-provider-details'
import { DialogFacilityProviderRelationship, DialogFacilityProviderRelationshipDialog } from './dialog-facility-provider-relationship'
import { AppMaterialModule } from '../app-material/app-material.module';
import { MatDialogRef } from '@angular/material';
import { FacilityProviderRelationshipComponent } from './facility-provider-relationship.component'
import { BooleanPipe } from '../service/pipe-service';
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
    BooleanPipe
  ],
  entryComponents: [
    DialogProviderDetails,
    DialogProviderDetailsDialog,
    DialogFacilityProviderRelationship,
    DialogFacilityProviderRelationshipDialog,
    DialogEditbhAttribute,
    DialogEditbhAttributeDialog,],
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
