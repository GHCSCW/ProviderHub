import { Injectable } from '@angular/core';
import { Http, Response, HttpModule, RequestOptions } from '@angular/http'; //Headers is deprecated
import { HttpClientModule, HttpClient, HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpHeaders } from '@angular/common/http';
import { map } from "rxjs/operators";
import { Observable } from "rxjs";

@Injectable()
export class ProviderHubService {
  constructor(private http: Http) { }
  hitAPI(url, body="") {
    try {
      //let headers = new Headers(); headers.append('Content-Type','application/json');
      var options = new RequestOptions({ withCredentials: true });
      return this.http.post(url,body,options).pipe(map(response => response.json()));
    } catch (err) { console.log(err); }
  }
}

@Injectable()
export class CustomInterceptor implements HttpInterceptor {

  constructor() {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    request = request.clone({
      withCredentials: true
    });

    return next.handle(request);
  }
}
