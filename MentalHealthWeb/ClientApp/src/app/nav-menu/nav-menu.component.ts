import { Component, OnInit } from '@angular/core';
import { NavbarService } from '../service/navbarservice';
import { MentalHealthService } from '../service/mental.health.service';


@Component({
  selector: 'nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
  providers: []
})
export class NavMenuComponent implements OnInit {
  router: any;
  //isExpanded: boolean;

  facilityProviderRelationship: any = [];
  provider: any = [];
  facility: any = [];
  facilityAddress: any = [];

  constructor(public nav: NavbarService, private mentalHealthService: MentalHealthService) {

  }

  ngOnInit() {
    //this.mentalHealthService


  }
}

