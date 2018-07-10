import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProviderComponent } from './provider.component';
import { FacilityProviderRelationshipComponent } from './facility-provider-relationship.component/facility-provider-relationship.component';
import { CreateProvider } from './create-provider-component/create-provider.component';
import { MapProviderToFacility } from './map-provider-to-facility-component/map-provider-to-facility.component';

const providerRoutes: Routes = [
  { path: 'provider', component: ProviderComponent },
  { path: 'provider/facilityrel/:id', component: ProviderComponent },
  { path: 'provider/:provid', component: ProviderComponent },
  { path: 'createprovider', component: CreateProvider },
  { path: 'mapprovidertoclinic', component: MapProviderToFacility }
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
