import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from "@angular/common";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

//3rd party
import 'hammerjs';
import { ToastrModule } from 'ngx-toastr';
import { AngularMultiSelectModule } from 'angular2-multiselect-dropdown/angular2-multiselect-dropdown';
import { GoogleAnalyticsEventsService } from "./services/google-analytics-events.service";

//Modules
import { AppRoutingModule } from './app-routing.module';
import { ProviderModule } from "./provider/provider.module";
import { FacilityModule } from "./facility/facility.module";
import { VendorModule } from "./vendor/vendor.module";
import { SearchModule } from "./search/search.module";
import { AddressModule} from "./address/address.module";
import { SharedModule } from './shared.module';

import { AppMaterialModule } from './app-material/app-material.module';

//Services
import { NavbarService } from './services/navbar.service';
import { EnumService } from './services/enum.service';
import { MentalHealthService } from './services/mental-health.service';
import { InterfaceService } from './services/interface.service';
import { ArrayService } from './services/array.service';
import { LogService} from './services/log.service'
import { LogPublishersService } from './services/log-publisher.service';
import { AuthenticationService } from './services/authentication.service';

//Components
import { NotFoundComponent } from './not-found/not-found.component';
import { LogTestComponent} from './log-component/log-test.component';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

//Interceptors
import { WinAuthInterceptor}  from './interceptors/winauth-interceptor';



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
    AppRoutingModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(), // ToastrModule added
    AppMaterialModule,
    AngularMultiSelectModule,
    SharedModule
    
  ],
  declarations: [
    AppComponent,
    NavMenuComponent,
    NotFoundComponent,
    LogTestComponent

  ],
  exports: [
    CommonModule,
    FormsModule,
    AppMaterialModule,
    AngularMultiSelectModule,
    SharedModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: WinAuthInterceptor,
      multi: true
    },
    MentalHealthService,
    NavbarService,
    EnumService,
    InterfaceService,
    ArrayService,
    LogPublishersService,
    LogService,
    AuthenticationService,
    ToastrModule,
    GoogleAnalyticsEventsService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
