import { Injectable } from '@angular/core';


export interface IAddress {

  ID: number;
  AddressType: number;  //     AddressType AddressType ENUM_TYPE
  AddressLine1: string;
  AddressLine2: string;
  //City: string;
  //State: string;
  //ZipCode: string;
  //County: string;
  //Region: string;
  //PhoneNumber: string;
  //PhoneExtension: string;
  //AlternatePhoneNumber: string;
  //FaxNumber: string;
  //Email: string;
  //Website: string;
  //ContactFirstName: string;
  //ContactLastName: string;
  //CreatedDate: Date;
  //CreatedBy: string;
  //LastUpdatedDate: Date;
  //LastUpdatedBy: string;

}

export interface IProvider {
  ID: number;
  EpicProviderID: string;
  NPI: string;
  FirstName: string;
  LastName: string;
  ExternalProviderName: string;
  DelegatName: string;
  HospitalAffiliation: string;
  DateOfBirth: Date;
  // Gender: enum;
  DelegatedIndicator: Boolean;
  CSP_Indicator: Boolean;
  MedicaidIndicator: Boolean;
  MedicareIndicator: Boolean;
  MedicarePTAN: string;
  MedicareEffectiveDate: Date;
  MedicareTerminationDate: Date;
  TaxonomyCode: string;
  InternalNotes: string;
  UniquePhysicanId: string;
  //CreatedDate: Date;
  CreatedBy: string;
  LastUpdatedDate: Date;
  LastUpdatedBy: string
  //languages ENUM
  //ClinitianTitle ENUM
  //ProviderSpecialties
}

@Injectable()

export class InterfaceService {
  /**
   * Returns a list of all of the current user's todos.
   */

 createAddress(
  ID?: number,
  AddressType: number = 1,
  AddressLine1?: string,
  AddressLine2?: string
  //City?: string,
  //State?: string,
  //ZipCode?: string,
  //County?: string,
  //Region?: string,
  //PhoneNumber?: string,
  //PhoneExtension?: string,
  //AlternatePhoneNumber?: string,
  //FaxNumber?: string,
  //Email?: string,
  //Website?: string,
  //ContactFirstName?: string,
  //ContactLastName?: string,
  ////CreatedDate?: Date,
  //CreatedBy?: string,
  //LastUpdatedDate?: Date,
  //LastUpdatedBy?: string
): IAddress {
  return {
    ID,
    AddressType,
    AddressLine1,
    AddressLine2
    // City: string,
    // State: string,
    // ZipCode: string,
    // County: string,
    // Region: string,
    // PhoneNumber: string,
    // PhoneExtension: string,
    // AlternatePhoneNumber: string,
    // FaxNumber: string,
    // Email: string,
    // Website: string,
    // ContactFirstName: string,
    // ContactLastName: string,
    //// CreatedDate: Date,
    // CreatedBy: string,
    // LastUpdatedDate: Date,
    // LastUpdatedBy: string
  }
}

}

