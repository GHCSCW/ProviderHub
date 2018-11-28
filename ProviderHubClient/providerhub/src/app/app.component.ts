import { Component, Output, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router, NavigationStart, NavigationEnd, NavigationError, NavigationCancel, RoutesRecognized } from '@angular/router';
import { API } from './globals';
import { environment } from '../environments/environment';
import { ProviderHubService } from './app.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'ProviderHub';
  selectedTab = "P";
  compactDesign = false;
  hasBeenAdjusted = false;
  public Service: any;
  public apiRoot: string;
  public username: string;
  public _interval: any;

  constructor(private route: ActivatedRoute, private router: Router, private service: ProviderHubService) {
    this.Service = service;
    router.events.forEach((event) => {
      if (event instanceof NavigationEnd) {
        console.log(this.router.url.split('/')[1]);
        switch (this.router.url.split('/')[1]) {
          case "provider": //from search before redirect
          case "Provider": this.selectedTab = "P"; break;
          case "facility": //from search before redirect
          case "Facility": this.selectedTab = "F"; break;
          case "vendor": //from search before redirect
          case "Vendor": this.selectedTab = "V"; break;
          default: this.selectedTab = "N"; console.log("a url in this app without a '/'? or something wrong"); break;
        }
        this.compactDesign = (this.router.url.indexOf("Search") == -1 && this.router.url.indexOf("uwimport") == -1);
        if (this.compactDesign) {
          window.addEventListener('resize', function (event) {
            var desiredSidebarHeight = Math.max(document.getElementsByClassName('main-display')[0].clientHeight, (window.innerHeight - 50)); var defaultSidebarHeight = 410; var correctionalFactor = 10;
            var paddingBottomToAdd = desiredSidebarHeight - defaultSidebarHeight + correctionalFactor;
            if (paddingBottomToAdd > 0) { (document.getElementsByClassName('left-sidebar')[0] as HTMLElement).style.paddingBottom = paddingBottomToAdd + "px"; }
          });
          var _interval = setInterval(function () {
            if (typeof (Event) === 'function') { window.dispatchEvent(new Event('resize')); }
            else { var evt = window.document.createEvent('UIEvents'); evt.initUIEvent('resize', true, false, window, 0); window.dispatchEvent(evt); }
          },100);
        }
      }
      // Other Events you might want to handle:NavigationStart,NavigationEnd,NavigationCancel,NavigationError,RoutesRecognized
    });
  }

  ngOnInit() {
    this.apiRoot = environment.apiRoot; var _dis = this;
    this.service.hitAPI(this.apiRoot).subscribe(
      data => {
        console.log(data); //{"result":"GHC-HMO\spillai","username":"spillai","isSuperUser":"True","isEditor":"False","isUser":"False"}
        this.username = data.username;
        environment.authUser = data;
        //environment.authUser.isSuperUser = "False";
        //GITHUB ISSUE XX - LOCKDOWN APPLICATION FOR ANONYMOUS USER if no result or no auth/user detected, lock down application here
      }
    );
  }

  public nav(route,tab): void {
    this.router.navigate([route]);
    this.selectedTab = tab; 
  }
}
