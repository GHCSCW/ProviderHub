import { Injectable } from '@angular/core';

export enum Gender {
    Female = 1,
    Male = 2,
    Unknown = 3
}

export enum AddressType {
    'Clinical Practice Service Location' = 1,
    'Main Address' = 2,
    'Mail Address' = 3,
    ' Business Administration' = 4
}

export enum Language {
    English = 1,
    Spanish = 2,
    Hmung = 3
}

export enum BHAttributeType {
    Ages = 1,
    Models = 2,
    Conditions = 3,
    TherapeuticApproaches = 4,
    Other = 5
}



@Injectable()

export class EnumService {

    constructor() { }


}

