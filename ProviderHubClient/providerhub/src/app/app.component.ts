import { Component, Output, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router, NavigationStart, NavigationEnd, NavigationError, NavigationCancel, RoutesRecognized } from '@angular/router';
import { API } from './globals';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'ProviderHub';
  selectedTab = "P";

  constructor(private route: ActivatedRoute, private router: Router) {
    router.events.forEach((event) => {
      if (event instanceof NavigationEnd) {
        console.log(this.router.url.split('/')[1]);
        switch (this.router.url.split('/')[1]) {
          case "provider": //from search before redirect
          case "Provider": this.selectedTab = "P"; break;
          case "facility": //from search before redirect
          case "Facility": this.selectedTab = "F"; break;
          default: this.selectedTab = "N"; console.log("a url in this app without a '/'? or something wrong"); break;
        }
      }
      // Other Events you might want to handle:NavigationStart,NavigationEnd,NavigationCancel,NavigationError,RoutesRecognized
    });
  }

  ngOnInit() {
    
  }

  public nav(route,tab): void {
    this.router.navigate([route]);
    this.selectedTab = tab; 
  }
}
