import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-uwimport',
  templateUrl: './uwimport.component.html',
  styleUrls: ['./uwimport.component.scss']
})
export class UwimportComponent implements OnInit {

  public environment: any;

  constructor() { }

  ngOnInit() {
    this.environment = environment;
  }

  public submit() {
    var formData = new FormData();

    formData.append("username", "Groucho");
    //formData.append("accountnum", 123456); // number 123456 is immediately converted to a string "123456"

    // HTML file input, chosen by user
    formData.append("userfile", (document.getElementById("fileinput") as HTMLFormElement).files[0]);

    // JavaScript file-like object
    /*var content = '<a id="a"><b id="b">hey!</b></a>'; // the body of the new file...
    var blob = new Blob([content], { type: "text/xml" });

    formData.append("webmasterfile", blob);*/

    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
      if (xhr.readyState === 4) {
        console.log(xhr.response); alert("File Staged and Imported to DB Successfully");
        var o = JSON.parse(xhr.response);
        console.log(o.map_result.db_transaction);
      }
    }
    xhr.open("POST", "http://localhost:51660/Home/UWDataImport");
    xhr.withCredentials = true;
    xhr.send(formData);
  }

}
