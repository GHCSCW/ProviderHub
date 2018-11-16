using System;
using System.Data;
using System.Linq;
using System.Web;
using Ghc.Utility.DataAccess;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Dynamic;

namespace ProviderHubService
{
    public class DataLayer : IDisposable
    {
        #region PRIVATE VARIABLES

        GHCDataAccessLayer dataLayer = null;
        private const string DATABASE = "providerhub";

        #endregion

        #region CONSTRUCTOR

        public DataLayer()
        {
            dataLayer = GHCDataAccessLayerFactory.GetDataAccessLayer(DataProviderType.Sql, DATABASE);
        }

        #endregion

        #region FUNCTION: GetProviderByID(int providerID)

        public Provider GetProviderByID(int providerID, bool calledFromPH = false)
        {
            Provider provider = new Provider();

            string sql = (calledFromPH)? "providerHub.dbo.sp_GetProviderByID" : "providerHub.bh.sp_GetProviderByID";

            SqlParameter[] sqlParams = { new SqlParameter("PROVIDER_ID", SqlDbType.Int) { Value = providerID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var x = ds.Tables[0].AsEnumerable().FirstOrDefault();
                {
                    provider.ID = x.Field<int>("PROVIDER_ID");
                    provider.EpicProviderID = x.Field<string>("EPIC_PROVIDER_ID");
                    provider.NPI = x.Field<string>("NATIONAL_PROVIDER_IDENTIFIER");
                    provider.FirstName = x.Field<string>("PROVIDER_FIRST_NAME");
                    provider.MiddleName = x.Field<string>("PROVIDER_MIDDLE_NAME");
                    provider.LastName = x.Field<string>("PROVIDER_LAST_NAME");
                    provider.ExternalProviderID = x.Field<string>("EXTERNAL_PROVIDER_ID");
                    provider.ExternalProviderName = x.Field<string>("EXTERNAL_PROVIDER_NAME");
                    provider.DateOfBirth = Convert.ToString(x.Field<DateTime?>("PROVIDER_DATE_OF_BIRTH")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("PROVIDER_DATE_OF_BIRTH"));
                    provider.Gender = (ProviderGender)Enum.Parse(typeof(ProviderGender), x.Field<int>("PROVIDER_GENDER_ID").ToString());
                    provider.CSP_Indicator = x.Field<bool>("CSP_INDICATOR");
                    provider.MedicareIndicator = x.Field<bool?>("MEDICARE_PROVIDER_INDICATOR");
                    provider.MedicarePTAN = x.Field<string>("MEDICARE_PTAN");
                    provider.MedicareEffectiveDate = Convert.ToString(x.Field<DateTime?>("MEDICARE_EFFECTIVE_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("MEDICARE_EFFECTIVE_DATE"));
                    provider.MedicareTerminationDate = Convert.ToString(x.Field<DateTime?>("MEDICARE_TERMINATION_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("MEDICARE_TERMINATION_DATE"));
                    provider.MedicaidIndicator = (bool?)x.Field<bool?>("MEDICAID_PROVIDER_INDICATOR");
                    provider.MedicaidProviderID = x.Field<string>("MEDICAID_PROVIDER_ID");
                    provider.EffectiveDate = Convert.ToString(x.Field<DateTime?>("EFFECTIVE_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("EFFECTIVE_DATE"));
                    provider.TerminationDate = Convert.ToString(x.Field<DateTime?>("TERMINATION_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("TERMINATION_DATE"));
                    provider.InternalNotes = x.Field<string>("INTERNAL_NOTES");
                    provider.CreatedDate = x.Field<DateTime>("CREATED_DATE");
                    provider.CreatedBy = x.Field<string>("CREATED_BY");
                    provider.LastUpdatedDate = x.Field<DateTime>("LAST_UPDATED_DATE");
                    provider.LastUpdatedBy = x.Field<string>("LAST_UPDATED_BY");
                    provider.LanguageList = GetProviderLanguageByID(providerID, true);
                    provider.CredentialList = GetProviderCredentialByID(providerID, true);
                    provider.CredentialListStr = (calledFromPH)? x.Field<string>("CREDENTIAL_LIST") : "";
                    provider.ParentSpecialtyList = (calledFromPH) ? x.Field<string>("PARENT_SPECIALTY_LIST") : "";
                    provider.ChildSpecialtyList = (calledFromPH) ? x.Field<string>("CHILD_SPECIALTY_LIST") : "";
                    provider.SubSpecialtyList = (calledFromPH) ? x.Field<string>("SUB_SPECIALTY_LIST") : "";
                    provider.ProviderSpecialties = (calledFromPH) ? GetProviderSpecialties(providerID) : null;
                    provider.ProviderFacilities = (calledFromPH) ? GetProviderFacilities(providerID) : null;
                }
            }

            return provider;
        }

        #endregion

        #region FUNCTION: GetProviderSpecialties(int providerID)
        public List<Specialty> GetProviderSpecialties(int providerID) {
            List<Specialty> specialtyList = new List<Specialty>();
            string sql = "providerhub.dbo.sp_GetProviderSpecialtiesByID";
            SqlParameter[] sqlParams = { new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = providerID } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
            if (ds.Tables[0].Rows.Count > 0) {
                specialtyList = (from specialty in ds.Tables[0].AsEnumerable()
                                 select new Specialty() {
                                     MappingID = specialty.Field<int>("PROVIDER_SPECIALTY_MAPPING_ID"),
                                     ID = specialty.Field<int>("SPECIALTY_ID"),
                                     Name = specialty.Field<string>("SPECIALTY_NAME"),
                                     SequenceNumber = specialty.Field<int>("SEQUENCE_NUMBER"),
                                     EffectiveDate = specialty.Field<DateTime>("EFFECTIVE_DATE"),
                                     TerminationDate = (specialty.IsNull("TERMINATION_DATE")) ? (DateTime?)null : specialty.Field<DateTime>("TERMINATION_DATE"),
                                     ParentSpecialtyID = !specialty.IsNull("PARENT_SPECIALTY_ID") ? specialty.Field<int>("PARENT_SPECIALTY_ID") : 0,
                                     SpecialtyType = specialty.Field<string>("SPECIALTY_TYPE_NAME"),
                                     ParentName = specialty.Field<string>("PARENT_NAME"),
                                     LastUpdatedDate = specialty.IsNull("LAST_UPDATED_DATE") ? (DateTime?)null : specialty.Field<DateTime>("LAST_UPDATED_DATE"),
                                     LastUpdatedBy = specialty.Field<string>("LAST_UPDATED_BY")
                                 }).ToList();
            }
            return specialtyList;
        }
        #endregion

        #region FUNCTION: GetProviderFacilities(int providerID)
        public List<Facility> GetProviderFacilities(int providerID) {
            List<Facility> facilityList = new List<Facility>();
            string sql = "providerhub.dbo.sp_GetProviderFacilitiesByID";
            SqlParameter[] sqlParams = { new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = providerID } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
            if (ds.Tables[0].Rows.Count > 0) {
                facilityList = (from x in ds.Tables[0].AsEnumerable()
                                select new Facility() {
                                    ID = x.Field<int>("FACILITY_ID"),
                                    FacilityName = x.Field<string>("FACILITY_NAME"),
                                    NPI = x.Field<string>("FACILITY_NPI"),
                                    ExternalID = x.Field<string>("EXTERNAL_ID"),
                                    InternalNotes = x.Field<string>("FACILITY_INTERNAL_NOTES"),
                                    CreatedDate = x.Field<DateTime>("FACILITY_CREATED_DATE"),
                                    CreatedBy = x.Field<string>("FACILITY_CREATED_BY"),
                                    LastUpdatedDate = x.Field<DateTime>("FACILITY_LAST_UPDATED_DATE"),
                                    LastUpdatedBy = x.Field<string>("FACILITY_LAST_UPDATED_BY"),
                                    FacilityAddress = new Address() {
                                        AddressLine1 = x.Field<string>("ADDRESS_LINE_1"),
                                        AddressLine2 = x.Field<string>("ADDRESS_LINE_2"),
                                        City = x.Field<string>("CITY"),
                                        State = x.Field<string>("STATE"),
                                        ZipCode = x.Field<string>("ZIP_CODE"),
                                        PhoneNumber = x.Field<string>("PHONE_NUMBER"),
                                        Website = x.Field<string>("WEBSITE"),
                                        AddressTypeName = x.Field<string>("ADDRESS_TYPE_NAME"),
                                        PhoneExtension = x.Field<string>("PHONE_EXTENSION"),
                                        AlternatePhoneNumber = x.Field<string>("ALTERNATE_PHONE_NUMBER"),
                                        AlternateExtension = x.Field<string>("ALTERNATE_PHONE_EXTENSION"),
                                        FaxNumber = x.Field<string>("FAX_NUMBER"),
                                        LastUpdatedBy = x.Field<string>("ADDRESS_LAST_UPDATED_BY"),
                                        LastUpdatedDate = x.Field<DateTime>("ADDRESS_LAST_UPDATED_DATE")
                                    },
                                    FPRelationship = new FacilityProviderRelationship {
                                        RelationshipID = x.Field<int>("FACILITY_PROVIDER_RELATIONSHIP_ID"),
                                        ExternalProviderIndicator = x.Field<bool?>("EXTERNAL_PROVIDER_INDICATOR"),
                                        AcceptingNewPatientIndicator = x.Field<bool?>("ACCEPTING_NEW_PATIENT_INDICATOR"),
                                        PrescriberIndicator = x.Field<bool?>("PRESCRIBER_INDICATOR"),
                                        ReferralIndicator = x.Field<bool?>("REFERRALL_INDICATOR"),
                                        //PCPEligibleIndicator = x.Field<bool?>("PCP_ELIGIBLE_INDICATOR"),
                                        FloatProviderIndicator = x.Field<bool?>("FLOAT_PROVIDER_INDICATOR"),
                                        LastUpdatedDate = x.Field<DateTime>("FP_LAST_UPDATED_DATE"),
                                        LastUpdatedBy = x.Field<string>("FP_LAST_UPDATED_BY")
                                    }
                                }).ToList();
            }
            return facilityList;
        }
        #endregion

        #region FUNCTION: GetProviderLanguage(int providerID)

        public List<Language> GetProviderLanguageByID(int providerID, bool isCalledFromPH = false)
        {
            List<Language> languageList = new List<Language>();

            string sql = (isCalledFromPH)?"providerhub.dbo.sp_GetProviderLanguageList":"providerhub.bh.sp_GetProviderLanguageByID";

            SqlParameter[] sqlParams = { new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = providerID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                languageList = (from language in ds.Tables[0].AsEnumerable()
                                select new Language()
                                {
                                    ID = (isCalledFromPH)? language.Field<int>("LANGUAGE_ID") : language.Field<int>("PROVIDER_LANGUAGE_ID"),
                                    Name = (isCalledFromPH)? language.Field<string>("LANGUAGE_NAME") : language.Field<string>("PROVIDER_LANGUAGE_NAME"),
                                    SequenceNumber = language.Field<int>("LANGUAGE_SEQUENCE_NUMBER"),
                                    CreatedDate = language.Field<DateTime>("LANGUAGE_CREATED_DATE"),
                                    MappingID = language.Field<int>("PROVIDER_LANGUAGE_MAPPING_ID")
                                }).ToList();
            }

            return languageList;
        }

        #endregion

        #region FUNCTION: GetProviderCredentialByID(int providerID)

        public List<Credential> GetProviderCredentialByID(int providerID, bool calledFromPH=false)
        {
            List<Credential> credentialList = new List<Credential>();
            string sql = (calledFromPH)? "providerhub.dbo.sp_GetProviderCredentialByID" : "providerhub.bh.sp_GetProviderCredentialByID";

            SqlParameter[] sqlParams = { new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = providerID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                credentialList = (from credentials in ds.Tables[0].AsEnumerable()
                                  select new Credential()
                                  {
                                      ID = credentials.Field<int>("CREDENTIAL_ID"),
                                      Value = credentials.Field<string>("CREDENTIAL_VALUE"),
                                      Description = credentials.Field<string>("CREDENTIAL_DESCRIPTION"),
                                      SequenceNumber = credentials.Field<int>("CREDENTIAL_SEQUENCE_NUMBER"),
                                      CreatedDate = credentials.Field<DateTime>("CREDENTIAL_CREATED_DATE"),
                                      MappingID = credentials.Field<int>("CREDENTIAL_MAPPING_ID")
                                  }).ToList();
            }

            return credentialList;
        }

        #endregion

        #region FUNCTION: GetFacilityByID(int facilityID)

        public Facility GetFacilityByID(int facilityID, bool calledFromPH=false)
        {
            Facility facility = new Facility();

            string sql = (calledFromPH)? "providerHub.dbo.sp_GetFacilityByID":  "providerHub.bh.sp_GetFacilityByID";

            SqlParameter[] sqlParams = { new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var x = ds.Tables[0].AsEnumerable().FirstOrDefault();
                {
                    facility.ID = x.Field<int>("FACILITY_ID");
                    facility.FacilityName = x.Field<string>("FACILITY_NAME");
                    facility.NPI = x.Field<string>("FACILITY_NPI");
                    facility.ExternalID = x.Field<string>("EXTERNAL_ID");
                    facility.InternalNotes = x.Field<string>("INTERNAL_NOTES");
                    facility.CreatedDate = x.Field<DateTime>("CREATED_DATE");
                    facility.CreatedBy = x.Field<string>("CREATED_BY");
                    facility.LastUpdatedDate = x.Field<DateTime>("LAST_UPDATED_DATE");
                    facility.LastUpdatedBy = x.Field<string>("LAST_UPDATED_BY");
                    facility.FacilityAddress = GetAddressByFacilityID(facilityID, true);
                    facility.FacilitySpecialties = GetFacilitySpecialties(facilityID);
                    facility.FacilityProviders = GetFacilityProviders(facilityID);
                    facility.VendorAddresses = GetVendorAddressesByFacilityID(facilityID);
                }
            }

            return facility;
        }

        #endregion

        #region FUNCTION: GetFacilitySpecialties(int facilityID)
        public List<Specialty> GetFacilitySpecialties(int facilityID)
        {
            List<Specialty> specialtyList = new List<Specialty>();
            string sql = "providerhub.dbo.sp_GetFacilitySpecialtiesByID";
            SqlParameter[] sqlParams = { new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
            if (ds.Tables[0].Rows.Count > 0)
            {
                specialtyList = (from specialty in ds.Tables[0].AsEnumerable()
                                 select new Specialty()
                                 {
                                     MappingID = specialty.Field<int>("FACILITY_SPECIALTY_MAPPING_ID"),
                                     ID = specialty.Field<int>("SPECIALTY_ID"),
                                     Name = specialty.Field<string>("SPECIALTY_NAME"),
                                     SequenceNumber = specialty.Field<int>("SEQUENCE_NUMBER"),
                                     EffectiveDate = specialty.Field<DateTime>("EFFECTIVE_DATE"),
                                     TerminationDate = specialty.IsNull("TERMINATION_DATE") ? (DateTime?)null : specialty.Field<DateTime>("TERMINATION_DATE"),
                                     ParentSpecialtyID = !specialty.IsNull("PARENT_SPECIALTY_ID") ? specialty.Field<int>("PARENT_SPECIALTY_ID") : 0,
                                     SpecialtyType = specialty.Field<string>("SPECIALTY_TYPE_NAME"),
                                     ParentName = specialty.Field<string>("PARENT_NAME"),
                                     LastUpdatedDate = specialty.IsNull("LAST_UPDATED_DATE") ? (DateTime?)null : specialty.Field<DateTime>("LAST_UPDATED_DATE"),
                                     LastUpdatedBy = specialty.Field<string>("LAST_UPDATED_BY")
                                 }).ToList();
            }
            return specialtyList;
        }
        #endregion

        #region FUNCTION: GetFacilityProviders(int facilityID)
        public List<Provider> GetFacilityProviders(int facilityID) {
            List<Provider> providerList = new List<Provider>();
            string sql = "providerhub.dbo.sp_GetFacilityProvidersByID";
            SqlParameter[] sqlParams = { new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
            if (ds.Tables[0].Rows.Count > 0)
            {
                /*demo data only: FirstName, NPI, LastName, EpicProviderID, Credentials, Gender*/
                providerList = (from provider in ds.Tables[0].AsEnumerable()
                                select new Provider()
                                {
                                    ID = provider.Field<int>("PROVIDER_ID"),
                                    FirstName = provider.Field<string>("PROVIDER_FIRST_NAME"),
                                    LastName = provider.Field<string>("PROVIDER_LAST_NAME"),
                                    NPI = provider.Field<string>("NATIONAL_PROVIDER_IDENTIFIER"),
                                    EpicProviderID = provider.Field<string>("EPIC_PROVIDER_ID"),
                                    CredentialListStr = provider.Field<string>("CREDENTIAL_LIST"),
                                    PrimarySpecialty = provider.Field<string>("PRIMARY_SPECIALTY"),
                                    Gender = (ProviderGender)Enum.Parse(typeof(ProviderGender), provider.Field<int>("PROVIDER_GENDER_ID").ToString()),
                                    FPRelationship = new FacilityProviderRelationship().getshortObj(provider)
                                }).ToList();
            }
            return providerList;
        }
        #endregion

        public List<dynamic> GetVendorAddressesByFacilityID(int facilityID) {
            List<dynamic> VendorAddressList = new List<dynamic>();
            string sql = "providerhub.dbo.sp_GetVendorByFacilityID";
            SqlParameter[] sqlParams = { new SqlParameter("@FID", SqlDbType.Int) { Value = facilityID } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
            for (var i=0; i < ds.Tables[0].Rows.Count; i++)
            {
                dynamic toAdd = new ExpandoObject(); var row = ds.Tables[0].Rows[i];
                toAdd.VID = row.Field<int>("VENDOR_ID");
                toAdd.VendorName = row.Field<string>("VENDOR_NAME");
                toAdd.LastUpdatedBy = row.Field<string>("LAST_UPDATED_BY");
                toAdd.LastUpdatedDate = row.Field<DateTime>("LAST_UPDATED_DATE");
                //AddressTypeName, AddressLine1, AddressLine2, City, State, ZipCode, PhoneNumber, PhoneExtension, FaxNumber, AlternatePhoneNumber, Website
                toAdd.AddressTypeName = row.Field<string>("ADDRESS_TYPE_NAME");
                toAdd.AddressLine1 = row.Field<string>("ADDRESS_LINE_1");
                toAdd.AddressLine2 = row.Field<string>("ADDRESS_LINE_2");
                toAdd.City = row.Field<string>("CITY");
                toAdd.State = row.Field<string>("STATE");
                toAdd.ZipCode = row.Field<string>("ZIP_CODE");
                toAdd.PhoneNumber = row.Field<string>("PHONE_NUMBER");
                toAdd.PhoneExtension = row.Field<string>("PHONE_EXTENSION");
                toAdd.AlternatePhoneNumber = row.Field<string>("ALTERNATE_PHONE_NUMBER");
                toAdd.AlternateExtension = row.Field<string>("ALTERNATE_PHONE_EXTENSION");
                toAdd.FaxNumber = row.Field<string>("FAX_NUMBER");
                toAdd.Website = row.Field<string>("WEBSITE");
                //TODO: ORDER BY ADDRESS LAST_UPDATED DESC... AKA VERY LATEST UPDATED ONE FIRST
                VendorAddressList.Add(toAdd);
            }
            return VendorAddressList;
        }

        public dynamic PHGetVendorByID(int vendorID)
        {
            //ToReturn: Vendor.VendorField | Vendor.Addresses[i] | Vendor.Facilities[i]
            dynamic vendor = new ExpandoObject();
            //OLD METHOD 1. Populate Vendor base object (Vendor.xx) with fields from GetVendorByID
                /*string sql = "providerhub.dbo.sp_GetVendorBaseByID";
                SqlParameter[] sqlParams = { new SqlParameter("@VID", SqlDbType.Int) { Value = vendorID } };
                DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
                var row = ds.Tables[0].Rows[0]; //only 1 row result no matter what
                vendor.VID = row.Field<int>("VENDOR_ID");
                vendor.VendorName = row.Field<string>("VENDOR_NAME");
                vendor.NPI = row.Field<string>("VENDOR_NPI");
                vendor.TaxID = row.Field<string>("VENDOR_TAX_ID");
                vendor.EpicID = row.Field<string>("VENDOR_EPIC_ID");*/
            //1 AND 2. Populate Vendor.Addresses[i] with fields from GetVendorAddressesByVendorID (clone of -ByFacilityID)
            string sql = "providerhub.dbo.sp_GetVendorByID";
            SqlParameter[] sqlParams = { new SqlParameter("@VENDOR_ID", SqlDbType.Int) { Value = vendorID } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
            vendor.Addresses = new List<dynamic>();
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var row = ds.Tables[0].Rows[i]; dynamic _toAdd = new ExpandoObject();
                //1. Populate vendor base object ONCE
                if (i == 0) {
                    vendor.VID = row.Field<int>("VENDOR_ID");
                    vendor.VendorName = row.Field<string>("VENDOR_NAME");
                    vendor.NPI = row.Field<string>("VENDOR_NPI");
                    vendor.TaxID = row.Field<string>("VENDOR_TAX_ID");
                    vendor.EpicID = row.Field<string>("VENDOR_EPIC_ID");
                }
                //2. AddressTypeName, AddressLine1, AddressLine2, City, State, ZipCode, PhoneNumber, FaxNumber, AlternatePhoneNumber, Website, Exts LOOP
                _toAdd.AddressID = row.Field<int>("ADDRESS_ID");
                _toAdd.AddressTypeName = row.Field<string>("ADDRESS_TYPE_NAME");
                _toAdd.AddressLine1 = row.Field<string>("ADDRESS_LINE_1");
                _toAdd.AddressLine2 = row.Field<string>("ADDRESS_LINE_2");
                _toAdd.City = row.Field<string>("CITY");
                _toAdd.State = row.Field<string>("STATE");
                _toAdd.ZipCode = row.Field<string>("ZIP_CODE");
                _toAdd.PhoneNumber = row.Field<string>("PHONE_NUMBER");
                _toAdd.PhoneExtension = row.Field<string>("PHONE_EXTENSION");
                _toAdd.AlternatePhoneNumber = row.Field<string>("ALTERNATE_PHONE_NUMBER");
                _toAdd.AlternateExtension = row.Field<string>("ALTERNATE_PHONE_EXTENSION");
                _toAdd.FaxNumber = row.Field<string>("FAX_NUMBER");
                _toAdd.Website = row.Field<string>("WEBSITE");
                _toAdd.LastUpdatedBy = row.Field<string>("ALAST_UPDATED_BY");
                _toAdd.LastUpdatedDate = row.Field<DateTime>("ALAST_UPDATED_DATE");
                vendor.Addresses.Add(_toAdd);
            }
            //3. Vendor.Facilities[i] with fields from GetFacilitiesByVendorID
            sql = "providerhub.dbo.sp_GetFacilitiesByVendorID";
            sqlParams[0] = new SqlParameter("@VID", SqlDbType.Int) { Value = vendorID };
            ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
            vendor.Facilities = new List<dynamic>();
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                var row = ds.Tables[0].Rows[i]; dynamic _toAdd = new ExpandoObject();
                //Fields that are displayed in FacilitySearch, for the Vendor Fac DT on Vendor - Facilities page
                /*F.FACILITY_ID, F.FACILITY_NAME, F.FACILITY_NPI, A.ADDRESS_LINE_1, A.ADDRESS_LINE_2, A.CITY, A.STATE, A.ZIP_CODE, A.PHONE_NUMBER*/
                _toAdd.ID = row.Field<int>("FACILITY_ID");
                _toAdd.FacilityName = row.Field<string>("FACILITY_NAME");
                _toAdd.AddressLine1 = row.Field<string>("ADDRESS_LINE_1");
                _toAdd.AddressLine2 = row.Field<string>("ADDRESS_LINE_2");
                _toAdd.City = row.Field<string>("CITY");
                _toAdd.State = row.Field<string>("STATE");
                _toAdd.ZipCode = row.Field<string>("ZIP_CODE");
                _toAdd.PhoneNumber = row.Field<string>("PHONE_NUMBER");
                vendor.Facilities.Add(_toAdd);
            }
            //spit out object!
            return vendor;
        }

        #region FUNCTION: GetAddressByFacilityID(int facilityID)

        public Address GetAddressByFacilityID(int facilityID, bool isCalledFromPH=false)
        {
            Address address = new Address();

            string sql = (isCalledFromPH)? "providerHub.dbo.sp_GetAddressByFacilityID" : "providerHub.bh.sp_GetAddressByFacilityID";

            SqlParameter[] sqlParams = { new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var x = ds.Tables[0].AsEnumerable().FirstOrDefault();
                {
                    address.ID = x.Field<int>("ADDRESS_ID");
                    address.AddressType = (AddressType)Enum.Parse(typeof(AddressType), x.Field<int>("ADDRESS_TYPE_ID").ToString());
                    if (isCalledFromPH) { address.AddressTypeName = x.Field<string>("ADDRESS_TYPE_NAME").ToString(); }
                    address.AddressLine1 = x.Field<string>("ADDRESS_LINE_1");
                    address.AddressLine2 = x.Field<string>("ADDRESS_LINE_2");
                    address.City = x.Field<string>("CITY");
                    address.State = x.Field<string>("STATE");
                    address.ZipCode = x.Field<string>("ZIP_CODE");
                    address.County = x.Field<string>("COUNTY");
                    address.Region = x.Field<string>("REGION");
                    address.PhoneNumber = x.Field<string>("PHONE_NUMBER");
                    address.PhoneExtension = x.Field<string>("PHONE_EXTENSION");
                    address.AlternatePhoneNumber = x.Field<string>("ALTERNATE_PHONE_NUMBER");
                    if (isCalledFromPH) { address.AlternateExtension = x.Field<string>("ALTERNATE_PHONE_EXTENSION"); }
                    address.FaxNumber = x.Field<string>("FAX_NUMBER");
                    address.Email = x.Field<string>("EMAIL");
                    address.Website = x.Field<string>("WEBSITE");
                    address.ContactFirstName = x.Field<string>("CONTACT_FIRST_NAME");
                    address.ContactLastName = x.Field<string>("CONTACT_LAST_NAME");
                    address.CreatedDate = x.Field<DateTime>("CREATED_DATE");
                    address.CreatedBy = x.Field<string>("CREATED_BY");
                    address.LastUpdatedDate = x.Field<DateTime>("LAST_UPDATED_DATE");
                    address.LastUpdatedBy = x.Field<string>("LAST_UPDATED_BY");
                }
            }

            return address;
        }

        #endregion

        #region FUNCTION: GetProviderSpecialties(int id)
        #endregion

        #region FUNCTION: GetProviderList(string searchValue)

        public List<Provider> GetProviderList(string searchValue, bool calledFromPH=false)
        {
            List<Provider> providers = new List<Provider>();

            string sql = (calledFromPH)? "providerHub.dbo.sp_GetProviderList" : "providerHub.bh.sp_GetProviderList";
            SqlParameter[] sqlParams = { new SqlParameter("@SEARCH_VALUE", SqlDbType.VarChar) { Value = searchValue } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            providers = (from x in ds.Tables[0].AsEnumerable()
                         select new Provider
                         {
                             ID = x.Field<int>("PROVIDER_ID"),
                             EpicProviderID = x.Field<string>("EPIC_PROVIDER_ID"),
                             NPI = x.Field<string>("NATIONAL_PROVIDER_IDENTIFIER"),
                             FirstName = x.Field<string>("PROVIDER_FIRST_NAME"),
                             MiddleName = x.Field<string>("PROVIDER_MIDDLE_NAME"),
                             LastName = x.Field<string>("PROVIDER_LAST_NAME"),
                             DateOfBirth = Convert.ToString(x.Field<DateTime?>("PROVIDER_DATE_OF_BIRTH")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("PROVIDER_DATE_OF_BIRTH")),
                             ExternalProviderID = x.Field<string>("EXTERNAL_PROVIDER_ID"),
                             ExternalProviderName = x.Field<string>("EXTERNAL_PROVIDER_NAME"),
                             Gender = (ProviderGender)Enum.Parse(typeof(ProviderGender), x.Field<int>("PROVIDER_GENDER_ID").ToString()),
                             CSP_Indicator = x.Field<bool>("CSP_INDICATOR"),
                             MedicareIndicator = x.Field<bool>("MEDICARE_PROVIDER_INDICATOR"),
                             MedicarePTAN = x.Field<string>("MEDICARE_PTAN"),
                             MedicareEffectiveDate = Convert.ToString(x.Field<DateTime?>("MEDICARE_EFFECTIVE_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("MEDICARE_EFFECTIVE_DATE")),
                             MedicareTerminationDate = Convert.ToString(x.Field<DateTime?>("MEDICARE_TERMINATION_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("MEDICARE_TERMINATION_DATE")),
                             MedicaidIndicator = x.Field<bool>("MEDICAID_PROVIDER_INDICATOR"),
                             MedicaidProviderID = x.Field<string>("MEDICAID_PROVIDER_ID"),
                             EffectiveDate = Convert.ToString(x.Field<DateTime?>("EFFECTIVE_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("EFFECTIVE_DATE")),
                             TerminationDate = Convert.ToString(x.Field<DateTime?>("TERMINATION_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("TERMINATION_DATE")),
                             InternalNotes = x.Field<string>("INTERNAL_NOTES"),
                             CreatedDate = x.Field<DateTime>("CREATED_DATE"),
                             CreatedBy = x.Field<string>("CREATED_BY"),
                             LastUpdatedDate = x.Field<DateTime>("LAST_UPDATED_DATE"),
                             LastUpdatedBy = x.Field<string>("LAST_UPDATED_BY"),
                             CredentialListStr = (calledFromPH)? x.Field<string>("CREDENTIAL_LIST") : "",
                             PrimarySpecialty = (calledFromPH)? x.Field<string>("PRIMARY_SPECIALTY") == null? "" : x.Field<string>("PRIMARY_SPECIALTY").Split(',')[0] : ""
                         }).ToList();

            return providers;

        }
        #endregion

        #region FUNCTION: GetFacilityList(string searchValue)

        public List<Facility> GetFacilityListForProviderTab(int pid)
        {
            List<Facility> facilities = new List<Facility>();

            string sql = "providerHub.dbo.sp_GetFacilityListForProviderTab";
            SqlParameter[] sqlParams = { new SqlParameter("@PID", SqlDbType.Int) { Value = pid } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            facilities = (from x in ds.Tables[0].AsEnumerable()
                          select new Facility
                          {
                              ID = x.Field<int>("FACILITY_ID"),
                              FacilityName = x.Field<string>("FACILITY_NAME"),
                              NPI = x.Field<string>("FACILITY_NPI"),
                              ExternalID = x.Field<string>("EXTERNAL_ID"),        //should this be by specialties
                              InternalNotes = x.Field<string>("INTERNAL_NOTES"),
                              CreatedDate = x.Field<DateTime>("CREATED_DATE"),
                              CreatedBy = x.Field<string>("CREATED_BY"),
                              LastUpdatedDate = x.Field<DateTime>("LAST_UPDATED_DATE"),
                              LastUpdatedBy = x.Field<string>("LAST_UPDATED_BY"),
                              FacilityAddress = new Address()
                              {
                                  AddressLine1 = x.Field<string>("ADDRESS_LINE_1"),
                                  AddressLine2 = x.Field<string>("ADDRESS_LINE_2"),
                                  City = x.Field<string>("CITY"),
                                  State = x.Field<string>("STATE"),
                                  ZipCode = x.Field<string>("ZIP_CODE"),
                                  PhoneNumber = x.Field<string>("PHONE_NUMBER"),
                                  Website = x.Field<string>("WEBSITE"),
                                  AddressTypeName = x.Field<string>("ADDRESS_TYPE_NAME"),
                                  PhoneExtension = x.Field<string>("PHONE_EXTENSION"),
                                  AlternatePhoneNumber = x.Field<string>("ALTERNATE_PHONE_NUMBER"),
                                  AlternateExtension = x.Field<string>("ALTERNATE_PHONE_EXTENSION"),
                                  FaxNumber = x.Field<string>("FAX_NUMBER"),
                                  LastUpdatedBy = x.Field<string>("ADDRESS_LAST_UPDATED_BY"),
                                  LastUpdatedDate = x.Field<DateTime>("ADDRESS_LAST_UPDATED_DATE")
                              }
                          }).ToList();

            return facilities;

        }

        public List<Facility> GetFacilityList(string searchValue, bool isCalledFromPH = false)
        {
            List<Facility> facilities = new List<Facility>();

            string sql = (isCalledFromPH)? "providerHub.dbo.sp_GetFacilityList" : "providerHub.bh.sp_GetFacilityList";
            SqlParameter[] sqlParams = { new SqlParameter("@SEARCH_VALUE", SqlDbType.VarChar) { Value = searchValue } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            facilities = (from x in ds.Tables[0].AsEnumerable()
                          select new Facility
                          {
                              ID = x.Field<int>("FACILITY_ID"),
                              FacilityName = x.Field<string>("FACILITY_NAME"),
                              NPI = x.Field<string>("FACILITY_NPI"),
                              ExternalID = x.Field<string>("EXTERNAL_ID"),        //should this be by specialties
                              InternalNotes = x.Field<string>("INTERNAL_NOTES"),
                              CreatedDate = x.Field<DateTime>("CREATED_DATE"),
                              CreatedBy = x.Field<string>("CREATED_BY"),
                              LastUpdatedDate = x.Field<DateTime>("LAST_UPDATED_DATE"),
                              LastUpdatedBy = x.Field<string>("LAST_UPDATED_BY"),
                              FacilityAddress = (from address in ds.Tables[0].AsEnumerable()
                                                 where address.Field<int>("FACILITY_ID") == x.Field<int>("FACILITY_ID")
                                                 select new Address()
                                                 {
                                                     ID = address.Field<int>("ADDRESS_ID"),
                                                     AddressType = (AddressType)Enum.Parse(typeof(AddressType), address.Field<int>("ADDRESS_TYPE_ID").ToString()),
                                                     AddressLine1 = address.Field<string>("ADDRESS_LINE_1"),
                                                     AddressLine2 = address.Field<string>("ADDRESS_LINE_2"),
                                                     City = address.Field<string>("CITY"),
                                                     State = address.Field<string>("STATE"),
                                                     ZipCode = address.Field<string>("ZIP_CODE"),
                                                     County = address.Field<string>("COUNTY"),
                                                     Region = address.Field<string>("REGION"),
                                                     PhoneNumber = address.Field<string>("PHONE_NUMBER"),
                                                     PhoneExtension = address.Field<string>("PHONE_EXTENSION"),
                                                     AlternatePhoneNumber = address.Field<string>("ALTERNATE_PHONE_NUMBER"),
                                                     FaxNumber = address.Field<string>("FAX_NUMBER"),
                                                     Email = address.Field<string>("EMAIL"),
                                                     Website = address.Field<string>("WEBSITE"),
                                                     ContactFirstName = address.Field<string>("CONTACT_FIRST_NAME"),
                                                     ContactLastName = address.Field<string>("CONTACT_LAST_NAME"),
                                                     CreatedDate = Convert.ToDateTime(address.Field<DateTime>("ADDRESS_CREATED_DATE")),
                                                     CreatedBy = address.Field<string>("ADDRESS_CREATED_BY"),
                                                     LastUpdatedDate = Convert.ToDateTime(address.Field<DateTime>("ADDRESS_LAST_UPDATED_DATE")),
                                                     LastUpdatedBy = address.Field<string>("ADDRESS_LAST_UPDATED_BY")
                                                 }).First()
                          }).ToList();

            return facilities;

        }

        #endregion

        #region FUNCTION: GetFacilityProviderRelationshipList(string searchValue)

        public List<FacilityProviderRelationship> GetFacilityProviderRelationshipList(string searchValue)
        {
            string sql = "providerhub.bh.sp_GetFacilityProviderRelationshipList";
            List<FacilityProviderRelationship> relationship = new List<FacilityProviderRelationship>();

            SqlParameter[] sqlParams = { new SqlParameter("@VALUE", SqlDbType.VarChar) { Value = searchValue } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                relationship = ParseFacilityProviderData(ds);
            }

            return relationship;
        }

        #endregion

        #region FUNCTION: GetFacilityProviderRelationshipByID(int relationshipID)

        public FacilityProviderRelationship GetFacilityProviderRelationshipByID(int relationshipID)
        {
            FacilityProviderRelationship relationship = new FacilityProviderRelationship();
            string sql = "providerhub.bh.sp_GetFacilityProviderRelationshipByID";

            SqlParameter[] sqlParams = { new SqlParameter("@RELATIONSHIP_ID", SqlDbType.Int) { Value = relationshipID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                relationship = ParseFacilityProviderData(ds).FirstOrDefault();
            }

            return relationship;
        }

        #endregion

        #region FUNCTION: ParseFacilityProviderData(Dataset ds)

        private List<FacilityProviderRelationship> ParseFacilityProviderData(DataSet ds)
        {
            List<FacilityProviderRelationship> facilityProviderList = new List<FacilityProviderRelationship>();

            facilityProviderList = (from x in ds.Tables[0].AsEnumerable()
                                    select new FacilityProviderRelationship
                                    {
                                        RelationshipID = x.Field<int>("FACILITY_PROVIDER_RELATIONSHIP_ID"),
                                        Facility = (from facility in ds.Tables[0].AsEnumerable()
                                                    where facility.Field<int>("FACILITY_ID") == x.Field<int>("FACILITY_ID")
                                                    select new Facility()
                                                    {
                                                        ID = facility.Field<int>("FACILITY_ID"),
                                                        FacilityName = facility.Field<string>("FACILITY_NAME"),
                                                        NPI = facility.Field<string>("FACILITY_NPI"),
                                                        ExternalID = facility.Field<string>("EXTERNAL_ID"),
                                                        InternalNotes = facility.Field<string>("FACILITY_INTERNAL_NOTES"),
                                                        CreatedDate = facility.Field<DateTime>("FACILITY_CREATED_DATE"),
                                                        CreatedBy = facility.Field<string>("FACILITY_CREATED_BY"),
                                                        LastUpdatedDate = facility.Field<DateTime>("FACILITY_LAST_UPDATED_DATE"),
                                                        LastUpdatedBy = facility.Field<string>("FACILITY_LAST_UPDATED_BY"),
                                                        FacilityAddress = (from address in ds.Tables[0].AsEnumerable()
                                                                           where address.Field<int>("FACILITY_ID") == x.Field<int>("FACILITY_ID")
                                                                           select new Address()
                                                                           {
                                                                               ID = address.Field<int>("ADDRESS_ID"),
                                                                               AddressType = (AddressType)Enum.Parse(typeof(AddressType), address.Field<int>("ADDRESS_TYPE_ID").ToString()),
                                                                               AddressLine1 = address.Field<string>("ADDRESS_LINE_1"),
                                                                               AddressLine2 = address.Field<string>("ADDRESS_LINE_2"),
                                                                               City = address.Field<string>("CITY"),
                                                                               State = address.Field<string>("STATE"),
                                                                               ZipCode = address.Field<string>("ZIP_CODE"),
                                                                               County = address.Field<string>("COUNTY"),
                                                                               Region = address.Field<string>("REGION"),
                                                                               PhoneNumber = address.Field<string>("PHONE_NUMBER"),
                                                                               PhoneExtension = address.Field<string>("PHONE_EXTENSION"),
                                                                               AlternatePhoneNumber = address.Field<string>("ALTERNATE_PHONE_NUMBER"),
                                                                               FaxNumber = address.Field<string>("FAX_NUMBER"),
                                                                               Email = address.Field<string>("EMAIL"),
                                                                               Website = address.Field<string>("WEBSITE"),
                                                                               ContactFirstName = address.Field<string>("CONTACT_FIRST_NAME"),
                                                                               ContactLastName = address.Field<string>("CONTACT_LAST_NAME"),
                                                                               CreatedDate = Convert.ToDateTime(address.Field<DateTime>("ADDRESS_CREATED_DATE")),
                                                                               CreatedBy = address.Field<string>("ADDRESS_CREATED_BY"),
                                                                               LastUpdatedDate = Convert.ToDateTime(address.Field<DateTime>("ADDRESS_LAST_UPDATED_DATE")),
                                                                               LastUpdatedBy = address.Field<string>("ADDRESS_LAST_UPDATED_BY")
                                                                           }).First()
                                                    }).First(),
                                        Provider = (from provider in ds.Tables[0].AsEnumerable()
                                                    where provider.Field<int>("PROVIDER_ID") == x.Field<int>("PROVIDER_ID")
                                                    select new Provider()
                                                    {
                                                        ID = provider.Field<int>("PROVIDER_ID"),
                                                        EpicProviderID = provider.Field<string>("EPIC_PROVIDER_ID"),
                                                        NPI = provider.Field<string>("NATIONAL_PROVIDER_IDENTIFIER"),
                                                        FirstName = provider.Field<string>("PROVIDER_FIRST_NAME"),
                                                        MiddleName = provider.Field<string>("PROVIDER_MIDDLE_NAME"),
                                                        LastName = provider.Field<string>("PROVIDER_LAST_NAME"),
                                                        ExternalProviderID = provider.Field<string>("EXTERNAL_PROVIDER_ID"),
                                                        ExternalProviderName = provider.Field<string>("EXTERNAL_PROVIDER_NAME"),
                                                        DateOfBirth = Convert.ToString(provider.Field<DateTime?>("PROVIDER_DATE_OF_BIRTH")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(provider.Field<DateTime>("PROVIDER_DATE_OF_BIRTH")),
                                                        Gender = (ProviderGender)Enum.Parse(typeof(ProviderGender), provider.Field<int>("PROVIDER_GENDER_ID").ToString()),
                                                        CSP_Indicator = provider.Field<bool>("CSP_INDICATOR"),
                                                        MedicareIndicator = provider.Field<bool>("MEDICARE_PROVIDER_INDICATOR"),
                                                        MedicarePTAN = provider.Field<string>("MEDICARE_PTAN"),
                                                        MedicareEffectiveDate = Convert.ToString(provider.Field<DateTime?>("MEDICARE_EFFECTIVE_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(provider.Field<DateTime>("MEDICARE_EFFECTIVE_DATE")),
                                                        MedicareTerminationDate = Convert.ToString(provider.Field<DateTime?>("MEDICARE_TERMINATION_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(provider.Field<DateTime>("MEDICARE_TERMINATION_DATE")),
                                                        MedicaidIndicator = provider.Field<bool>("MEDICAID_PROVIDER_INDICATOR"),
                                                        MedicaidProviderID = provider.Field<string>("MEDICAID_PROVIDER_ID"),
                                                        EffectiveDate = Convert.ToString(provider.Field<DateTime?>("EFFECTIVE_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(provider.Field<DateTime>("EFFECTIVE_DATE")),
                                                        TerminationDate = Convert.ToString(provider.Field<DateTime?>("TERMINATION_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(provider.Field<DateTime>("TERMINATION_DATE")),
                                                        InternalNotes = provider.Field<string>("PROVIDER_INTERNAL_NOTES"),
                                                        CreatedDate = provider.Field<DateTime>("PROVIDER_CREATED_DATE"),
                                                        CreatedBy = provider.Field<string>("PROVIDER_CREATED_BY"),
                                                        LastUpdatedDate = provider.Field<DateTime>("PROVIDER_LAST_UPDATED_DATE"),
                                                        LastUpdatedBy = provider.Field<string>("PROVIDER_LAST_UPDATED_BY"),
                                                        CredentialList = GetProviderCredentialByID(provider.Field<int>("PROVIDER_ID")),
                                                        LanguageList = GetProviderLanguageByID(provider.Field<int>("PROVIDER_ID"))
                                                    }).First(),
                                        AcceptingNewPatientIndicator = (bool)x.Field<bool>("ACCEPTING_NEW_PATIENT_INDICATOR"),
                                        EffectiveDate = Convert.ToDateTime(x.Field<DateTime>("FP_EFFECTIVE_DATE")),
                                        TerminationDate = Convert.ToDateTime(x.Field<DateTime>("FP_TERMINATION_DATE")),
                                        ExternalProviderIndicator = (bool)x.Field<bool>("EXTERNAL_PROVIDER_INDICATOR"),
                                        FloatProviderIndicator = (bool)x.Field<bool>("FLOAT_PROVIDER_INDICATOR"),
                                        PrescriberIndicator = (bool)x.Field<bool>("PRESCRIBER_INDICATOR"),
                                        ReferralIndicator = (bool)x.Field<bool>("REFERRALL_INDICATOR"),
                                        ProviderEmail = x.Field<string>("FP_PROVIDER_EMAIL"),
                                        ProviderPhoneNumber = x.Field<string>("FP_PROVIDER_PHONE_NUMBER"),
                                        ProviderExtensionNumber = x.Field<string>("FP_PROVIDER_PHONE_EXTENSION"),
                                        InternalNotes = x.Field<string>("FP_INTERNAL_NOTES"),
                                        CreatedDate = Convert.ToDateTime(x.Field<DateTime>("FP_CREATED_DATE")),
                                        CreatedBy = x.Field<string>("FP_CREATED_BY"),
                                        LastUpdatedDate = Convert.ToDateTime(x.Field<DateTime>("FP_LAST_UPDATED_DATE")),
                                        LastUpdatedBy = x.Field<string>("FP_LAST_UPDATED_BY"),
                                    }).ToList();

            return facilityProviderList;
        }

        #endregion

        #region FUNCTION: SaveProviderDetail(Provider provider)

        public int SaveProviderDetail(Provider provider)
        {
            string sql = "providerhub.bh.sp_SaveProviderDetail";

            DataTable dt = DataUtilityHelper.PopulateProviderDetailsTable(provider);

            SqlParameter[] sqlParams = { new SqlParameter("@PROVIDER_DETAIL", SqlDbType.Structured) { Value = dt } };

            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        public int SaveProviderHeader(dynamic p) {
            string sql = "providerhub.dbo.sp_SaveProviderHeader";
            //FirstName: val("FirstName"), LastName: val("LastName"), Credentials: val("Credentials"), NPI: val("NPI"), EpicProviderID: val("EpicProviderID"), Gender: val("Gender")
            SqlParameter[] sqlParams = { new SqlParameter("@FirstName", SqlDbType.VarChar){ Value = p.FirstName }, new SqlParameter("@LastName", SqlDbType.VarChar){ Value = p.LastName }, 
                                         new SqlParameter("@Credentials", SqlDbType.VarChar){ Value = p.Credstr }, new SqlParameter("@NPI", SqlDbType.VarChar){ Value = p.NPI }, 
                                         new SqlParameter("@EpicProviderID", SqlDbType.VarChar){ Value = p.EpicProviderID }, new SqlParameter("@Gender", SqlDbType.Int){ Value = p.Gender },
                                         new SqlParameter("@User", SqlDbType.VarChar){ Value = p.User }, new SqlParameter("@ID",SqlDbType.Int){ Value = p.ID } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        public int SaveProviderDemo(dynamic p) {
            string sql = "providerhub.dbo.sp_SaveProviderDemo";
            //{ Languages: val2("Language"), MedicareIndicator: val2("MedicareIndicator"), MedicaidIndicator: val2("MedicaidIndicator") }
            // if Medicare: MedicarePTAN, MedicareEffectiveDate, MedicareTerminationDate | if Medicaid: MedicaidProviderID | (else these are present in p but null)
            SqlParameter[] sqlParams = { new SqlParameter("@MedicareIndicator", SqlDbType.Bit){ Value = p.MedicareIndicator }, new SqlParameter("@MedicaidIndicator", SqlDbType.Bit){ Value = p.MedicaidIndicator },
                                         new SqlParameter("@Languages", SqlDbType.VarChar){ Value = p.Langstr }, new SqlParameter("@MedicarePTAN", SqlDbType.VarChar){ Value = p.MedicarePTAN },
                                         new SqlParameter("@MedicareEffectiveDate", SqlDbType.Date){  Value = p.MedicareEffectiveDate == null? DBNull.Value : p.MedicareEffectiveDate },
                                         new SqlParameter("@MedicareTerminationDate", SqlDbType.Date){ Value = p.MedicareTerminationDate == null? DBNull.Value : p.MedicareTerminationDate },
                                         new SqlParameter("@MedicaidProviderID", SqlDbType.VarChar){ Value = p.MedicaidProviderID }, new SqlParameter("@User", SqlDbType.VarChar){ Value = p.User },
                                         new SqlParameter("@ID",SqlDbType.Int){ Value = p.ID } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }
        public int SaveProviderSpecialty(dynamic ps) {
            //Stored Proc needs: (@SpecialtyID VARCHAR(10), @User VARCHAR(20), @ID INT,      @SEQ INT, @EDATE DATE,       @TDATE DATE = NULL,   @First BIT = 0)
            //                    ps.SpecialtyID            ps.User            ps.ID         ps.SEQ    ps.EDATE           ps.TDATE              ps.First
            string sql = "providerhub.dbo.sp_SaveProviderSpecialty";
            SqlParameter[] sqlParams = { new SqlParameter("@SpecialtyID", SqlDbType.VarChar){ Value = ps.SpecialtyID }, new SqlParameter("@User", SqlDbType.VarChar){ Value = ps.User },
                                         new SqlParameter("@ID", SqlDbType.Int){ Value = ps.ID }, new SqlParameter("@SEQ", SqlDbType.VarChar){ Value = ps.SEQ },
                                         new SqlParameter("@EDATE", SqlDbType.Date){ Value = ps.EDATE }, new SqlParameter("@TDATE", SqlDbType.Date){ Value = (ps.TDATE==null)? (DateTime?)null : ps.TDATE, IsNullable = true },
                                         new SqlParameter("@First", SqlDbType.Bit){ Value = ps.First }, new SqlParameter("@Last", SqlDbType.Bit){ Value = ps.Last } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        /*
         dynamic forSP = new ExpandoObject(); forSP.FacilityID = inputJSON.ProviderFacilities[i].ID; forSP.ID = inputJSON.ID; forSP.User = inputJSON.User;
            forSP.ExternalProviderIndicator = (pf.ExternalProviderIndicator == "Yes") ? 1 : (pf.ExternalProviderIndicator == "No") ? (int?)0 : null;
            forSP.AcceptingNewPatientIndicator = (pf.AcceptingNewPatientIndicator == "Yes") ? 1 : (pf.AcceptingNewPatientIndicator == "No") ? (int?)0 : null;
            forSP.PrescriberIndicator = (pf.PrescriberIndicator == "Yes") ? 1 : (pf.PrescriberIndicator == "No") ? (int?)0 : null;
            forSP.ReferralIndicator = (pf.ReferralIndicator == "Yes") ? 1 : (pf.ReferralIndicator == "No") ? (int?)0 : null;
            forSP.FloatProviderIndicator = (pf.FloatProviderIndicator == "Yes") ? 1 : (pf.FloatProviderIndicator == "No") ? (int?)0 : null;
            forSP.First = (i == 0) ? 1 : 0; forSP.Last = (i == inputJSON.ProviderFacilities.Count - 1) ? 1 : 0;
         */
        public int SaveProviderFacility(dynamic ps) { //forSP passed into ps
            //Stored Proc needs: @FacilityID VARCHAR(10),@User VARCHAR(20),@ID INT, @ExternalProviderIndicator BIT, @AcceptingNewPatientIndicator BIT, @PrescriberIndicator BIT,
            //                   @ReferralIndicator BIT, @FloatProviderIndicator BIT, @First BIT = 0, @Last BIT = 0
            //                   forSP.FacilityID, forSP.User, forSP.ExternalProviderIndicator, forSP.AcceptingNewPa...
            string sql = "providerhub.dbo.sp_SaveProviderFacility";
            SqlParameter[] sqlParams = { new SqlParameter("@FacilityID", SqlDbType.VarChar){ Value = ps.FacilityID }, new SqlParameter("@User", SqlDbType.VarChar){ Value = ps.User },
                                         new SqlParameter("@ID", SqlDbType.Int){ Value = ps.ID }, new SqlParameter("@MappingID", SqlDbType.Int){ Value=ps.MappingID },
                                         new SqlParameter("@ExternalProviderIndicator", SqlDbType.Bit){ Value = ps.ExternalProviderIndicator, IsNullable=true },
                                         new SqlParameter("@AcceptingNewPatientIndicator", SqlDbType.Bit){ Value = ps.AcceptingNewPatientIndicator, IsNullable=true },
                                         new SqlParameter("@PrescriberIndicator", SqlDbType.Bit){ Value = ps.PrescriberIndicator, IsNullable=true },
                                         new SqlParameter("@ReferralIndicator", SqlDbType.Bit){ Value = ps.ReferralIndicator, IsNullable=true },
                                         new SqlParameter("@FloatProviderIndicator", SqlDbType.Bit){ Value = ps.FloatProviderIndicator, IsNullable=true },
                                         new SqlParameter("@First", SqlDbType.Bit){ Value = ps.First }, new SqlParameter("@Last", SqlDbType.Bit){ Value = ps.Last } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        public int SaveFacilityHeader(dynamic p) {
            string sql = "providerhub.dbo.sp_SaveFacilityHeader";
            //body = { Name:val("Name"), NPI: val("NPI"), User: "GHC-HMO\\spillai" };
            SqlParameter[] sqlParams = { new SqlParameter("@Name", SqlDbType.VarChar){ Value = p.Name }, new SqlParameter("@NPI", SqlDbType.VarChar){ Value = p.NPI },
                                         new SqlParameter("@ID", SqlDbType.Int){ Value = p.ID }, new SqlParameter("@User", SqlDbType.VarChar){ Value = p.User } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        public int SaveFacilityDemo(dynamic p) {
            string sql = "providerhub.dbo.sp_SaveFacilityDemo";
            /*body = { Address1: val2("Address1"), Address2: val2("Address2"), City: val2("City"), State: val2("State"), Zip: val2("ZipCode"), User: "GHC-HMO\\spillai" };
                 body.PhoneNumber = val2("PhoneNumber"); body.FaxNumber = val2("FaxNumber"); body.Website = val2("Website");*/ //add alternate phone and alternate phone ext, and phone ext
            SqlParameter[] sqlParams = { new SqlParameter("@Address1", SqlDbType.VarChar){ Value = p.Address1 }, new SqlParameter("@Address2", SqlDbType.VarChar){ Value = p.Address2 },
                                         new SqlParameter("@City", SqlDbType.VarChar){ Value = p.City }, new SqlParameter("@State", SqlDbType.VarChar){ Value = p.State },
                                         new SqlParameter("@Zip", SqlDbType.VarChar){  Value = p.Zip }, new SqlParameter("@User", SqlDbType.VarChar){ Value = p.User },
                                         new SqlParameter("@PhoneNumber", SqlDbType.VarChar){ Value = p.PhoneNumber }, new SqlParameter("@PhoneExtension", SqlDbType.VarChar){ Value = p.PhoneExtension },
                                         new SqlParameter("@AltPhoneNumber", SqlDbType.VarChar){ Value = p.AlternatePhoneNumber }, new SqlParameter("@AltPhoneExtension", SqlDbType.VarChar){ Value = p.AlternateExtension },
                                         new SqlParameter("@FaxNumber", SqlDbType.VarChar){ Value = p.FaxNumber },
                                         new SqlParameter("@Website",SqlDbType.VarChar){ Value = p.Website }, new SqlParameter("@ID", SqlDbType.Int){ Value = p.ID } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }
        public int SaveFacilitySpecialty(dynamic ps)
        {
            //Stored Proc needs: (@SpecialtyID VARCHAR(10), @User VARCHAR(20), @ID INT,      @SEQ INT, @EDATE DATE,       @TDATE DATE = NULL,   @First BIT = 0)
            //                    ps.SpecialtyID            ps.User            ps.ID (FAC)   ps.SEQ    ps.EDATE           ps.TDATE              ps.First
            string sql = "providerhub.dbo.sp_SaveFacilitySpecialty";
            SqlParameter[] sqlParams = { new SqlParameter("@SpecialtyID", SqlDbType.VarChar){ Value = ps.SpecialtyID }, new SqlParameter("@User", SqlDbType.VarChar){ Value = ps.User },
                                         new SqlParameter("@ID", SqlDbType.Int){ Value = ps.ID }, new SqlParameter("@SEQ", SqlDbType.VarChar){ Value = ps.SEQ },
                                         new SqlParameter("@EDATE", SqlDbType.Date){ Value = ps.EDATE }, new SqlParameter("@TDATE", SqlDbType.Date){ Value = (ps.TDATE==null)? (DateTime?)null : ps.TDATE, IsNullable = true },
                                         new SqlParameter("@First", SqlDbType.Bit){ Value = ps.First }, new SqlParameter("@Last",SqlDbType.Bit){ Value = ps.Last } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        public int SaveVendorHeader(dynamic p) {
            string sql = "providerhub.dbo.sp_SaveVendorHeader";
            //body = { Name:val("Name"), NPI: val("NPI"), TaxID + EpicID, User: "GHC-HMO\\spillai" };
            SqlParameter[] sqlParams = { new SqlParameter("@Name", SqlDbType.VarChar){ Value = p.Name }, new SqlParameter("@NPI", SqlDbType.VarChar){ Value = p.NPI },
                                         new SqlParameter("@TaxID", SqlDbType.VarChar){ Value = p.TaxID }, new SqlParameter("@EpicID", SqlDbType.VarChar){ Value = p.EpicID },
                                         new SqlParameter("@VID", SqlDbType.Int){ Value = p.ID }, new SqlParameter("@User", SqlDbType.VarChar){ Value = p.User } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        public int SaveVendorAddress(dynamic p) {
            string sql = "providerhub.dbo.sp_SaveVendorAddress";
            /*body = { AddressID: x, Address1: val2("Address1"), Address2: val2("Address2"), City: val2("City"), State: val2("State"), Zip: val2("ZipCode"), User: "GHC-HMO\\spillai" };
                 body.PhoneNumber = val2("PhoneNumber"); body.FaxNumber = val2("FaxNumber"); body.Website = val2("Website");*/ //add alternate phone and alternate phone ext, and phone ext
            SqlParameter[] sqlParams = { new SqlParameter("@Address1", SqlDbType.VarChar){ Value = p.Address1 }, new SqlParameter("@Address2", SqlDbType.VarChar){ Value = p.Address2 },
                                         new SqlParameter("@City", SqlDbType.VarChar){ Value = p.City }, new SqlParameter("@State", SqlDbType.VarChar){ Value = p.State },
                                         new SqlParameter("@Zip", SqlDbType.VarChar){  Value = p.Zip }, new SqlParameter("@User", SqlDbType.VarChar){ Value = p.User },
                                         new SqlParameter("@PhoneNumber", SqlDbType.VarChar){ Value = p.PhoneNumber }, new SqlParameter("@PhoneExtension", SqlDbType.VarChar){ Value = p.PhoneExtension },
                                         new SqlParameter("@AltPhoneNumber", SqlDbType.VarChar){ Value = p.AlternatePhoneNumber }, new SqlParameter("@AltPhoneExtension", SqlDbType.VarChar){ Value = p.AlternateExtension },
                                         new SqlParameter("@FaxNumber", SqlDbType.VarChar){ Value = p.FaxNumber },
                                         new SqlParameter("@Website",SqlDbType.VarChar){ Value = p.Website }, new SqlParameter("@AID", SqlDbType.Int){ Value = p.AddressID } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        #region FUNCTION: SaveFacility(Facility facility)

        public int SaveFacility(Facility facility)
        {
            string sql = "providerhub.bh.sp_SaveFacilityData";

            DataTable dt = DataUtilityHelper.PopulateFacilityTable(facility);

            SqlParameter[] sqlParams = { new SqlParameter("@FACILITY_DATA", SqlDbType.Structured) { Value = dt } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        #region FUNCTION: SaveFacilityProviderRelationship(FacilityProviderRelationship relationship)

        public int SaveFacilityProviderRelationship(FacilityProviderRelationship relationship)
        {
            string sql = "providerhub.bh.sp_SaveFacilityProviderRelationship";

            DataTable dt = DataUtilityHelper.PopulateFacilityProviderRelationshipTable(relationship);

            SqlParameter[] sqlParams = { new SqlParameter("@FACILITY_PROVIDER_RELATIONSHIP", SqlDbType.Structured) { Value = dt } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        #region FUNCTION: SaveAddress(Address address)

        public int SaveAddress(Address address)
        {
            string sql = "providerhub.bh.sp_SaveAddress";

            DataTable dt = DataUtilityHelper.PopulateAddressTable(address);

            SqlParameter[] sqlParams = { new SqlParameter("@ADDRESS", SqlDbType.Structured) { Value = dt } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        #region FUNCTION: MapAddressToFacility(int facilityID, int addressID, string createdBy)

        public int MapAddressToFacility(int facilityID, int addressID, string createdBy)
        {
            string sql = "providerhub.bh.sp_MapAddressToFacility";

            SqlParameter[] sqlParams = {
                                            new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID },
                                            new SqlParameter("@ADDRESS_ID", SqlDbType.Int) { Value = addressID },
                                            new SqlParameter("@CREATED_BY", SqlDbType.VarChar) { Value = createdBy }
                                        };

            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        #region FUNCTION: GetLanguageList()

        public List<Language> GetLanguageList(bool isCalledFromPH=false)
        {
            List<Language> languageList = new List<Language>();
            string sql = (isCalledFromPH)? "providerhub.dbo.sp_GetLanguageList" : "providerhub.bh.sp_GetLanguageList";

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure);

            if (ds.Tables[0].Rows.Count > 0)
            {
                languageList = (from language in ds.Tables[0].AsEnumerable()
                                select new Language()
                                {
                                    ID = (isCalledFromPH)? language.Field<int>("LANGUAGE_ID") : language.Field<int>("PROVIDER_LANGUAGE_ID"),
                                    Name = (isCalledFromPH) ? language.Field<string>("LANGUAGE_NAME") : language.Field<string>("PROVIDER_LANGUAGE_NAME")
                                }).ToList();
            }

            return languageList;
        }

        public string GetHospitalAffiliationByPID(int pid) {
            string toReturn = "";
            string sql = "providerhub.dbo.sp_GetHospitalAffiliationByID";
            SqlParameter[] sqlParams = { new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = pid } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
            foreach (DataRow dr in ds.Tables[0].Rows) {
                toReturn += dr.Field<string>("HOSPITAL_NAME") + ",";
            }
            return toReturn.TrimEnd(',');
        }

        //{ data: "Network" }, { data: "NetworkEffectiveDate" }, { data: "Provider" }, { data: "Facility" }, { data: "Specialty" }, { data: "EpicNetworkID" }
        public List<Directory> GetNetworkTabByPID(int pid) {
            List<Directory> toReturn = new List<Directory>();
            string sql = "providerhub.dbo.sp_GetProviderNetworkTabByID";
            SqlParameter[] sqlParams = {
                                            new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = pid }
                                        };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
            if (ds.Tables[0].Rows.Count > 0)
            {
                toReturn = (from spec in ds.Tables[0].AsEnumerable()
                            select new Directory()
                            {
                                ID = spec.Field<int>("DIRECTORY_ID"),
                                Network = spec.Field<string>("NETWORK_NAME"),
                                NetworkEffectiveDate = spec.Field<DateTime>("NETWORK_EFFECTIVE_DATE"),
                                Provider = spec.Field<string>("PROVIDER_NAME"),
                                Facility = spec.Field<string>("FACILITY_NAME"),
                                Specialty = spec.Field<string>("SPECIALTY_NAME"),
                                PCPEligibleIndicator = spec.Field<bool?>("PCP_ELIGIBLE_INDICATOR"),
                                EpicNetworkID = spec.Field<int>("EPIC_NETWORK_ID"),
                                DirectoryEffectiveDate = spec.Field<DateTime>("EFFECTIVE_DATE"),  
                                DirectoryTerminationDate = spec.Field<DateTime>("TERMINATION_DATE"),
                                /*indicators*/
                                FPRelationship = new FacilityProviderRelationship() {
                                    RelationshipID = spec.Field<int>("EMPLOYMENT_FACPROV_ID"),
                                    ExternalProviderIndicator = spec.Field<bool?>("EXTERNAL_PROVIDER_INDICATOR"),
                                    AcceptingNewPatientIndicator = spec.Field<bool?>("ACCEPTING_NEW_PATIENT_INDICATOR"),
                                    PrescriberIndicator = spec.Field<bool?>("PRESCRIBER_INDICATOR"),
                                    ReferralIndicator = spec.Field<bool?>("REFERRALL_INDICATOR"),
                                    //PCPEligibleIndicator = spec.Field<bool?>("PCP_ELIGIBLE_INDICATOR"),
                                    FloatProviderIndicator = spec.Field<bool?>("FLOAT_PROVIDER_INDICATOR")
                                    
                                }
                            }).ToList();
            }
            return toReturn;
        }

        #endregion

        public List<Specialty> GetSpecialtyList(bool isCalledFromPH = false) {
            List<Specialty> toReturn = new List<Specialty>();
            string sql = "providerhub.dbo.sp_GetSpecialtyList";
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure);
            if (ds.Tables[0].Rows.Count > 0) {
                toReturn = (from spec in ds.Tables[0].AsEnumerable()
                            select new Specialty() {
                                ID = spec.Field<int>("SPECIALTY_ID"),
                                Name = spec.Field<string>("SPECIALTY_NAME"),
                                SpecialtyType = spec.Field<int>("SPECIALTY_TYPE_ID") == 1 ? "Parent" : spec.Field<int>("SPECIALTY_TYPE_ID") == 2 ? "Child" : "Sub-Specialty",
                                ParentSpecialtyID = spec.Field<int>("SPECIALTY_TYPE_ID")
                            }).ToList();
            }
            return toReturn;
        }

        #region FUNCTION: GetCredentialList()

        public List<Credential> GetCredentialList(bool isCalledFromPH=false)
        {
            List<Credential> credentialList = new List<Credential>();
            string sql = (isCalledFromPH)? "providerhub.dbo.sp_GetCredentialList" : "providerhub.bh.sp_GetCredentialList";

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure);

            if (ds.Tables[0].Rows.Count > 0)
            {
                credentialList = (from title in ds.Tables[0].AsEnumerable()
                                  select new Credential()
                                  {
                                      ID = title.Field<int>("CREDENTIAL_ID"),
                                      Value = title.Field<string>("CREDENTIAL_VALUE"),
                                      Description = title.Field<string>("CREDENTIAL_DESCRIPTION")
                                  }).ToList();
            }

            return credentialList;
        }

        #endregion

        #region FUNCTION: GetVendorList(string searchValue)

        public List<Vendor> GetVendorList(string searchValue = "", bool isCalledFromPH = false)
        {
            List<Vendor> vendors = new List<Vendor>();

            string sql = (isCalledFromPH)? "providerHub.dbo.sp_GetVendorList" : "providerHub.bh.sp_GetVendorList";
            SqlParameter[] sqlParams = { new SqlParameter("@SEARCH_VALUE", SqlDbType.VarChar) { Value = searchValue } };
            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            vendors = (from v in ds.Tables[0].AsEnumerable()
                       select new Vendor
                       {
                           ID = v.Field<int>("VENDOR_ID"),
                           VendorName = v.Field<string>("VENDOR_NAME"),
                           NPI = v.Field<string>("VENDOR_NPI"),
                           TaxID = v.Field<string>("VENDOR_TAX_ID"),
                           EPICVendorID = v.Field<string>("VENDOR_EPIC_ID"),
                           ExternalID = v.Field<string>("VENDOR_EXTERNAL_ID"),
                           InternalNotes = v.Field<string>("INTERNAL_NOTES"),
                           CreatedDate = v.Field<DateTime>("CREATED_DATE"),
                           CreatedBy = v.Field<string>("CREATED_BY"),
                           LastUpdatedDate = v.Field<DateTime>("LAST_UPDATED_DATE"),
                           LastUpdatedBy = v.Field<string>("LAST_UPDATED_BY"),
                           City = (isCalledFromPH)? v.Field<string>("CITY") : "",
                           State = (isCalledFromPH)? v.Field<string>("STATE") : "",
                           ZipCode = (isCalledFromPH)? v.Field<string>("ZIP_CODE") : ""
                       }).ToList();

            return vendors;

        }
        #endregion

        #region FUNCTION: SaveVendor(Vendor vendor)

        public int SaveVendor(Vendor vendor)
        {
            string sql = "providerhub.bh.sp_SaveVendorData";

            DataTable dt = DataUtilityHelper.PopulateVendorTable(vendor);

            SqlParameter[] sqlParams = { new SqlParameter("@VENDOR_DATA", SqlDbType.Structured) { Value = dt } };
            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        #region FUNCTION: MapAddressToVendor(int vendorID, int addressID, string createdBy)

        public int MapAddressToVendor(int vendorID, int addressID, string createdBy)
        {
            string sql = "providerhub.bh.sp_MapAddressToVendor";

            SqlParameter[] sqlParams = {
                                            new SqlParameter("@VENDOR_ID", SqlDbType.Int) { Value = vendorID },
                                            new SqlParameter("@ADDRESS_ID", SqlDbType.Int) { Value = addressID },
                                            new SqlParameter("@CREATED_BY", SqlDbType.VarChar) { Value = createdBy }
                                        };

            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        #region FUNCTION: MapFacilityToVendor(int facilityID, int vendorID, string createdBy)

        public int MapFacilityToVendor(int facilityID, int vendorID, string createdBy)
        {
            string sql = "providerhub.bh.sp_MapFacilityToVendor";

            SqlParameter[] sqlParams = {
                                            new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID },
                                            new SqlParameter("@VENDOR_ID", SqlDbType.Int) { Value = vendorID },
                                            new SqlParameter("@CREATED_BY", SqlDbType.VarChar) { Value = createdBy }
                                        };

            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        #region FUNCTION: GetVendorByFacilityID(int facilityID)

        public Vendor GetVendorByFacilityID(int facilityID)
        {
            Vendor vendor = new Vendor();

            string sql = "providerHub.bh.sp_GetVendorByFacilityID";

            SqlParameter[] sqlParams = { new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var x = ds.Tables[0].AsEnumerable().FirstOrDefault();
                {
                    vendor.ID = x.Field<int>("VENDOR_ID");
                    vendor.VendorName = x.Field<string>("VENDOR_NAME");
                    vendor.TaxID = x.Field<string>("VENDOR_TAX_ID");
                    vendor.NPI = x.Field<string>("VENDOR_NPI");
                    vendor.EPICVendorID = x.Field<string>("VENDOR_EPIC_ID");
                    vendor.ExternalID = x.Field<string>("VENDOR_EXTERNAL_ID");
                    vendor.InternalNotes = x.Field<string>("INTERNAL_NOTES");
                    vendor.CreatedDate = x.Field<DateTime>("CREATED_DATE");
                    vendor.CreatedBy = x.Field<string>("CREATED_BY");
                    vendor.LastUpdatedDate = x.Field<DateTime>("LAST_UPDATED_DATE");
                    vendor.LastUpdatedBy = x.Field<string>("LAST_UPDATED_BY");
                    vendor.AddressesList = GetAddressByVendorID(x.Field<int>("VENDOR_ID"));
                    vendor.VendorFacilityMappingID = x.Field<int>("VENDOR_FACILITY_MAPPING_ID");
                }
            }

            return vendor;
        }

        #endregion

        #region FUNCTION: GetVendorByID(int vendorID)

        public Vendor GetVendorByID(int vendorID)
        {
            Vendor vendor = new Vendor();
            string sql = "providerHub.bh.sp_GetVendorByID";

            SqlParameter[] sqlParams = { new SqlParameter("@VENDOR_ID", SqlDbType.Int) { Value = vendorID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var x = ds.Tables[0].AsEnumerable().FirstOrDefault();
                {
                    vendor.ID = x.Field<int>("VENDOR_ID");
                    vendor.VendorName = x.Field<string>("VENDOR_NAME");
                    vendor.TaxID = x.Field<string>("VENDOR_TAX_ID");
                    vendor.NPI = x.Field<string>("VENDOR_NPI");
                    vendor.EPICVendorID = x.Field<string>("VENDOR_EPIC_ID");
                    vendor.ExternalID = x.Field<string>("VENDOR_EXTERNAL_ID");
                    vendor.InternalNotes = x.Field<string>("INTERNAL_NOTES");
                    vendor.CreatedDate = x.Field<DateTime>("CREATED_DATE");
                    vendor.CreatedBy = x.Field<string>("CREATED_BY");
                    vendor.LastUpdatedDate = x.Field<DateTime>("LAST_UPDATED_DATE");
                    vendor.LastUpdatedBy = x.Field<string>("LAST_UPDATED_BY");
                    vendor.AddressesList = GetAddressByVendorID(vendorID);
                }
            }

            return vendor;
        }

        #endregion

        #region FUNCTION: GetAddressByVendorID(int vendorID)

        public List<Address> GetAddressByVendorID(int vendorID)
        {
            List<Address> addresses = new List<Address>();
            string sql = "providerHub.bh.sp_GetAddressByVendorID";

            SqlParameter[] sqlParams = { new SqlParameter("@VENDOR_ID", SqlDbType.Int) { Value = vendorID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            addresses = (from a in ds.Tables[0].AsEnumerable()
                         select new Address
                         {
                             ID = a.Field<int>("ADDRESS_ID"),
                             AddressType = (AddressType)Enum.Parse(typeof(AddressType), a.Field<int>("ADDRESS_TYPE_ID").ToString()),
                             AddressLine1 = a.Field<string>("ADDRESS_LINE_1"),
                             AddressLine2 = a.Field<string>("ADDRESS_LINE_2"),
                             City = a.Field<string>("CITY"),
                             State = a.Field<string>("STATE"),
                             ZipCode = a.Field<string>("ZIP_CODE"),
                             County = a.Field<string>("COUNTY"),
                             Region = a.Field<string>("REGION"),
                             PhoneNumber = a.Field<string>("PHONE_NUMBER"),
                             PhoneExtension = a.Field<string>("PHONE_EXTENSION"),
                             AlternatePhoneNumber = a.Field<string>("ALTERNATE_PHONE_NUMBER"),
                             FaxNumber = a.Field<string>("FAX_NUMBER"),
                             Email = a.Field<string>("EMAIL"),
                             Website = a.Field<string>("WEBSITE"),
                             ContactFirstName = a.Field<string>("CONTACT_FIRST_NAME"),
                             ContactLastName = a.Field<string>("CONTACT_LAST_NAME"),
                             CreatedDate = a.Field<DateTime>("CREATED_DATE"),
                             CreatedBy = a.Field<string>("CREATED_BY"),
                             LastUpdatedDate = a.Field<DateTime>("LAST_UPDATED_DATE"),
                             LastUpdatedBy = a.Field<string>("LAST_UPDATED_BY")
                         }).ToList();

            return addresses;
        }

        #endregion

        #region FUNCTION: SaveLanguageByProviderID(int providerID, List<Language> languages)

        public bool SaveLanguageByProviderID(int providerID, List<Language> languages)
        {
            bool retVal = false;
            string sql = "providerhub.bh.sp_SaveProviderLanguage";

            DataTable dt = DataUtilityHelper.PopulateProviderLanguageTable(providerID, languages);

            SqlParameter[] sqlParams = { new SqlParameter("@PROVIDER_LANGUAGE", SqlDbType.Structured) { Value = dt } };

            dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams);

            retVal = true;
            return retVal;
        }

        #endregion

        #region FUNCTION: SaveCredentialByProviderID(int providerID, List<Credential> credentials)

        public bool SaveCredentialByProviderID(int providerID, List<Credential> credentials)
        {
            bool retVal = false;
            string sql = "providerhub.bh.sp_SaveProviderCredential";

            DataTable dt = DataUtilityHelper.PopulateProviderCredentialTable(providerID, credentials);

            SqlParameter[] sqlParams = {
                                            new SqlParameter("@PROVIDER_CREDENTIAL", SqlDbType.Structured) { Value = dt }
                                        };

            dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams);

            retVal = true;
            return retVal;
        }

        #endregion

        #region FUNCTION: AdvancedSearch(Dictionary<string, List<string>> args)

        public List<FacilityProviderRelationship> AdvancedSearch(Dictionary<string, List<string>> args)
        {
            List<FacilityProviderRelationship> relationshipList = new List<FacilityProviderRelationship>();
            string sql = "providerhub.bh.sp_AdvancedSearch";

            DataTable dt = DataUtilityHelper.LoadSearchFields(args);

            SqlParameter[] sqlParams = { new SqlParameter("@SEARCH_FIELDS", SqlDbType.Structured) { Value = dt } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                relationshipList = ParseFacilityProviderData(ds);
            }

            return relationshipList;
        }

        #endregion

        #region FUNCTION: GetBehavioralHealthAttributeByID(BHAttributeType bHAttributeType)

        public List<BehavioralHealthAttribute> GetBehavioralHealthAttributeByID(BHAttributeType bHAttributeType)
        {
            List<BehavioralHealthAttribute> bhAttributeList = new List<BehavioralHealthAttribute>();
            string sql = "providerhub.bh.sp_GetBehavioralHealthAttributeByID";

            SqlParameter[] sqlParams = { new SqlParameter("@BH_TYPE_ID", SqlDbType.Int) { Value = Convert.ToInt32(bHAttributeType) } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                bhAttributeList = (from bhSpecialty in ds.Tables[0].AsEnumerable()
                                   select new BehavioralHealthAttribute()
                                   {
                                       SetID = bhSpecialty.Field<int>("BH_ATTRIBUTE_SET_ID"),
                                       ValueID = bhSpecialty.Field<int>("BH_ATTRIBUTE_VALUE_ID"),
                                       BHSpecialtyType = (BHAttributeType)Enum.Parse(typeof(BHAttributeType), bhSpecialty.Field<int>("BH_ATTRIBUTE_TYPE_ID").ToString()),
                                       TextValue = bhSpecialty.Field<string>("BH_ATTRIBUTE_TEXT_VALUE")
                                   }).ToList();
            }

            return bhAttributeList;
        }

        #endregion

        #region FUNCTION: GetBHAttributeByRelationshipID(int relationshipID)

        public List<BehavioralHealthAttribute> GetBHAttributeByRelationshipID(int relationshipID)
        {
            List<BehavioralHealthAttribute> bhAttributeList = new List<BehavioralHealthAttribute>();
            string sql = "providerhub.bh.sp_GetBHAttributeByRelationshipID";

            SqlParameter[] sqlParams = { new SqlParameter("@RELATIONSHIP_ID", SqlDbType.Int) { Value = relationshipID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                bhAttributeList = (from bhSpecialty in ds.Tables[0].AsEnumerable()
                                   select new BehavioralHealthAttribute()
                                   {
                                       SetID = bhSpecialty.Field<int>("BH_ATTRIBUTE_SET_ID"),
                                       ValueID = bhSpecialty.Field<int>("BH_ATTRIBUTE_VALUE_ID"),
                                       BHSpecialtyType = (BHAttributeType)Enum.Parse(typeof(BHAttributeType), bhSpecialty.Field<int>("BH_ATTRIBUTE_TYPE_ID").ToString()),
                                       TextValue = bhSpecialty.Field<string>("BH_ATTRIBUTE_TEXT_VALUE")
                                   }).ToList();
            }

            return bhAttributeList;
        }

        #endregion

        #region FUNCTION: SaveBHAttributeToRelationship(int relationshipID, List<BehavioralHealthAttribute> bhAttributeList)

        public bool SaveBHAttributeToRelationship(int relationshipID, List<BehavioralHealthAttribute> bhAttributeList)
        {
            bool retVal = false;
            string sql = "providerhub.bh.sp_SaveBHAttributeToRelationship";

            DataTable dt = DataUtilityHelper.PopulateBHAttributeToRelationshipTable(relationshipID, bhAttributeList);

            SqlParameter[] sqlParams = {
                                            new SqlParameter("@BH_ATTRIBUTE", SqlDbType.Structured) { Value = dt }
                                        };

            dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams);

            retVal = true;

            return retVal;
        }

        #endregion

        #region FUNCTION: IsAddressMappedToFacility(int facilityID, addressID)

        public bool IsAddressMappedToFacility(int facilityID, int addressID)
        {
            string sql = "providerhub.bh.sp_IsAddressMappedToFacility";

            SqlParameter[] sqlParams = {
                                            new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID },
                                            new SqlParameter("@ADDRESS_ID", SqlDbType.Int) { Value = addressID }
                                        };

            return Convert.ToBoolean(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        #region FUNCTION: GetRelationshipDataByFacilityID(int facilityID)

        public List<FacilityProviderRelationship> GetRelationshipDataByFacilityID(int facilityID)
        {
            List<FacilityProviderRelationship> relationships = new List<FacilityProviderRelationship>();
            string sql = "providerhub.bh.sp_GetRelationshipDataByFacilityID";

            SqlParameter[] sqlParams = { new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                relationships = ParseFacilityProviderData(ds).ToList();
            }

            return relationships;
        }

        #endregion

        #region FUNCTION: GetRelationshipDataByProviderID(int providerID)

        public List<FacilityProviderRelationship> GetRelationshipDataByProviderID(int providerID)
        {
            List<FacilityProviderRelationship> relationships = new List<FacilityProviderRelationship>();
            string sql = "providerhub.bh.sp_GetRelationshipDataByProviderID";

            SqlParameter[] sqlParams = { new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = providerID } };

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

            if (ds.Tables[0].Rows.Count > 0)
            {
                relationships = ParseFacilityProviderData(ds).ToList();
            }

            return relationships;
        }

        #endregion

        #region FUNCTION: MapProviderToFacility(int providerID, int facilityID, string createdBy)

        public int MapProviderToFacility(int providerID, int facilityID, string createdBy)
        {
            string sql = "providerhub.bh.sp_MapProviderToFacility";

            SqlParameter[] sqlParams = {
                                            new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = providerID },
                                            new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID },
                                            new SqlParameter("@CREATED_BY", SqlDbType.VarChar) { Value = createdBy }
                                        };

            return Convert.ToInt32(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        /*      toReturn.id_result = DataLayer.GetIDResult(records);
                toReturn.map_result = DataLayer.GetMapResult(toReturn.id_result, records);
                toReturn.term_result = DataLayer.GetTermResult(toReturn.id_result, toReturn.map_result, records);*/
        public dynamic GetIDResult(IEnumerable<dynamic> records) {
            dynamic toReturn = new ExpandoObject();
            //I. populate base tables (used by MapResult and TermResult via idResult.baseTables
            toReturn.baseTables = new ExpandoObject(); toReturn.baseCounts = new ExpandoObject(); 
            //toReturn.provider_alias_to_create_arr = new List<dynamic>(); toReturn.provider_data_to_change_arr = new List<dynamic>();
            //EX_1 = SqlParameter[] sqlParams = { new SqlParameter("@RELATIONSHIP_ID", SqlDbType.Int) { Value = relationshipID } };
            //1. Provider table
            DataSet ds = dataLayer.ExecuteDataSet("select * from dbo.PROVIDER", CommandType.Text);//to add params: ",0,{ EX_1 }"
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.Provider = rows;
            toReturn.baseCounts.ProviderCount = rows.Count;
            //1a. Provider mapping tables
            /*    SELECT * FROM dbo.CREDENTIAL WHERE CREDENTIAL_VALUE='MD';
                  SELECT * FROM dbo.PROVIDER_LANGUAGE WHERE LANGUAGE_NAME='English';*/
            ds = dataLayer.ExecuteDataSet("select * from dbo.CREDENTIAL", CommandType.Text);//to add params: ",0,{ EX_1 }"
            rows = new List<Dictionary<string, string>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.Credential = rows;
            toReturn.baseCounts.CredentialCount = rows.Count;
            //
            ds = dataLayer.ExecuteDataSet("select * from dbo.PROVIDER_LANGUAGE", CommandType.Text);//to add params: ",0,{ EX_1 }"
            rows = new List<Dictionary<string, string>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.Language = rows;
            toReturn.baseCounts.LanguageCount = rows.Count;
            //2. Facility table
            ds = dataLayer.ExecuteDataSet("select * from dbo.FACILITY", CommandType.Text);//to add params: ",0,{ EX_1 }"
            rows = new List<Dictionary<string, string>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.Facility = rows;
            toReturn.baseCounts.FacilityCount = rows.Count;
            //fax ext + fac ext mapping
            ds = dataLayer.ExecuteDataSet("select * from dbo.FACILITY_EXTERNAL", CommandType.Text);//to add params: ",0,{ EX_1 }"
            rows = new List<Dictionary<string, string>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.FacilityExternal = rows;
            toReturn.baseCounts.FacilityCountExternal = rows.Count;
            ds = dataLayer.ExecuteDataSet("select * from dbo.FACILITY_EXTERNAL_MAPPING", CommandType.Text);//to add params: ",0,{ EX_1 }"
            rows = new List<Dictionary<string, string>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.FacilityExternalMapping = rows;
            toReturn.baseCounts.FacilityCountExternalMapping = rows.Count;
            //3. Specialty table
            ds = dataLayer.ExecuteDataSet("select * from dbo.SPECIALTY", CommandType.Text);//to add params: ",0,{ EX_1 }"
            rows = new List<Dictionary<string, string>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.Specialty = rows;
            toReturn.baseCounts.SpecialtyCount = rows.Count;
            //spec ext + spec ext mapping
            ds = dataLayer.ExecuteDataSet("select * from dbo.SPECIALTY_EXTERNAL", CommandType.Text);//to add params: ",0,{ EX_1 }"
            rows = new List<Dictionary<string, string>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.SpecialtyExternal = rows;
            toReturn.baseCounts.SpecialtyCountExternal = rows.Count;
            ds = dataLayer.ExecuteDataSet("select * from dbo.SPECIALTY_EXTERNAL_MAPPING", CommandType.Text);//to add params: ",0,{ EX_1 }"
            rows = new List<Dictionary<string, string>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.SpecialtyExternalMapping = rows;
            toReturn.baseCounts.SpecialtyCountExternalMapping = rows.Count;
            //4. FACILITY_PROVIDER_RELATIONSHIP
            ds = dataLayer.ExecuteDataSet("select * from dbo.FACILITY_PROVIDER_RELATIONSHIP", CommandType.Text);//to add params: ",0,{ EX_1 }"
            rows = new List<Dictionary<string, string>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.FacilityProvider = rows;
            toReturn.baseCounts.FacilityProviderCount = rows.Count;
            //5. PROVIDER_SPECIALTY_MAPPING, ADD FOR EVERY FAC PROVIDER SPEC THAT ISN'T A PROV SPEC
            ds = dataLayer.ExecuteDataSet("select * from dbo.PROVIDER_SPECIALTY_MAPPING", CommandType.Text);//to add params: ",0,{ EX_1 }"
            rows = new List<Dictionary<string, string>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }
            toReturn.baseTables.ProviderSpecialty = rows;
            toReturn.baseCounts.ProviderSpecialtyCount = rows.Count;
            //6. (todo) FACILITY_SPECIALTY_MAPPING, ADD FOR EVERY FAC PROVIDER SPEC THAT ISN'T A FAC SPEC
            //II. actually get ID result now
            //store blank NPI providers in DB to compare with blank NPI providers in records
            toReturn.idCounts = new ExpandoObject();
            toReturn.idCounts.ProvidersIDd = 0; toReturn.idCounts.ProvidersNotIDd = 0;
            toReturn.counts = new ExpandoObject(); toReturn.counts.name_different_for_matched_npi = 0; toReturn.counts.blankNpiCSV = 0;
            List<Dictionary<string,string>> providers_with_blank_npi_in_db = new List<Dictionary<string,string>>(); toReturn.counts.blankNpiDB = 0;
            List<dynamic> providers_with_blank_npi_in_records = new List<dynamic>(); //each dynamic record can be cast into IDictionary<string,object>...see npi_match conditional's first line of true condition below
            foreach (Dictionary<string, string> provider in toReturn.baseTables.Provider) {
                string _tmp; provider.TryGetValue("NATIONAL_PROVIDER_IDENTIFIER", out _tmp);
                if (_tmp==null || _tmp.Trim() == "") { providers_with_blank_npi_in_db.Add(provider); toReturn.counts.blankNpiDB++; }
            }
            toReturn.providers_with_blank_npi_in_db = providers_with_blank_npi_in_db;
            var uniqueNPIs =new List<string>(); toReturn.idCounts.UProvidersIDd = 0; toReturn.idCounts.UProvidersNotIDd = 0;
            /*toReturn.IDd_providers_to_update_arr = new List<dynamic>(); toReturn.provider_to_be_created_arr = new List<dynamic>();
            toReturn.IDd_facs_for_providerNPI = new List<dynamic>(); toReturn.IDd_specs_for_providerNPI = new List<dynamic>();*/
            toReturn.staging_rows = new List<dynamic>(); int _counter = 0;
            foreach (var record in records) {
                dynamic staging_row = new ExpandoObject(); staging_row.record = record; staging_row.NPI = record.NPI;
                _counter++; staging_row.unique_id = _counter; bool? add_to_staging_rows = null; dynamic _staging_row = new ExpandoObject();
                staging_row.add_to_staging_rows = null;
                //1. look for NPI match.
                bool npi_match = false; string _NPI=""; Dictionary<string, string> matchedProvider = new Dictionary<string, string>(); string _DBID = "0";
                foreach (Dictionary<string,string> provider in toReturn.baseTables.Provider) {
                    provider.TryGetValue("NATIONAL_PROVIDER_IDENTIFIER", out _NPI); provider.TryGetValue("PROVIDER_ID", out _DBID);
                    if (String.Equals(_NPI, record.NPI) && _NPI.Trim() != "")
                     { npi_match = true; matchedProvider = provider; staging_row.matched_provider = provider; break; }
                }
                staging_row.npi_match = npi_match;
                //***IF NPI MATCH: Provider is IDd, if Name is different, "alias to create" array, if other fields different "data to change" arr
                if (npi_match) {
                    //if Name is different, add to "provider_alias_to_create_arr" for storing alias, and count as name_different_for_matched_npi
                    string _fn, _ln; var dict = (IDictionary<string, object>)record; staging_row.alias_to_create = null;
                    var fn = dict["First Name"] as string; var ln = dict["Last Name"] as string;
                    matchedProvider.TryGetValue("PROVIDER_FIRST_NAME", out _fn); matchedProvider.TryGetValue("PROVIDER_LAST_NAME", out _ln);
                    if ((!uniqueNPIs.Contains(record.NPI)) && (!String.Equals(_fn, fn) || !String.Equals(_ln, ln)))
                    {
                        toReturn.counts.name_different_for_matched_npi++; dynamic toAdd = new ExpandoObject();
                        toAdd.oldFN = _fn; toAdd.oldLN = _ln; toAdd.newFN = fn; toAdd.newLN = ln; toAdd.NPI = _NPI; toAdd.DBID = _DBID;
                        staging_row.alias_to_create=toAdd;
                    }
                    //check other fields for difference, if found, add to IDd_providers_to_update_arr (NPI and changed fields/values)
                    /*  CSV VALUES: TIN # = EXTERNAL_PROVIDER_ID
                        NPI = NATIONAL_PROVIDER_IDENTIFIER (already used to match/ID, so no use checking for diff.)
                        Last Name = PROVIDER_LAST_NAME (already used for alias check)
                        First Name = PROVIDER_FIRST_NAME (already used for alias check)
                        MI = PROVIDER_MIDDLE_NAME
                        Credentials and Languages = Split by |, then try add Provider mapping like we do from selectize tag in UI, in sequence order
                        --while adding to Provider mapping, if cred/language doesn't exist yet, add it (w/o description) to base table, then add to Provider mapping.
                        Gender - Update DB to csvRecord value every time
                    */
                    //fields for dff. TIN/ExtID, Middle, Gender
                    //--csv = ["TIN #", "MI", "Gender"] | db = ["NOTHING_YET","PROVIDER_MIDDLE_NAME","PROVIDER_GENDER_ID"];
                    string[] genders = { "", "Female", "Male", "Unknown" }; dynamic providerToUpdate = new ExpandoObject();
                    providerToUpdate.additional_matched_rows = new List<dynamic>(); providerToUpdate.unique_id = _counter;
                    var tin_csv = dict["TIN #"] as string; var mi_csv = dict["MI"] as string; var gender_csv = dict["Gender"];
                    string tin_db, mi_db, gender_id_db; matchedProvider.TryGetValue("EXTERNAL_PROVIDER_ID", out tin_db);
                    matchedProvider.TryGetValue("PROVIDER_MIDDLE_NAME", out mi_db); matchedProvider.TryGetValue("PROVIDER_GENDER_ID", out gender_id_db);
                    if (!uniqueNPIs.Contains(record.NPI) && 
                       (!String.Equals(tin_db, tin_csv) || !String.Equals(mi_db, mi_csv) || !String.Equals(gender_csv, genders[Int32.Parse(gender_id_db)]))) { 
                        providerToUpdate.tin_csv = tin_csv; providerToUpdate.mi_csv = mi_csv; providerToUpdate.gender_csv = gender_csv;
                        providerToUpdate.tin_db = tin_db; providerToUpdate.mi_db = mi_db; providerToUpdate.gender_db = genders[Int32.Parse(gender_id_db)];
                    }
                    //Credentials and Languages split by | then try add Provider mapping, just like from selectize tag. Use SEQUENCE_NUMBER by order in fields
                    //--drop all current, and re-add, again, just like when updating these in UI via slectize
                    //--add new now, again...like UI via selectize does
                    var creds = dict["Credentials"]; var langs = dict["Languages"]; string[] credsArr = creds.ToString().Split('|');
                    string[] langsArr = langs.ToString().Split('|'); providerToUpdate.NPI = record.NPI;
                    providerToUpdate.Languages = new List<string>(); providerToUpdate.Credentials = new List<String>();
                    foreach (string cred in credsArr) {
                        providerToUpdate.Credentials.Add(cred);//ID, add if not exist, add mapping
                    }
                    foreach (string lang in langsArr) {
                        providerToUpdate.Languages.Add(lang);//ID, add if not exist, add mapping
                    }
                    /* Additional Fields to use
                        Primary Specialty - check against ProvSpecs and set sequence_number 0, bump rest up
                        Primary Practice Location - set this ProvFac to sequence_number 0, bump rest up
                        Start Date - set all Provider mappings' Effective Date to this value, if defined (Prov-Fac/Spec/Language/Cred, Prov itself?)
                        PC Panel Status - Map to Accepting New Patients in ProvFac if trivial.
                        Hospital - map to hospital affiliations table (hospital<==>provider), and add new hospital if not in hospital table
                       Not Confirmed on what to do / Don't currently use
                        Fam Med w/OB - not using atm, but could just check to verify provider has FamMed and OB in specs
                        Status - not using atm
                        Directories/Web - do we do anything with this? not at the moment, we might though
                        Primary Care - is this stored on our end anywhere
                        Board Certifications - do we use anywhere? not yet
                    */
                    providerToUpdate.PrimarySpecialty = dict["Primary Specialty"]; providerToUpdate.PrimaryPracticeLocation = dict["Primary Practice Location"];
                    providerToUpdate.StartDate = dict["Start Date"]; providerToUpdate.PCPanelStatus = dict["PC Panel Status"];
                    providerToUpdate.HospitalAffiliation = dict["Hospitals"]; providerToUpdate.Specialty = dict["Specialty"];
                    //mark Provider as IDd
                    toReturn.idCounts.ProvidersIDd++;
                    if (!uniqueNPIs.Contains(record.NPI)) { //if a new, unique NPI, add provider to array
                        toReturn.idCounts.UProvidersIDd++; uniqueNPIs.Add(record.NPI); add_to_staging_rows = true; staging_row.add_to_staging_rows = true;
                        staging_row.provider_row = providerToUpdate; staging_row.provider_action = "update";
                    } else { //else append object to already identified provider (object with same NPI)
                        _staging_row = (toReturn.staging_rows as List<dynamic>).Where(p => p.NPI == record.NPI).First();
                        _staging_row.provider_row.additional_matched_rows.Add(providerToUpdate); add_to_staging_rows = false; staging_row.add_to_staging_rows = false;
                    }
                }
                //***IF NO NPI MATCH: Provider "to_be_created" w/ "IDd PID"=0, also "possible_provider_match" array with F/L and listed P_SpecFac matching
                else {
                    //mark Provider as Not IDd
                    toReturn.idCounts.ProvidersNotIDd++; 
                    //if blank NPI, handle accordingly -- try to match provider, may be less likely to create_new
                    if (record.NPI.Trim() == "")
                    {
                        toReturn.counts.blankNpiCSV++;
                        providers_with_blank_npi_in_records.Add(record);
                    }
                    else
                    {
                        //generate provider_likely_match_arr with likelihood_of_match, record, and likely_match (from Provider base table - use NPI and DBID) 
                        //--Likelihood_of_match field = 1 if F/L and SpecFac match, 2 if one doesnt match, 3 if two dont, >2 dont = dont even consider...
                        //LIKELY MATCH CODE_HERE

                        //curate fields for new insert --basically fields specific to each potential additional_matched_rows entry
                        dynamic providerToCreate = new ExpandoObject(); providerToCreate.NPI = record.NPI; providerToCreate.unique_id = _counter;
                        providerToCreate.additional_matched_rows = new List<dynamic>(); var ptcDict = (IDictionary<string, object>)record;
                        providerToCreate.PrimarySpecialty = ptcDict["Primary Specialty"]; providerToCreate.PrimaryPracticeLocation = ptcDict["Primary Practice Location"];
                        providerToCreate.StartDate = ptcDict["Start Date"]; providerToCreate.PCPanelStatus = ptcDict["PC Panel Status"];
                        providerToCreate.Specialty = ptcDict["Specialty"]; providerToCreate.HospitalAffiliation = ptcDict["Hospitals"];
                        //
                        var creds = ptcDict["Credentials"]; var langs = ptcDict["Languages"]; string[] credsArr = creds.ToString().Split('|');
                        string[] langsArr = langs.ToString().Split('|');
                        providerToCreate.Languages = new List<string>(); providerToCreate.Credentials = new List<String>();
                        foreach (string cred in credsArr)
                        {
                            providerToCreate.Credentials.Add(cred);//ID, add if not exist, add mapping
                        }
                        foreach (string lang in langsArr)
                        {
                            providerToCreate.Languages.Add(lang);//ID, add if not exist, add mapping
                        }
                        //

                        if (!uniqueNPIs.Contains(record.NPI))
                        { //if a new, unique NPI, add provider to array
                            toReturn.idCounts.UProvidersNotIDd++; uniqueNPIs.Add(record.NPI); add_to_staging_rows = true; staging_row.add_to_staging_rows = true;
                            staging_row.provider_row = providerToCreate; staging_row.provider_action = "create";
                        }
                        else
                        { //else append object to already identified provider (object with same NPI)
                            _staging_row = (toReturn.staging_rows as List<dynamic>).Where(p => p.NPI == record.NPI).First();
                            _staging_row.provider_row.additional_matched_rows.Add(providerToCreate); add_to_staging_rows = false; staging_row.add_to_staging_rows = false;
                        }
                    }
                }
                /*FacSpec stuff
                    Specialty = Lookup in DB by name, try add active providerSpec mapping if not exist (if exist, and active, don't add) (if exist, and not active, add new active)
                    --lookup Ext mapping as well if not found directly in Spec, mark for add (*to spec and/or map?) if not found in either
                    ClinicName,Address,City,State,Zip,Phone = Try IDing Fac by ClinicName/ExtNames + Address + City + State + Zip, if full match found mark as IDd with FacID
                    --if not found mark Facility(ClinicName), Address(Address,City,State,Zip,Phone), and FacilityAddressMap(FacIDPlaceholder,AddrIDPlaceholder) for creation
                    ----also add FacVendor (whatever vendor the file is, in this case UW)
                    --(name match, diff. address = Fac IDd, but mark address for update) **ASK if there could be same name'd clinic + diff addr in a given vendor (wont happen w UW but stilll)
                    --(address match, diff. name = new fac at same address as another, not IDd, mark for create like above UNLESS ALIAS**so ask about alias case??)
                */
                //IDd_facs_for_providerNPI | IDd_specs_for_providerNPI -- both Dictionary<string,string>
                //FacilityExternal, FacilityExternalMapping : SpecialtyExternal, SpecialtyExternalMapping -- base table names
                //2. Look for Fac match: "to create" if not exist, flag to map "new fac id #xxx" to provider (whether prov is new or existing)
                dynamic IDd_fac = new ExpandoObject(); string fac_name = ""; var dict2 = (IDictionary<string, object>)record; var fac_match = false;
                var matchedFac = new Dictionary<string, string>();  var fac_match_type = "no match direct or indirect"; string feid, _feid;
                Dictionary<string,Dictionary<string,string>> FacByID = new Dictionary<string, Dictionary<string,string>>(); string fac_id = "";
                foreach (Dictionary<string, string> facility in toReturn.baseTables.Facility) {
                    facility.TryGetValue("FACILITY_ID", out fac_id); FacByID.Add(fac_id, facility);
                }
                foreach (Dictionary<string, string> facilityExt in toReturn.baseTables.FacilityExternal)
                {
                    facilityExt.TryGetValue("FACILITY_EXTERNAL_NAME", out fac_name);
                    if (String.Equals(fac_name, dict2["ClinicName"]) && fac_name.Trim() != "")
                    {
                        fac_match = true; fac_match_type = "indirect from Facility External table's FacilityName";
                        foreach (Dictionary<string, string> facilityExtMap in toReturn.baseTables.FacilityExternalMapping)
                        {
                            facilityExt.TryGetValue("FACILITY_EXTERNAL_ID", out feid); facilityExtMap.TryGetValue("FACILITY_EXTERNAL_ID", out _feid);
                            facilityExtMap.TryGetValue("FACILITY_ID", out fac_id);
                            if (String.Equals(feid, _feid)) { FacByID.TryGetValue(fac_id, out matchedFac); }
                        }
                        break;
                    }
                }
                if (!fac_match) {
                    foreach (Dictionary<string, string> facility in toReturn.baseTables.Facility)
                    {
                        facility.TryGetValue("FACILITY_NAME", out fac_name); facility.TryGetValue("FACILITY_ID", out fac_id);
                        if (String.Equals(fac_name, dict2["ClinicName"]) && fac_name.Trim() != "")
                        { fac_match = true; matchedFac = facility; fac_match_type = "direct from Facility table's FacilityName"; break; }
                    }
                }
                IDd_fac.fac_match = fac_match; IDd_fac.fac_match_type = fac_match_type; IDd_fac.matchedFac = matchedFac;
                //IDd_fac.linked_csv_record = record; IDd_fac.linked_csv_record_npi = record.NPI;
                //WE check explicily because we use "add_to_staging_rows == null" as a third case for rows that shouldn't be processed (like blank NPI rows)
                if (add_to_staging_rows==true) //again, since it's "bool?" (not just bool) and we use the bool?==null case we need ==true/false
                { //staging_row
                    staging_row.IDd_fac = IDd_fac;
                } else if(add_to_staging_rows==false) { //_staging_row
                    dynamic subRow = (_staging_row.provider_row.additional_matched_rows as List<dynamic>).Last();
                    subRow.IDd_fac = IDd_fac;
                }
                //if fac not IDd, mark to create (as non-external by default?) then use new Fac's ID for provider and add to base_table for subsequent providers

                //3. Look for Spec match: "to create" if not exist, flag to map "new spec id #xxx" to provider (whether new or existing)
                dynamic IDd_spec = new ExpandoObject(); string spec_name = ""; var dict3 = (IDictionary<string, object>)record; var spec_match = false;
                var matchedSpec = new Dictionary<string, string>(); var spec_match_type = "no match direct or indirect"; string sid, _sid;
                Dictionary<string, Dictionary<string, string>> SpecByID = new Dictionary<string, Dictionary<string, string>>(); string spec_id = "";
                foreach (Dictionary<string, string> specialty in toReturn.baseTables.Specialty) {
                    specialty.TryGetValue("SPECIALTY_ID", out spec_id); SpecByID.Add(spec_id, specialty);
                }
                foreach (Dictionary<string, string> specialtyExt in toReturn.baseTables.SpecialtyExternal)
                {
                    specialtyExt.TryGetValue("SPECIALTY_EXTERNAL_NAME", out spec_name);
                    if (String.Equals(spec_name, dict3["Specialty"]) && spec_name.Trim() != "")
                    {
                        spec_match = true; spec_match_type = "indirect from Specialty External table's SpecName";
                        foreach (Dictionary<string, string> specialtyExtMap in toReturn.baseTables.SpecialtyExternalMapping)
                        {
                            specialtyExt.TryGetValue("SPECIALTY_EXTERNAL_ID", out sid); specialtyExtMap.TryGetValue("SPECIALTY_EXTERNAL_ID", out _sid);
                            specialtyExtMap.TryGetValue("SPECIALTY_ID", out spec_id);
                            if (String.Equals(sid, _sid)) { SpecByID.TryGetValue(spec_id, out matchedSpec); }
                        }
                        break;
                    }
                }
                if (!spec_match)
                {
                    foreach (Dictionary<string, string> specialty in toReturn.baseTables.Specialty)
                    {
                        specialty.TryGetValue("SPECIALTY_NAME", out spec_name); specialty.TryGetValue("SPECIALTY_ID", out spec_id);
                        if (String.Equals(spec_name, dict3["Specialty"]) && spec_name.Trim() != "")
                        { spec_match = true; matchedSpec = specialty; spec_match_type = "direct from Spec table's SpecName"; break; }
                    }
                }
                IDd_spec.spec_match = spec_match; IDd_spec.spec_match_type = spec_match_type; IDd_spec.matchedSpec = matchedSpec;
                //IDd_spec.linked_csv_record = record; IDd_spec.linked_csv_record_npi = record.NPI;
                //WE check explicily because we use "add_to_staging_rows == null" as a third case for rows that shouldn't be processed (like blank NPI rows)
                if (add_to_staging_rows==true) //again, since it's "bool?" (not just bool) and we use the bool?==null case we need ==true/false
                { //staging_row
                    staging_row.IDd_spec = IDd_spec;
                }
                else if (add_to_staging_rows==false)
                { //_staging_row
                    dynamic subRow = (_staging_row.provider_row.additional_matched_rows as List<dynamic>).Last();
                    subRow.IDd_spec = IDd_spec;
                }
                //if spec not IDd, mark to create (as non-external by default?) then use new Spec's ID for provider and add to base_table for subsequent providers

                //M1. (definitely best to do here) try matching non-NPI id'd providers
                //M2. (maybe not here, but sometime before GetTermResult) store "in db and not in file"...just add to term lists

                //if new staging_row modified with this record (i.e. "is a parent row" and not a sub-row/'additional row') add to list
                if (add_to_staging_rows == true) { toReturn.staging_rows.Add(staging_row); }
            }
            //
            toReturn.providers_with_blank_npi_in_records = providers_with_blank_npi_in_records;
            //FIN. QED. 
            return toReturn;
        }
        public dynamic GetMapResult(dynamic idResult, IEnumerable<dynamic> records) {
            dynamic toReturn = new ExpandoObject(); toReturn.db_transaction = ""; int seed_id = 457; int fpr_seed_id = 4487;
            foreach (var staging_row in idResult.staging_rows)
            {
                List<string> alreadyUpdatedFacs = new List<string>(); List<string> alreadyUpdatedSpecs = new List<string>();
                toReturn.validFacProvSpecs = new List<dynamic>();//--for TermResult, valid for Vendor/Facilities of course
                //***NOW WE HAVE IDd PROV FAC SPEC in vars x y z, respectively (or 0 if not IDd***\\
                //3a. If any not IDd + 'toCreate', insert new (provider, fac, or spec), If IDd + 'toUpdate', update changed info
                //PROVIDER 1. INSERT/UPDATE PROVIDER ROW
                var valid_match = false; string _pid = "0"; string _set = ""; string _query = ""; var _dict = (IDictionary<string, object>)staging_row.record;
                string _gender="",_effdate = "";
                if (staging_row.npi_match)
                {
                    //generate UPDATE sql for parent row of this provider (i.e. not an 'additional row')
                    //UPDATE EXISTING RECORD for 'parent row' and store PROVIDER_ID as PROV_ID / _DBID
                    // get PROVIDER_ID from staging_row.matched_provider, which should always be populated for npi_match==true
                    /*--"mi_csv": "L.","gender_csv": "Female","tin_db": "","mi_db": "","gender_db": "Female",*/
                        if (staging_row.provider_row.mi_csv != staging_row.provider_row.mi_db) { _set += "PROVIDER_MIDDLE_NAME='"+staging_row.provider_row.mi_csv+"'"; }
                        if (staging_row.provider_row.gender_csv != staging_row.provider_row.gender_db) {
                            _gender = (staging_row.provider_row.gender_csv == "Female") ? "1" : (staging_row.provider_row.gender_csv == "Male") ? "2" : "3";
                            //_set += (_set!="")? "," : "" + "PROVIDER_GENDER_ID='" + _gender + "'";
                        }
                        if (((IDictionary<string,string>)staging_row.matched_provider)["EFFECTIVE_DATE"] as string != _dict["Start Date"] as string)
                        {
                            _effdate = _dict["Start Date"] as string;
                            //_set += (_set != "") ? "," : "" + "EFFECTIVE_DATE='" + _effdate + "'";
                        }
                        _pid = ((IDictionary<string, string>)staging_row.matched_provider)["PROVIDER_ID"];
                        //if (_set != "") { _query = "UPDATE dbo.PROVIDER SET " + _set + " WHERE PROVIDER_ID=" + _pid; }
                        string sql = "providerhub.dbo.sp_UWImport_UpdateExisting";
                        SqlParameter[] sqlParams = {
                            new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = _pid }, new SqlParameter("@PROVIDER_GENDER_ID", SqlDbType.Int) { Value = _gender },
                            new SqlParameter("@EFFECTIVE_DATE", SqlDbType.DateTime) { Value = _effdate }
                        };
                        DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);//TODO: check result
                    //debug only
                        //row check level: staging_row.queryOne = _query;
                        //manualdb level:
                        //toReturn.db_transaction += _query + ";";
                    //production
                    //(run update query)
                    //

                    //NOTE: we have both staging_row and _staging_row.provider_row.additional_matched_rows[x] to imply 3, 4 and 5 from...
                    //      as well as staging_row.record or _staging_row.record to base additional changes from...(if needed, prob not)
                    valid_match = true;
                }
                else if (!staging_row.npi_match && staging_row.NPI.Trim() != "")
                { //include !npi_match for clarity only; not needed
                    //generate INSERT sql for parent row of provider (like above, i.e. not an 'additional row')
                        //INSERT NEW RECORD for 'parent row' and store LAST_INSERT_ID as PROV_ID / _DBID, _query, _pid
                        var _fields = "NATIONAL_PROVIDER_IDENTIFIER,PROVIDER_FIRST_NAME,PROVIDER_MIDDLE_NAME,PROVIDER_LAST_NAME,PROVIDER_GENDER_ID,CSP_INDICATOR,EFFECTIVE_DATE";
                        _gender = (_dict["Gender"] as string == "Female") ? "1" : (_dict["Gender"] as string == "Male") ? "2" : "3";
                        var _values = "'"+_dict["NPI"] as string+"','"+ _dict["First Name"] as string + "','"+_dict["MI"] as string+"','"+_dict["Last Name"] as string+"',"+_gender+",0,'"+_dict["Start Date"] as string+"'";
                        //_query = "INSERT INTO dbo.PROVIDER("+_fields+") VALUES("+_values+")";
                        _pid = seed_id.ToString();
                        string sql = "providerhub.dbo.sp_UWImport_CreateNew";
                        SqlParameter[] sqlParams = {
                                new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = _pid }, new SqlParameter("@PROVIDER_GENDER_ID", SqlDbType.Int) { Value = _dict["Gender"] },
                                new SqlParameter("@PROVIDER_FIRST_NAME", SqlDbType.VarChar) { Value = _dict["First Name"] }, new SqlParameter("@NPI", SqlDbType.VarChar) { Value = _dict["NPI"] },
                                new SqlParameter("@PROVIDER_LAST_NAME", SqlDbType.VarChar) { Value = _dict["Last Name"] },new SqlParameter("@PROVIDER_MIDDLE_NAME", SqlDbType.VarChar) { Value = _gender },
                                new SqlParameter("@CSP_INDICATOR", SqlDbType.Int) { Value =0 }, new SqlParameter("@EFFECTIVE_DATE", SqlDbType.VarChar) { Value = _dict["Start Date"] }
                        };
                        DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);
                    //toReturn.db_transaction += _query + ";";
                    // get LAST_INSERT_ID after insert statement or as part of result from insert statement, or just from DBConnection object

                    //NOTE: we have both staging_row and _staging_row.provider_row.additional_matched_rows[x] to imply 3, 4 and 5 from...
                    //      as well as staging_row.record or _staging_row.record to base additional changes from...(if needed, prob not)
                    valid_match = true; seed_id++;
                }

                if (valid_match)
                {
                    //PROVIDER 2. Create alias if needed / set in object (staging_row.alias_to_create!=null)
                    //--toAdd.oldFN = _fn; toAdd.oldLN = _ln; toAdd.newFN = fn; toAdd.newLN = ln; toAdd.NPI = _NPI; toAdd.DBID = _DBID;

                    //PROVIDER 3. (Drop & Re)Create creds/languages mappings (and any other "based on prov data only" mappings), from parent row + PROV_ID / _DBID
                    /*    SELECT * FROM dbo.CREDENTIAL WHERE CREDENTIAL_VALUE='MD'; ds = dataLayer.ExecuteDataSet("select * from dbo.PROVIDER_LANGUAGE", CommandType.Text);
                          SELECT * FROM dbo.PROVIDER_LANGUAGE WHERE LANGUAGE_NAME='English';*/
                    string _tmp; string _tmpid; string _lid="0"; string _cid="0"; int _seq = 0;
                    var _drop = "DELETE FROM dbo.PROVIDER_LANGUAGE_MAPPING WHERE PROVIDER_ID=" + _pid + ";DELETE FROM dbo.PROVIDER_CREDENTIAL_MAPPING WHERE PROVIDER_ID=" + _pid + ";";
                    toReturn.db_transaction += _drop;
                    foreach (string langName in staging_row.provider_row.Languages) { 
                        foreach (Dictionary<string, string> language in idResult.baseTables.Language)
                        {
                            language.TryGetValue("LANGUAGE_NAME", out _tmp); language.TryGetValue("LANGUAGE_ID", out _tmpid);
                            if (String.Equals(_tmp, langName))
                            { _lid = _tmpid; break; }
                        }
                        string _fields2 = "PROVIDER_ID,LANGUAGE_ID,SEQUENCE_NUMBER"; string _values2 = _pid+","+_lid+","+_seq.ToString();
                        var _reinsert = "INSERT INTO dbo.PROVIDER_LANGUAGE_MAPPING(" + _fields2 + ") VALUES (" + _values2 + ");";
                        toReturn.db_transaction += _reinsert;
                        _seq++;
                    }
                    _seq = 0;
                    foreach (string credName in staging_row.provider_row.Credentials) {
                        foreach (Dictionary<string, string> credential in idResult.baseTables.Credential) {
                            credential.TryGetValue("CREDENTIAL_VALUE", out _tmp); credential.TryGetValue("CREDENTIAL_ID", out _tmpid);
                            if (String.Equals(_tmp, credName))
                            { _cid = _tmpid; break; }
                        }
                        string _fields2 = "PROVIDER_ID,CREDENTIAL_ID,SEQUENCE_NUMBER"; string _values2 = _pid + "," + _cid + "," + _seq.ToString();
                        var _reinsert = "INSERT INTO dbo.PROVIDER_LANGUAGE_MAPPING(" + _fields2 + ") VALUES (" + _values2 + ");";
                        toReturn.db_transaction += _reinsert;
                        _seq++;
                    }
                    //PROVIDER 4. add FacProvSpec IDs to validFacProvSpec so they aren't mistakenly marked for removal in Terms

                    //PROVIDER 5-6. Update IDd_fac and IDd_spec info if different than DB record for matched Fac/Spec/TableLinkedToFacOrSpec, and not in alreadyUpdatedFacs/Specs

                    //7. FacProv + Employment (FP+Spec) toMap, create Active FacSpec/ProvSpec if not exist 
                    //   for ProvSpec, (what seq #? by default add to end/last seq #...but if set as "Primary Specialty" then move to 1 and push all others down by 1)
                    //   USE prov_id/_DBID, IDd_fac.matchedFac.FACILITY_ID, IDd_spec.matchedSpec.SPECIALTY_ID
                    // _pid | staging_row.IDd_fac.matchedFac.FACILITY_ID | staging_row.IDd_spec.matchedSpec.SPECIALTY_ID
                    var __values = _pid+","+ ((IDictionary<string, string>)staging_row.IDd_fac.matchedFac)["FACILITY_ID"] + ",'"+ _dict["Start Date"] as string+"'";
                    string _fpr = "INSERT INTO dbo.FACILITY_PROVIDER_RELATIONSHIP(FACILITY_ID,PROVIDER_ID,EFFECTIVE_DATE) VALUES("+__values+")";
                    toReturn.db_transaction += _fpr + ";";
                    __values= fpr_seed_id.ToString()+","+ ((IDictionary<string, string>)staging_row.IDd_spec.matchedSpec)["SPECIALTY_ID"] + ",'"+_dict["Start Date"] as string+"'";
                    string _e = "INSERT INTO dbo.EMPLOYMENT(FACILITY_PROVIDER_RELATIONSHIP_ID,SPECIALTY_ID,EFFECTIVE_DATE) VALUES("+__values+");";
                    toReturn.db_transaction += _e; fpr_seed_id++;
                    /*try
                    {
                        foreach (dynamic additional_row in staging_row.additional_matched_rows)
                        {
                            //_PID | additional_row.IDd_fac_matchedFac.FACILITY_ID | additional_row.IDd_spec.matchedSpec.SPECIALTY_ID
                            var ___values = _pid + "," + ((IDictionary<string, string>)additional_row.IDd_fac.matchedFac)["FACILITY_ID"] + "," + _dict["Start Date"] as string;
                            string __fpr = "INSERT INTO dbo.FACILITY_PROVIDER_RELATIONSHIP(FACILITY_ID,PROVIDER_ID,EFFECTIVE_DATE) VALUES(" + ___values + ")";
                            toReturn.db_transaction += __fpr + ";";
                            ___values = fpr_seed_id.ToString() + "," + ((IDictionary<string, string>)staging_row.IDd_spec.matchedSpec)["SPECIALTY_ID"] + "," + _dict["Start Date"] as string;
                            string __e = "INSERT INTO dbo.EMPLOYMENT(FACILITY_PROVIDER_RELATIONSHIP_ID,SPECIALTY_ID,EFFECTIVE_DATE) VALUES(" + ___values + ");";
                            toReturn.db_transaction += __e; fpr_seed_id++;
                        }
                    }
                    catch (Exception e) { }*/
                }
            }
            toReturn.staging_rows = idResult.staging_rows;
            return toReturn;
        }
        public dynamic GetTermResult(dynamic mapResult, dynamic idResult, IEnumerable<dynamic> records) {
            dynamic toReturn = new ExpandoObject();
            foreach (var staging_row in mapResult.staging_rows)
            {
                //8. FacProv to Term (term Employment for removed FacProv ID, and Directory for removed Employment ID to keep DB clean)
                //  FOR every row's IDd_fac:
                //   FIND all Employment for Provider, if Fac/Prov/Spec exist in DB and not added in 4 / in validFacProvSpecs, remove
                //   FIND all FacProvRelationship for Provider, if Fac/Prov exist in DB and not added in 4 / in validFacProvSpecs, remove

            }
            return toReturn;
        }

        #region FUNCTION: IsProviderAndFacilityMapped(int providerID, facilityID)

        public bool IsProviderAndFacilityMapped(int providerID, int facilityID)
        {
            string sql = "providerhub.bh.sp_IsProviderAndFacilityMapped";

            SqlParameter[] sqlParams = {
                                            new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = providerID },
                                            new SqlParameter("@FACILITY_ID", SqlDbType.Int) { Value = facilityID }
                                        };

            return Convert.ToBoolean(dataLayer.ExecuteScalar(sql, CommandType.StoredProcedure, 0, sqlParams));
        }

        #endregion

        //#region FUNCTION: GetProviderSchedule(int providerID)

        //public List<ProviderAgencyAvailability> GetProviderSchedule(int providerID)
        //{
        //    List<ProviderAgencyAvailability> AvailabilityList = new List<ProviderAgencyAvailability>();
        //    string sql = "providerhub.dbo.sp_GetProviderSchedule";

        //    SqlParameter[] sqlParams =
        //    {
        //        new SqlParameter("@PROVIDER_ID", SqlDbType.Int) { Value = providerID }
        //    };

        //    DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure, 0, sqlParams);

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        AvailabilityList = (from schedule in ds.Tables[0].AsEnumerable()
        //                            select new ProviderAgencyAvailability()
        //                            {
        //                                AvailabilityID = schedule.Field<int>("AVAILABILITY_ID"),
        //                                DayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), schedule.Field<int>("DAY_OF_WEEK_ID").ToString()),
        //                                StartTimeOfDay = (TimeOfDay)Enum.Parse(typeof(TimeOfDay), schedule.Field<int>("START_TIME_OF_DAY_ID").ToString()),
        //                                EndTimeOfDay = (TimeOfDay)Enum.Parse(typeof(TimeOfDay), schedule.Field<int>("END_TIME_OF_DAY_ID").ToString()),
        //                                InternalNote = schedule.Field<string>("INTERNAL_NOTE"),
        //                                CreatedDate = schedule.Field<DateTime>("CREATED_DATE"),
        //                                CreatedBy = schedule.Field<string>("CREATED_BY")
        //                            }).ToList();
        //    }

        //    return AvailabilityList;
        //}

        //#endregion

        #region DISPOSE FUNCTIONALITY

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                dataLayer = null;
            }
        }

        #endregion

    }
}