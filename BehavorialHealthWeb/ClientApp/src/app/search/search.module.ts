import { NgModule } from '@angular/core';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { SearchComponent } from './search.component';
import { AppModule } from "../app.module";
import { SearchRoutingModule } from './search.routing.module';
import { CommonModule } from '@angular/common'
import { ProviderModule } from '../provider/provider.module';
import { AdvancedSearchComponent } from './advanced.search.component';
import { AppMaterialModule } from '../app-material/app-material.module';
import { SharedModule } from '../shared.module';

@NgModule({
  imports: [
    SearchRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ProviderModule,
    AppMaterialModule,
    SharedModule
    ],
  declarations: [
    SearchComponent,
    AdvancedSearchComponent
  ],
  exports: [
    SearchComponent,
    AdvancedSearchComponent
  ],
  providers: []
})

export class SearchModule {


}
