import { Pipe, PipeTransform } from '@angular/core';

/*

*/
@Pipe({
  name: 'ActiveInactivePipe'
})
export class ActiveInactivePipe implements PipeTransform {
  transform(value: boolean): string {
    return value == true ? 'Active' : 'Inactive'
  };
}
