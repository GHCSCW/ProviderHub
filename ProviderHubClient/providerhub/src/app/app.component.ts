import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'ProviderHub';

  ngOnInit() {
    document.getElementById("page-title").innerHTML = this.title;//faster than jQ or Ang
  }
}
