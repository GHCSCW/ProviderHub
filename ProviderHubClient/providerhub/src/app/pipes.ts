import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'gender'
})
export class GenderPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return (value==2)? "Male" : "Female";
  }
}

@Pipe({
  name: 'nullable'
})
export class NullablePipe implements PipeTransform {
  transform(value: any, nullDisplayValue: string = "-"): any {
    return (value == null) ? nullDisplayValue : value;
  }
}

@Pipe({
  name: 'bool'
})
export class BoolPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return (value===null)? "Unknown" : (value) ? "Yes" : "No";
  }
}

@Pipe({
  name: 'specialtyType'
})
export class SpecialtyTypePipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return (value.SequenceNumber==1) ? "Primary Specialty" : "Secondary Specialty";
  }
}

@Pipe({
  name: 'parentSpecialty'
})
export class ParentSpecialtyPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return (value.ParentSpecialtyID==0) ? "(is a Parent Specialty)" : "Parent Specialty :"+value.ParentName;
  }
}
