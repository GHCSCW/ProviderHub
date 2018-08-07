using System;
using System.Data;
using System.Linq;
using System.Web;
using Ghc.Utility.DataAccess;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Collections.Generic;

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
                    provider.MedicareIndicator = x.Field<bool>("MEDICARE_PROVIDER_INDICATOR");
                    provider.MedicarePTAN = x.Field<string>("MEDICARE_PTAN");
                    provider.MedicareEffectiveDate = Convert.ToString(x.Field<DateTime?>("MEDICARE_EFFECTIVE_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("MEDICARE_EFFECTIVE_DATE"));
                    provider.MedicareTerminationDate = Convert.ToString(x.Field<DateTime?>("MEDICARE_TERMINATION_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("MEDICARE_TERMINATION_DATE"));
                    provider.MedicaidIndicator = (bool)x.Field<bool>("MEDICAID_PROVIDER_INDICATOR");
                    provider.MedicaidProviderID = x.Field<string>("MEDICAID_PROVIDER_ID");
                    provider.EffectiveDate = Convert.ToString(x.Field<DateTime?>("EFFECTIVE_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("EFFECTIVE_DATE"));
                    provider.TerminationDate = Convert.ToString(x.Field<DateTime?>("TERMINATION_DATE")).Length == 0 ? (DateTime?)null : Convert.ToDateTime(x.Field<DateTime>("TERMINATION_DATE"));
                    provider.InternalNotes = x.Field<string>("INTERNAL_NOTES");
                    provider.CreatedDate = x.Field<DateTime>("CREATED_DATE");
                    provider.CreatedBy = x.Field<string>("CREATED_BY");
                    provider.LastUpdatedDate = x.Field<DateTime>("LAST_UPDATED_DATE");
                    provider.LastUpdatedBy = x.Field<string>("LAST_UPDATED_BY");
                    provider.LanguageList = GetProviderLanguageByID(providerID, true);
                    provider.CredentialList = GetProviderCredentialByID(providerID);
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
                                        FaxNumber = x.Field<string>("FAX_NUMBER"),
                                        LastUpdatedBy = x.Field<string>("ADDRESS_LAST_UPDATED_BY"),
                                        LastUpdatedDate = x.Field<DateTime>("ADDRESS_LAST_UPDATED_DATE")
                                    },
                                    FPRelationship = new FacilityProviderRelationship {
                                        ExternalProviderIndicator = x.Field<bool?>("EXTERNAL_PROVIDER_INDICATOR"),
                                        AcceptingNewPatientIndicator = x.Field<bool?>("ACCEPTING_NEW_PATIENT_INDICATOR"),
                                        PrescriberIndicator = x.Field<bool?>("PRESCRIBER_INDICATOR"),
                                        ReferralIndicator = x.Field<bool?>("REFERRALL_INDICATOR"),
                                        PCPEligibleIndicator = x.Field<bool?>("PCP_ELIGIBLE_INDICATOR"),
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

        public List<Language> GetLanguageList()
        {
            List<Language> languageList = new List<Language>();
            string sql = "providerhub.bh.sp_GetLanguageList";

            DataSet ds = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure);

            if (ds.Tables[0].Rows.Count > 0)
            {
                languageList = (from language in ds.Tables[0].AsEnumerable()
                                select new Language()
                                {
                                    ID = language.Field<int>("PROVIDER_LANGUAGE_ID"),
                                    Name = language.Field<string>("PROVIDER_LANGUAGE_NAME")
                                }).ToList();
            }

            return languageList;
        }

        #endregion

        #region FUNCTION: GetCredentialList()

        public List<Credential> GetCredentialList()
        {
            List<Credential> credentialList = new List<Credential>();
            string sql = "providerhub.bh.sp_GetCredentialList";

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

        public List<Vendor> GetVendorList(string searchValue)
        {
            List<Vendor> vendors = new List<Vendor>();

            string sql = "providerHub.bh.sp_GetVendorList";
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
                           LastUpdatedBy = v.Field<string>("LAST_UPDATED_BY")
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