import { Component, Output, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from './globals';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'ProviderHub';

  constructor(private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    
  }

  public nav(route): void {
    this.router.navigate([route]);
  }
}
