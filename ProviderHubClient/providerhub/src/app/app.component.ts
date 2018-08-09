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
  compactDesign = false;

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
        this.compactDesign = (this.router.url.indexOf("Search") == -1);
        if (this.compactDesign) {
          window.addEventListener('resize', function (event) {
            var desiredSidebarHeight = Math.max(document.getElementsByClassName('main-display')[0].clientHeight, (window.innerHeight - 50));
            var currentSidebarHeight = document.getElementsByClassName('left-sidebar')[0].clientHeight;
            var paddingBottomToAdd = desiredSidebarHeight - currentSidebarHeight;
            if (paddingBottomToAdd != 0) { (document.getElementsByClassName('left-sidebar')[0] as HTMLElement).style.paddingBottom = paddingBottomToAdd + "px"; }
          });
          if (typeof (Event) === 'function') {
            //what it should be
            window.dispatchEvent(new Event('resize'));
          } else {
            //deprecated but IE11 is dumb; actually warns that it's deprecated in IE11 but it's the only working method :)
            var evt = window.document.createEvent('UIEvents');
            evt.initUIEvent('resize', true, false, window, 0);
            window.dispatchEvent(evt);
          }
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
