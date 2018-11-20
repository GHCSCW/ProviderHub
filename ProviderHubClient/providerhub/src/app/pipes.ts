import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'phdate'
})
export class PHDatePipe implements PipeTransform {
  transform(value: any): any {
    //console.log("PHDatePipe input: " + value);
    if (value.charAt(value.length - 7) == '-') {
      //console.log("PHDateOutput: " + new DatePipe('en-US').transform(value.replace(/\D/g, '').slice(0, -4)));
      return new DatePipe('en-US').transform(value.replace(/\D/g, '').slice(0, -4));
    }
    //console.log("PHDateOutput: " + new DatePipe('en-US').transform(value.replace(/\D/g, '')));
    return new DatePipe('en-US').transform(value.replace(/\D/g, ''));
  }
}

@Pipe({
  name: 'gender'
})
export class GenderPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return (value==2)? "Male" : (value==1)? "Female" : "Unknown";
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
  name: 'reversebool'
})
export class ReverseBoolPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return (value == "Yes") ? true : (value == "No") ? false : null;
  }
}

@Pipe({
  name: 'specialtyType'
})
export class SpecialtyTypePipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return (value.SequenceNumber==1) ? "Primary Specialty" : (value.SequenceNumber==2)? "Secondary Specialty" : "Specialty "+value.SequenceNumber;
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

@Pipe({
  name: 'specStatus'
})
export class SpecStatusPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    var todaysDate = new Date(); todaysDate.setHours(0, 0, 0, 0);
    return (value.TerminationDate !== null && parseInt(value.TerminationDate) <= todaysDate.getTime()) ? "INACTIVE" : "ACTIVE";
  }
}

@Pipe({
  name: 'directoryStatus'
})
export class TermStatusPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    var todaysDate = new Date(); todaysDate.setHours(0, 0, 0, 0);
    return (value !== null && parseInt(value) <= todaysDate.getTime()) ? "INACTIVE" : "ACTIVE";
  }
}

@Pipe({
  name: 'phonetodb'
})
export class PhoneToDBPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    if (typeof value=="undefined" || value===null || value.trim() == "") { return ""; }
    value = value.replace(/\D/g, '');//strip any non digit character (including +, -, ())
    if (value.substring(0, 1) == "1" && value.length > 10) { value = value.substring(1, value.length); }//leading 1
    return (value.length==10)? value : "NaN";//ANOTHER OPTION: client can check for desired # of digits aka 10 and we can return value
  }
}

@Pipe({
  name: 'phonefromdb'
})
export class PhoneFromDBPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    if (typeof value == "undefined" || value === null || value.trim() == "") { return ""; }
    if (value != value.replace(/\D/g, '')) { return value; }
    return value.substring(0, 3) + "-" + value.substring(3, 6) + "-" + value.substring(6, 10);
  }
}

