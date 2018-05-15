import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { environment } from '../../environments/environment';
@Injectable()
export class AthenticationServiceService {


  constructor( private http: HttpClient
  ) { }

  getUser(): Observable<string> {
    console.log('Calling getUser');
    let serviceUrl: string = `${environment.serviceBaseUrl}api/auth/getuser`;
    return this.http.get(serviceUrl)
      .map((rslt: string) => {
        return rslt;
      });
  }
  getUserRoles(): Observable<string[]> {
    console.log('Calling getUserRoles');
    let serviceUrl: string = `${environment.serviceBaseUrl}api/auth/getuserroles`;
    return this.http.get(serviceUrl)
      .map((rslt: string[]) => {
        return rslt;
      });
  }
}
