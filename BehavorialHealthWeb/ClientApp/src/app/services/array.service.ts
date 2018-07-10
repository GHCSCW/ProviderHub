import { Injectable } from '@angular/core';
import { addressTypes } from '../models/address-types';
import { Language } from '../models/language';
import { MentalHealthService } from './mental-health.service';
import { Observable } from 'rxjs/Observable';

@Injectable()

export class ArrayService {
  private addressTypes: addressTypes[] = [
      {
        value: 1,
        label: 'Clinical Practice Service Location'
      },
      {
        value: 2,
        label: 'Main Address'
      },
      {
        value: 3,
        label: 'Mail Address'
      },
      {
        value: 3,
        label: 'Business Administration'
      },
    ]

  constructor(private mentalhealthservice: MentalHealthService) { }

  getLanguages(): Observable<Language[]> {
    return this.mentalhealthservice.getLanguages().map(res => {
      return res
    });
  }

  getAddressTypes(): addressTypes[] {
    return this.addressTypes;
  }
}

