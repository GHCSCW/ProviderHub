import { Pipe, PipeTransform } from '@angular/core';
import { AddressType } from '../services/enum-service'

@Pipe({ name: 'Boolean' })
export class BooleanPipe implements PipeTransform {
  transform(value: boolean): string {
    return value == true ? 'Yes' : 'No'
  };
}

