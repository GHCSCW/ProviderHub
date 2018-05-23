import { Component, OnInit } from '@angular/core';
import { NavbarService } from '../services/navbar.service';
import { MentalHealthService } from '../services/mental-health.service';
import { AuthenticationService } from '../services/authentication.service';


@Component({
  selector: 'nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
  providers: []
})
export class NavMenuComponent implements OnInit {
  canEdit: boolean = false;
  router: any;
  facilityProviderRelationship: any = [];
  provider: any = [];
  facility: any = [];
  facilityAddress: any = [];
  authRslt: string = '';
  userRoles: any = [];

  constructor(
    public nav: NavbarService,
    private mentalHealthService: MentalHealthService,
    private authSvc: AuthenticationService

  ) {

  }

  ngOnInit() {
    this.getUsername();

    this.authSvc.getUserRoles().finally(() => this.canUserEdit()).subscribe(
      result => {
      this.userRoles = result,
        e => { console.log(e) }
      })

  }

  getUsername(): void {
    this.authSvc.getUser()
      .subscribe(
      r => { this.authRslt = r },
      e => { console.log(e) }
      );
  }

  canUserEdit() {
    this.userRoles.forEach(item => {
      if ((item.roleName == "SuperUser") || (item.roleName == "Editor")) {
        this.canEdit = true;
        this.authSvc.addCanEdit();
      }
  
    });
    this.canEdit = true;
    this.authSvc.addCanEdit();
  }
}




