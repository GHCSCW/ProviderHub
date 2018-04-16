import { Observable } from 'rxjs/Observable';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { LogEntry } from './log.service';

export abstract class LogPublisher {
  location: string;

  abstract log(record: LogEntry) : Observable<boolean>;
  abstract clear() : Observable<boolean>;
}

export class LogConsole extends LogPublisher {
  log(record: LogEntry) : Observable<boolean>{
    // Log to the console
    console.log(record.buildLogString());

    return Observable.of(true);
  }

  clear() : Observable<boolean> {
    console.clear();

    return Observable.of(true);
  }
}

export class LogWebApi extends LogPublisher {
  constructor(private http: Http) {
    super();

    this.location = "http://localhost:57931/api/log";
  }

  log(record: LogEntry): Observable<boolean> {
    let headers = new Headers({ 'Content-Type': 'application/json' });
    let options = new RequestOptions({ headers: headers });

    return this.http.post(this.location, record, options)
      .map(response => response.json)
      .catch(this.handleErrors);
  }

  clear(): Observable<boolean> {
    // TODO: Call Web API to clear all log entries
    return Observable.of(true);
  }

  private handleErrors(error: any): Observable<any> {
    let errors: string[] = [];
    let msg: string = "";

    msg = "Status: " + error.status;
    msg += " - Status Text: " + error.statusText;
    if(error.json()) {
      msg += " - Exception Message: " + error.json().exceptionMessage;
    }

    errors.push(msg);

    console.error("An error occurred", errors);

    return Observable.throw(errors);
  }
}
