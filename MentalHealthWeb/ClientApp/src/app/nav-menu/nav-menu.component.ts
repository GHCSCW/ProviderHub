import { Component, OnInit } from '@angular/core';
import { NavbarService } from '../service/navbarservice';
import { MentalHealthService } from '../service/mental.health.service';
import { AthenticationServiceService } from '../service/AthenticationService';


@Component({
  selector: 'nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
  providers: []
})
export class NavMenuComponent implements OnInit {
  router: any;
  facilityProviderRelationship: any = [];
  provider: any = [];
  facility: any = [];
  facilityAddress: any = [];
  authRslt: string = '';

  constructor(public nav: NavbarService, private mentalHealthService: MentalHealthService, private authSvc: AthenticationServiceService) {

  }

  ngOnInit() {
    this.testAuthentication();
  }

  testAuthentication(): void {
    this.authSvc.getUser()
      .subscribe(
      r => { this.authRslt = r },
      e => { console.log(e) }
      );
  }
}

