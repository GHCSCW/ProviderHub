import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FacilityComponent } from './facility.component';
import { CreateFacility } from './create-facility';

const facilityRoutes: Routes = [
  { path: 'facility', component: FacilityComponent },
  { path: 'facility/:id', component: FacilityComponent },
  { path: 'createfacility', component: CreateFacility}

];

@NgModule({
  imports: [
    RouterModule.forChild(facilityRoutes)
  ],
  exports: [
    RouterModule
  ]
})
export class FacilityRoutingModule { }
