import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { environment } from '../../environments/environment';
import { Http } from '@angular/http';
@Injectable()
export class AthenticationServiceService {

  canEdit: boolean = false;
  userRoles: any = [];
  userRoles2: any = [];
  serviceData: any = [];
  serviceUrl: string = 'api/auth/';

  constructor
    (
    private httpClient: HttpClient,
    public http: Http
    ) {


  }

  getUserRoles(): Observable<any> {
    return this.httpClient.get(this.serviceUrl + '/getuserroles');
  }

  getUser(): Observable<string> {
    return this.httpClient.get(this.serviceUrl+'/getuser')
      .map((rslt: string) => {
        return rslt;
      });
  }

  userCanEdit() {
    return this.addCanEdit;
  }

  addCanEdit() {
    this.canEdit = true;
  }
}
