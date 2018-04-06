import { Pipe, PipeTransform } from '@angular/core';
import { AddressType } from '../service/enum-service'

@Pipe({ name: 'Boolean' })
export class BooleanPipe implements PipeTransform {
  transform(value: boolean): string {
    return value == true ? 'Yes' : 'No'
  };
}

@Pipe({
  name: 'filter',
  pure: false
})
export class FilterPipe implements PipeTransform {
  transform(items: any[], term): any {
    console.log('term', term);

    return term
      ? items.filter(item => item.title.indexOf(term) !== -1)
      : items;
  }
}

@Pipe({
  name: 'providerSearch'
})
export class ProviderSearchPipe implements PipeTransform {
  transform(items: Array<any>, nameSearch: string, credentialSearch, CSP_Indicator: Boolean) {
    if (items && items.length) {
      return items.filter(item => {
        if (nameSearch && item.provider.fullName.toLowerCase().indexOf(nameSearch.toLowerCase()) === -1) {
          return false;
        }
        if (credentialSearch.viewValue && item.provider.credentials.toLowerCase().indexOf(credentialSearch.viewValue.toLowerCase()) === -1) {
          return false;
        }
        if (CSP_Indicator && item.provider.csP_Indicator === -1) {
          return false;
        }
        //if (companySearch && item.company.toLowerCase().indexOf(companySearch.toLowerCase()) === -1) {
        //  return false;
        //}
        return true;
      })
    }
    else {
      return items;
    }
  }
}

//@Pipe({
//  name: 'eNumAsString'
//})
//export class ENumAsStringPipe implements PipeTransform {
//  transform(value: number): string {
//    return AddressType[value]
//  };
//}
