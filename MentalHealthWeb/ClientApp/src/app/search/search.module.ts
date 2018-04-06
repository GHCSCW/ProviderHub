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
import { FilterPipe, ProviderSearchPipe } from '../service/pipe-service';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';

@NgModule({
  imports: [
    SearchRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ProviderModule,
    AppMaterialModule,
    NgxMatSelectSearchModule
    ],
  declarations: [
    SearchComponent,
    AdvancedSearchComponent,
    FilterPipe,
    ProviderSearchPipe
  ],
  exports: [
    SearchComponent,
    AdvancedSearchComponent
  ],
  providers: []
})

export class SearchModule {


}
