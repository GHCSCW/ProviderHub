import { Pipe, PipeTransform } from '@angular/core';
import { AddressType } from '../service/enum-service'
/*

*/
@Pipe({
  name: 'eNumAsString'
})
export class ENumAsStringPipe implements PipeTransform {
  transform(value: number): string {
    return AddressType[value]
  };
}
