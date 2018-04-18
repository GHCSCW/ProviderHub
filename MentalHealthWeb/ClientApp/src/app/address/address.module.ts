import { NgModule } from '@angular/core';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AppMaterialModule } from '../app-material/app-material.module';
import { AddressComponent } from './address.component';
import { AddressRoutingModule } from './address.routing.module';
import { MentalHealthService } from '../services/mental.health.service'


@NgModule({
  imports: [
    AddressRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppMaterialModule
    
  ],
  declarations: [
    AddressComponent
  ],

  exports: [
    AddressComponent
  ],
  providers: []
})

export class AddressModule {

}
