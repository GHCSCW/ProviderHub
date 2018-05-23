import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import { environment } from '../../environments/environment';
import { EventEmitter } from '@angular/core';

@Injectable()
export class AuthenticationService {

  canEditUpdated: EventEmitter<boolean> = new EventEmitter();
  canEdit: any = false;
  userRoles: any = [];
  userRoles2: any = [];
  serviceData: any = [];
  serviceUrl: string = 'api/auth/';

  constructor
    (
    private httpClient: HttpClient
    ) {


  }

  getUserRoles(): Observable<any> {
    return this.httpClient.get(this.serviceUrl + '/getuserroles');
  }

  getUser(): Observable<string> {
    return this.httpClient.get(this.serviceUrl + '/getuser')
      .map((rslt: string) => {
        return rslt;
      });
  }

  userCanEdit() {
    this.canEdit = true;
    this.canEditUpdated.emit(this.canEdit);
  }

}
