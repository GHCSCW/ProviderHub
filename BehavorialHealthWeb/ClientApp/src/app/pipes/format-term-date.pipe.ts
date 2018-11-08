import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

/*

*/
@Pipe({
  name: 'FormatTermDatePipe'
})
export class FormatTermDatePipe implements PipeTransform {
  transform(value: Date): string{
    var newDate = moment(value).format('L');
    return newDate == "12/31/2999" ? '' : newDate
  };
}
