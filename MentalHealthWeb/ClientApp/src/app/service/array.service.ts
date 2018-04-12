import { Injectable } from '@angular/core';
import { addressTypes } from '../models/addressTypes';
import { Language } from '../models/language';
import { MentalHealthService } from './mental.health.service';
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

 //private  languages: language[] = [
 //   {
 //     value: 1,
 //     label: 'English'
 //   },
 //   {
 //     value: 2,
 //     label: 'Spanish'
 //   },
 //   {
 //     value: 3,
 //     label: 'Hmung'
 //   }
 //]

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

