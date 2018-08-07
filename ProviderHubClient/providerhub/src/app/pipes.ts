import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'phdate'
})
export class PHDatePipe implements PipeTransform {
  transform(value: any): any {
    return new DatePipe('en-US').transform(value.replace(/\D/g, ''));
  }
}

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
  name: 'novalue'
})
export class NoValuePipe implements PipeTransform {
  transform(value: any): any {
    return (value == null) || (value=="");
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
    return (value.ParentSpecialtyID==0) ? "(is a Parent Specialty)" : "Parent Specialty: "+value.ParentName;
  }
}
