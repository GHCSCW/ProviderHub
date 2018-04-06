import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule} from "@angular/common";
//3rd party
import 'hammerjs';

//import { SearchComponent } from './search/search.component';

//Modules
import { AppRoutingModule } from './app-routing.module';
import { ProviderModule } from "./provider/provider.module";
import { FacilityModule } from "./facility/facility.module";
import { VendorModule } from "./vendor/vendor.module";
import { SearchModule } from "./search/search.module";
import { AddressModule} from "./address/address.module";

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppMaterialModule } from './app-material/app-material.module';

//import { SharedModule } from './SharedModule';

//Services
import { NavbarService } from './service/navbarservice';
import { EnumService } from './service/enum-service';
import { MentalHealthService } from './service/mental.health.service';
import { InterfaceService} from './service/interface.service';
//Components
import { NotFoundComponent } from './not-found/not-found.component';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
//import { BooleanPipe } from './service/pipe-service';

@NgModule({
 
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    CommonModule,
    FormsModule,
    HttpModule,
    ReactiveFormsModule,
    ProviderModule,
    FacilityModule,
    VendorModule,
    SearchModule,
    AddressModule,
   // SharedModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    AppMaterialModule
    
  ],
  declarations: [
    AppComponent,
    NavMenuComponent,
    NotFoundComponent
  ],
  exports: [
    CommonModule,
    FormsModule,
    AppMaterialModule
  ],
  providers: [
    MentalHealthService,
    NavbarService,
    EnumService,
    InterfaceService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
