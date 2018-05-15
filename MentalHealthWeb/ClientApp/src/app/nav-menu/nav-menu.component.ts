import { Component, OnInit } from '@angular/core';
import { NavbarService } from '../services/navbarservice';
import { MentalHealthService } from '../services/mental.health.service';
import { AthenticationServiceService } from '../services/AthenticationService';


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
  userRoles: any = [];

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
    //this.authSvc.getUserRoles()
    //  .subscribe(
    //  r => { this.userRoles = r },
    //  e => { console.log(e) }
    //  );
  }
}

