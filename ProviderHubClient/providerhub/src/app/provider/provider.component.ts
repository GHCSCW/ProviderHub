import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { API } from '../globals';

@Component({
  selector: 'app-provider',
  templateUrl: './provider.component.html',
  styleUrls: ['./provider.component.scss']
})
export class ProviderComponent implements OnInit {

  public apiRoot: string;
  public providerId: number;
  public Provider: any;

  constructor(private route: ActivatedRoute, private router: Router) {
    //TEST PROVIDER FOR DEBUG
    this.Provider = {
      FirstName: "John", LastName: "Doe", Credentials: "MD (Primary), PhD", NPI: 3913874644,
      Languages: "English, Spanish", ABMS: { has: "Yes", exp: "12/19/2024" },
      License: { no: 452567, exp: "12/19/2024" }, CertifyingBoard: { name: "USDA", exp: "12/19/2024" },
      FacilityNames: "UW, Meriter"
    };
  }

  ngOnInit() {
    this.route.data.subscribe(v => this.apiRoot = v.apiRoot);
    console.log(this.apiRoot);
    this.route.params.subscribe(params => { this.providerId = +params['id']; });
    console.log(this.providerId);
    //note: if navigated to from direct link, and not clicking a provider,
    //      it'll be empty til the provider object loads from AJAX...but that's okay
    document.getElementById("page-title").innerHTML = API.selectedProvider;//faster than jQ or Ang
  }

}
