import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProviderComponent } from './provider.component';
import { FacilityProviderRelationshipComponent } from './facility-provider-relationship.component';
import { CreateProvider } from './create-provider.component';
import { MapProviderToClinic } from './mapProviderToFacility/map-provider-to-clinic.component';

const providerRoutes: Routes = [
  { path: 'provider', component: ProviderComponent },
  { path: 'provider/facilityrel/:id', component: ProviderComponent },
  { path: 'provider/:provid', component: ProviderComponent },
  { path: 'facilityrel', component: FacilityProviderRelationshipComponent },
  { path: 'createprovider', component: CreateProvider },
  { path: 'mapprovidertoclinic', component: MapProviderToClinic }
];

@NgModule({
  imports: [
    RouterModule.forChild(providerRoutes)
  ],
  exports: [
    RouterModule
  ]
})
export class ProviderRoutingModule { }
