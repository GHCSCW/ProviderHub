import {Injectable } from '@angular/core';
import { LogPublisher, LogConsole,LogWebApi } from './log-publishers';
import { Http} from '@angular/http';

@Injectable()
export class LogPublishersService {
  constructor(private http:Http) {
    this.buildPublishers();
  }

  publishers: LogPublisher[] = [];

  buildPublishers(): void {
    // Create an instance of the LogConsole class
    this.publishers.push(new LogConsole());

    this.publishers.push(new LogWebApi(this.http));
}
}