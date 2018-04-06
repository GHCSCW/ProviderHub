using System;
using System.Collections.Generic;
using System.Data;

namespace ProviderHubService
{
    public static class DataUtilityHelper
    {
        #region STATIC FUNCTION: CreateFacilityTable()

        private static DataTable CreateFacilityTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("FACILITY_ID", typeof(int));
            dt.Columns.Add("FACILITY_NAME", typeof(string));
            dt.Columns.Add("FACILITY_NPI", typeof(string));
            dt.Columns.Add("EXTERNAL_ID", typeof(string));
            dt.Columns.Add("INTERNAL_NOTES", typeof(string));
            dt.Columns.Add("LAST_UPDATED_BY", typeof(string));

            return dt;

        }
        #endregion

        #region STATIC FUNCTION: PopulateFacilityTable(Facility facility)

        public static DataTable PopulateFacilityTable(Facility facility)
        {
            DataTable dt = CreateFacilityTable();

            DataRow row = dt.NewRow();

            //Specify FacilityID to update record
            if (facility.ID > 0)
            {              
                row["FACILITY_ID"] = facility.ID;
            }

            row["FACILITY_NAME"] = facility.FacilityName;
            row["FACILITY_NPI"] = facility.NPI;
            row["EXTERNAL_ID"] = facility.ExternalID;
            row["INTERNAL_NOTES"] = facility.InternalNotes;
            row["LAST_UPDATED_BY"] = facility.LastUpdatedBy;

            dt.Rows.Add(row);

            return dt;

        }
        #endregion

        #region STATIC FUNCTION: CreateProviderDetailTable()

        private static DataTable CreateProviderDetailTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("PROVIDER_ID", typeof(int));
            dt.Columns.Add("EPIC_PROVIDER_ID", typeof(string));
            dt.Columns.Add("NATIONAL_PROVIDER_IDENTIFIER", typeof(string));
            dt.Columns.Add("PROVIDER_FIRST_NAME", typeof(string));
            dt.Columns.Add("PROVIDER_MIDDLE_NAME", typeof(string));
            dt.Columns.Add("PROVIDER_LAST_NAME", typeof(string));
            dt.Columns.Add("EXTERNAL_PROVIDER_ID", typeof(string));  
            dt.Columns.Add("EXTERNAL_PROVIDER_NAME", typeof(string));
            dt.Columns.Add("PROVIDER_DATE_OF_BIRTH", typeof(DateTime));
            dt.Columns.Add("PROVIDER_GENDER_ID", typeof(int));
            dt.Columns.Add("CSP_INDICATOR", typeof(bool));
            dt.Columns.Add("MEDICARE_PROVIDER_INDICATOR", typeof(bool));
            dt.Columns.Add("MEDICARE_PTAN", typeof(string));
            dt.Columns.Add("MEDICARE_EFFECTIVE_DATE", typeof(DateTime));
            dt.Columns.Add("MEDICARE_TERMINATION_DATE", typeof(DateTime));
            dt.Columns.Add("MEDICAID_PROVIDER_INDICATOR", typeof(bool));
            dt.Columns.Add("MEDICAID_PROVIDER_ID", typeof(string));
            dt.Columns.Add("EFFECTIVE_DATE", typeof(DateTime));
            dt.Columns.Add("TERMINATION_DATE", typeof(DateTime));
            dt.Columns.Add("INTERNAL_NOTES", typeof(string));
            dt.Columns.Add("LAST_UPDATED_BY", typeof(string));
                        
            return dt;
        }
        #endregion

        #region STATIC FUNCTION: PopulateProviderDetailsTable(Provider provider)

        public static DataTable PopulateProviderDetailsTable(Provider provider)
        {
            DataTable dt = CreateProviderDetailTable();
            try
            {
                DataRow row = dt.NewRow();

                //Specify ProviderID to update record
                if (provider.ID > 0)
                {
                    row["PROVIDER_ID"] = provider.ID;
                }

                row["EPIC_PROVIDER_ID"] = provider.EpicProviderID;
                row["NATIONAL_PROVIDER_IDENTIFIER"] = provider.NPI;
                row["PROVIDER_FIRST_NAME"] = provider.FirstName;
                row["PROVIDER_MIDDLE_NAME"] = provider.MiddleName;
                row["PROVIDER_LAST_NAME"] = provider.LastName;
                row["EXTERNAL_PROVIDER_ID"] = provider.ExternalProviderID;
                row["EXTERNAL_PROVIDER_NAME"] = provider.ExternalProviderName;

                if (provider.DateOfBirth.ToString().Length == 0)
                {
                    row["PROVIDER_DATE_OF_BIRTH"] = DBNull.Value;
                }
                else
                {
                    row["PROVIDER_DATE_OF_BIRTH"] = provider.DateOfBirth;
                }

                row["PROVIDER_GENDER_ID"] = Convert.ToInt32(provider.Gender);
                row["CSP_INDICATOR"] = provider.CSP_Indicator;
                row["MEDICARE_PROVIDER_INDICATOR"] = provider.MedicareIndicator;
                row["MEDICARE_PTAN"] = provider.MedicarePTAN;

                //MEDICARE DATES
                if (provider.MedicareEffectiveDate.ToString().Length == 0)
                {
                    row["MEDICARE_EFFECTIVE_DATE"] = DBNull.Value;
                }
                else
                {
                    row["MEDICARE_EFFECTIVE_DATE"] = provider.MedicareEffectiveDate;
                }

                if (provider.MedicareTerminationDate.ToString().Length == 0)
                {
                    row["MEDICARE_TERMINATION_DATE"] = DBNull.Value;
                }
                else
                {
                    row["MEDICARE_TERMINATION_DATE"] = provider.MedicareTerminationDate;
                }

                //MEDICAID/BADGERCARE DATA
                row["MEDICAID_PROVIDER_INDICATOR"] = provider.MedicaidIndicator;
                row["MEDICAID_PROVIDER_ID"] = provider.MedicaidProviderID;

                //EFFECTIVE/TERMINATION DATES
                if (provider.EffectiveDate.ToString().Length == 0)
                {
                    row["EFFECTIVE_DATE"] = DBNull.Value;
                }
                else
                {
                    row["EFFECTIVE_DATE"] = provider.EffectiveDate;
                }

                if (provider.TerminationDate.ToString().Length == 0)
                {
                    row["TERMINATION_DATE"] = DBNull.Value;
                }
                else
                {
                    row["TERMINATION_DATE"] = provider.TerminationDate;
                }
                
                row["INTERNAL_NOTES"] = provider.InternalNotes;
                row["LAST_UPDATED_BY"] = provider.LastUpdatedBy;

                dt.Rows.Add(row);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        #endregion

        #region STATIC FUNCTION: CreateFacilityProviderRelationshipTable()

        private static DataTable CreateFacilityProviderRelationshipTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("FACILITY_PROVIDER_RELATIONSHIP_ID", typeof(int));
            dt.Columns.Add("FACILITY_ID", typeof(int));
            dt.Columns.Add("PROVIDER_ID", typeof(int));
            dt.Columns.Add("EXTERNAL_PROVIDER_INDICATOR", typeof(bool));
            dt.Columns.Add("ACCEPTING_NEW_PATIENT_INDICATOR", typeof(bool));
            dt.Columns.Add("PRESCRIBER_INDICATOR", typeof(bool));
            dt.Columns.Add("REFERRALL_INDICATOR", typeof(bool));
            dt.Columns.Add("FLOAT_PROVIDER_INDICATOR", typeof(bool));
            dt.Columns.Add("EFFECTIVE_DATE", typeof(DateTime));
            dt.Columns.Add("TERMINATION_DATE", typeof(DateTime));
            dt.Columns.Add("INTERNAL_NOTES", typeof(string));
            dt.Columns.Add("LAST_UPDATED_BY", typeof(string));

            return dt;

        }
        #endregion

        #region STATIC FUNCTION: PopulateFacilityProviderRelationshipTable(FacilityProviderRelationship relationship)

        public static DataTable PopulateFacilityProviderRelationshipTable(FacilityProviderRelationship relationship)
        {
            DataTable dt = CreateFacilityProviderRelationshipTable();

            try
            {
                DataRow row = dt.NewRow();

                //Specify RelationshipID to update record
                if (relationship.RelationshipID > 0)
                {
                    row["FACILITY_PROVIDER_RELATIONSHIP_ID"] = relationship.RelationshipID;
                }

                row["FACILITY_ID"] = relationship.Facility.ID;
                row["PROVIDER_ID"] = relationship.Provider.ID;
                row["EXTERNAL_PROVIDER_INDICATOR"] = relationship.ExternalProviderIndicator;
                row["ACCEPTING_NEW_PATIENT_INDICATOR"] = relationship.AcceptingNewPatientIndicator;
                row["PRESCRIBER_INDICATOR"] = relationship.PrescriberIndicator;
                row["REFERRALL_INDICATOR"] = relationship.ReferralIndicator;
                row["FLOAT_PROVIDER_INDICATOR"] = relationship.FloatProviderIndicator;
                row["EFFECTIVE_DATE"] = relationship.EffectiveDate;
                row["TERMINATION_DATE"] = relationship.TerminationDate;
                row["PROVIDER_EMAIL"] = relationship.ProviderEmail;
                row["PROVIDER_PHONE_NUMBER"] = relationship.ProviderPhoneNumber;
                row["PROVIDER_EXTENSION_NUMBER"] = relationship.ProviderExtensionNumber;
                row["INTERNAL_NOTES"] = relationship.InternalNotes;
                row["LAST_UPDATED_BY"] = relationship.LastUpdatedBy;
                
                dt.Rows.Add(row);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }
        #endregion

        #region STATIC FUNCTION: CreateAddressTable()

        private static DataTable CreateAddressTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ADDRESS_ID", typeof(int));
            dt.Columns.Add("ADDRESS_TYPE_ID", typeof(int));
            dt.Columns.Add("ADDRESS_LINE_1", typeof(string));
            dt.Columns.Add("ADDRESS_LINE_2", typeof(string));
            dt.Columns.Add("CITY", typeof(string));
            dt.Columns.Add("STATE", typeof(string));
            dt.Columns.Add("ZIP_CODE", typeof(string));
            dt.Columns.Add("COUNTY", typeof(string));
            dt.Columns.Add("REGION", typeof(string));
            dt.Columns.Add("PHONE_NUMBER", typeof(string));
            dt.Columns.Add("PHONE_EXTENSION", typeof(string));
            dt.Columns.Add("ALTERNATE_PHONE_NUMBER", typeof(string));
            dt.Columns.Add("FAX_NUMBER", typeof(string));
            dt.Columns.Add("EMAIL", typeof(string));
            dt.Columns.Add("WEBSITE", typeof(string));
            dt.Columns.Add("CONTACT_FIRST_NAME", typeof(string));
            dt.Columns.Add("CONTACT_LAST_NAME", typeof(string));
            dt.Columns.Add("LAST_UPDATED_BY", typeof(string));

            return dt;

        }
        #endregion

        #region STATIC FUNCTION: PopulateAddressTable(Address address)

        public static DataTable PopulateAddressTable(Address address)
        {
            DataTable dt = CreateAddressTable();

            try
            {
                DataRow row = dt.NewRow();

                //Specify AddressID to update record
                if (address.ID > 0)
                {
                    row["ADDRESS_ID"] = address.ID;
                }
                
                row["ADDRESS_TYPE_ID"] = Convert.ToInt32(address.AddressType);
                row["ADDRESS_LINE_1"] = address.AddressLine1;
                row["ADDRESS_LINE_2"] = address.AddressLine2;
                row["CITY"] = address.City;
                row["STATE"] = address.State;
                row["ZIP_CODE"] = address.ZipCode;
                row["COUNTY"] = address.County;
                row["REGION"] = address.Region;
                row["PHONE_NUMBER"] = address.PhoneNumber;
                row["PHONE_EXTENSION"] = address.PhoneExtension;
                row["ALTERNATE_PHONE_NUMBER"] = address.AlternatePhoneNumber;
                row["FAX_NUMBER"] = address.FaxNumber;
                row["EMAIL"] = address.Email;
                row["WEBSITE"] = address.Website;
                row["CONTACT_FIRST_NAME"] = address.ContactFirstName;
                row["CONTACT_LAST_NAME"] = address.ContactLastName;                   
                row["LAST_UPDATED_BY"] = address.LastUpdatedBy;               

                dt.Rows.Add(row);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;

        }
        #endregion

        #region STATIC FUNCTION: CreateVendorTable()

        private static DataTable CreateVendorTable()
        {
            DataTable dt = new DataTable();
            
            dt.Columns.Add("VENDOR_ID", typeof(int));
            dt.Columns.Add("VENDOR_NAME", typeof(string));
            dt.Columns.Add("VENDOR_NPI", typeof(string));
            dt.Columns.Add("VENDOR_TAX_ID", typeof(string));
            dt.Columns.Add("EPIC_VENDOR_ID", typeof(string));
            dt.Columns.Add("EXTERNAL_ID", typeof(string));
            dt.Columns.Add("INTERNAL_NOTES", typeof(string));
            dt.Columns.Add("LAST_UPDATED_BY", typeof(string));

            return dt;

        }
        #endregion

        #region STATIC FUNCTION: PopulateVendorTable(Vendor vendor)

        public static DataTable PopulateVendorTable(Vendor vendor)
        {
            DataTable dt = CreateVendorTable();

            try
            {
                DataRow row = dt.NewRow();
                
                //Specify VendorID to update record
                if (vendor.ID > 0)
                {
                    row["VENDOR_ID"] = vendor.ID;
                }

                row["VENDOR_NAME"] = vendor.VendorName;
                row["VENDOR_NPI"] = vendor.NPI;
                row["VENDOR_TAX_ID"] = vendor.TaxID;
                row["EPIC_VENDOR_ID"] = vendor.EPICVendorID;
                row["EXTERNAL_ID"] = vendor.ExternalID;
                row["INTERNAL_NOTES"] = vendor.InternalNotes;
                row["LAST_UPDATED_BY"] = vendor.LastUpdatedBy;

                dt.Rows.Add(row);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;

        }
        #endregion

        #region STATIC FUNCTION: CreateProviderLanguageTable()

        private static DataTable CreateProviderLanguageTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("MAPPING_ID", typeof(int));
            dt.Columns.Add("PROVIDER_ID", typeof(int));
            dt.Columns.Add("LANGUAGE_ID", typeof(int));
            dt.Columns.Add("SEQUENCE_NUMBER", typeof(int));
            dt.Columns.Add("ACTIVE_STATUS", typeof(bool));

            return dt;

        }
        #endregion

        #region STATIC FUNCTION: PopulateProviderLanguageTable(int providerID, List<Language> languages)

        public static DataTable PopulateProviderLanguageTable(int providerID, List<Language> languages)
        {
            DataTable dt = CreateProviderLanguageTable();

            try
            {
                foreach (Language language in languages)
                {
                    DataRow row = dt.NewRow();
                    row["MAPPING_ID"] = language.MappingID;
                    row["PROVIDER_ID"] = providerID;
                    row["LANGUAGE_ID"] = language.ID;
                    row["SEQUENCE_NUMBER"] = language.SequenceNumber;
                    row["ACTIVE_STATUS"] = language.Status;
                    
                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;

        }
        #endregion
        
        #region STATIC FUNCTION: CreateProviderCredentialTable()

        private static DataTable CreateProviderCredentialTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("MAPPING_ID", typeof(int));
            dt.Columns.Add("PROVIDER_ID", typeof(int));
            dt.Columns.Add("CREDENTIAL_ID", typeof(int));
            dt.Columns.Add("SEQUENCE_NUMBER", typeof(int));
            dt.Columns.Add("ACTIVE_STATUS", typeof(bool));

            return dt;

        }
        #endregion

        #region STATIC FUNCTION: PopulateProviderCredentialTable(int providerID, List<Credential> credentials)

        public static DataTable PopulateProviderCredentialTable(int providerID, List<Credential> credentials)
        {
            DataTable dt = CreateProviderCredentialTable();

            try
            {
                foreach (Credential credential in credentials)
                {
                    DataRow row = dt.NewRow();
                    row["MAPPING_ID"] = credential.MappingID;
                    row["PROVIDER_ID"] = providerID;
                    row["CREDENTIAL_ID"] = credential.ID;
                    row["SEQUENCE_NUMBER"] = credential.SequenceNumber;
                    row["ACTIVE_STATUS"] = credential.Status;

                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;

        }
        #endregion

        #region STATIC FUNCTION: LoadSearchFields(Dictionary<string, List<string> args)

        public static DataTable LoadSearchFields(Dictionary<string, List<string>> args)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("FIELD_NAME", typeof(string));
            dt.Columns.Add("FIELD_VALUE", typeof(string));

            foreach (KeyValuePair<string, List<string>> pair in args)
            {
                foreach (string item in pair.Value)
                {
                    DataRow row = dt.NewRow();
                    
                    row["FIELD_NAME"] = pair.Key.ToString();
                    row["FIELD_VALUE"] = item;

                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        #endregion

        #region STATIC FUNCTION: CreateBHAttributeToRelationshipTable()

        private static DataTable CreateBHAttributeToRelationshipTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("FPR_BH_ATTRIBUTE_MAPPING_ID", typeof(int));
            dt.Columns.Add("FACILITY_PROVIDER_RELATIONSHIP_ID", typeof(int));
            dt.Columns.Add("BH_ATTRIBUTE_SET_ID", typeof(int));
            dt.Columns.Add("ACTIVE_STATUS", typeof(bool));
            
            return dt;
        }

        #endregion

        #region STATIC FUNCTION: PopulateBHAttributeToRelationshipTable(int relationshipID, List<BehavioralHealthAttribute> attributeList)

        public static DataTable PopulateBHAttributeToRelationshipTable(int relationshipID, List<BehavioralHealthAttribute> attributeList)
        {
            DataTable dt = CreateBHAttributeToRelationshipTable();

            try
            {
                foreach (BehavioralHealthAttribute attribute in attributeList)
                {
                    DataRow row = dt.NewRow();

                    row["FPR_BH_ATTRIBUTE_MAPPING_ID"] = attribute.MappingID;
                    row["FACILITY_PROVIDER_RELATIONSHIP_ID"] = relationshipID;
                    row["BH_ATTRIBUTE_SET_ID"] = attribute.SetID;
                    row["ACTIVE_STATUS"] = attribute.Status;

                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        #endregion

    }
}