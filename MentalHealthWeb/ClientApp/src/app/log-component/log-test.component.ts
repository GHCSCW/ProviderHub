import { Component } from "@angular/core";
import { LogService, LogLevel } from '../services/log.service';
import { AthenticationServiceService } from '../services/AthenticationService'
@Component({
  selector: "log-test",
  templateUrl: "./log-test.component.html"
})
export class LogTestComponent {

  constructor(
    private logger: LogService,
    private authSvc: AthenticationServiceService) {
  }

  testLog(): void {
    //this.logger.level = LogLevel.Off;

    this.logger.log("Test the log() Method", "Paul", "John", 2, 3);
  }
}
