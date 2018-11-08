import { Address } from './address';

export class Facility {
  ID: number;
  FacilityName: string;
  NPI: string;
  FacilityAddress: Address;
  ExternalID: string;
  InternalNotes: string;
  CreatedDate: Date;
  CreatedBy: string;
  LastUpdatedDate: Date;
  LastUpdatedBy: string;
}
