using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;
using System.Text;

namespace ProviderHubService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class ProviderHubService : IProviderHubService
    {
        #region FUNCTION: GetData(int value)

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        #endregion

        #region FUNCTION: GetProviderByID(int providerID)
        /// <summary>
        /// This function returns the Provider Object based on the database ProviderID value (PROVIDER_ID). The Provider object contains Provider Details data.
        /// </summary>
        /// <param name="providerID"></param>
        /// <returns></returns>
        public Provider GetProviderByID(int providerID)
        {
            Provider provider = new Provider();
            using (DataLayer dataLayer = new DataLayer())
            {
                provider = dataLayer.GetProviderByID(providerID);
            }

            return provider;
        }

        #endregion

        #region FUNCTION: GetProviderLanguageByID(int providerID)
        /// <summary>
        /// This function returns a list of Language object based on the database ProviderID value (PROVIDER_ID). A provider can speak multiple languages, there is a sequence number to identify what the primary language is.
        /// </summary>
        /// <param name="providerID"></param>
        /// <returns></returns>
        public List<Language> GetProviderLanguageByID(int providerID)
        {
            List<Language> languages = new List<Language>();

            using (DataLayer dataLayer = new DataLayer())
            {
                languages = dataLayer.GetProviderLanguageByID(providerID);
            }
            return languages;
        }

        #endregion

        #region FUNCTION: GetProviderCredentialByID(int providerID)
        /// <summary>
        /// This function returns a list of the Credential object based on the ProviderID value (PROVIDER_ID). 
        /// A provider can have multiple Credentials, there is a sequence number property 
        /// on the Language object to set the order of this for a provider.
        /// </summary>
        /// <param name="providerID"></param>
        /// <returns></returns>
        public List<Credential> GetProviderCredentialByID(int providerID)
        {
            List<Credential> credentials = new List<Credential>();

            using (DataLayer dataLayer = new DataLayer())
            {
                credentials = dataLayer.GetProviderCredentialByID(providerID);
            }

            return credentials;
        }

        #endregion

        #region FUNCTION: GetFacilityByID(int facilityID)
        /// <summary>
        /// This function returns a Facility object based on the database Facility ID value (FACILITY_ID).
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        public Facility GetFacilityByID(int facilityID)
        {
            Facility facility = new Facility();

            using (DataLayer dataLayer = new DataLayer())
            {
                facility = dataLayer.GetFacilityByID(facilityID);
            }

            return facility;
        }

        #endregion

        #region FUNCTION: GetProviderList(string searchValue)
        /// <summary>
        /// This function returns a list of Providers available in the system, the searchValue parameter can be used to look up a specific provider by name.
        /// To get the full Provider list, send an empty value in the parameter.
        /// </summary>
        /// <returns></returns>
        public List<Provider> GetProviderList(string searchValue)
        {
            List<Provider> providers = new List<Provider>();

            using (DataLayer dataLayer = new DataLayer())
            {
                providers = dataLayer.GetProviderList(searchValue);
            }
            return providers;
        }

        #endregion

        #region FUNCTION: GetFacilityList(string searchValue)
        /// <summary>
        /// This function returns a list of Providers available in the system, the searchValue parameter can be used to look up an specific provider by name.
        /// To get the full Provider list, send an empty value in the parameter.
        /// </summary>
        /// <returns></returns>
        public List<Facility> GetFacilityList(string searchValue)
        {
            List<Facility> facilities = new List<Facility>();

            using (DataLayer dataLayer = new DataLayer())
            {
                facilities = dataLayer.GetFacilityList(searchValue);
            }
            return facilities;
        }

        #endregion

        #region FUNCTION: GetAddressByFacilityID(int facilityID)
        /// <summary>
        /// This function returns an Address object based on the FacilityID value. 
        /// A facility can only have one address which is why is it only returns one address only.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        public Address GetAddressByFacilityID(int facilityID)
        {
            Address address = new Address();
            using (DataLayer dataLayer = new DataLayer())
            {
                address = dataLayer.GetAddressByFacilityID(facilityID);
            }

            return address;
        }

        #endregion

        #region FUNCTION: SearchForValue(string searchValue)
        /// <summary>
        /// This function returns the SearchResults object with any records that match the value being requested. 
        /// The SearchResults object has three list of objects with data populated for each (if the search value matches any of the values in the database).
        /// FacilityProviderRelationship list, Provider list and Facility List.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public SearchResults SearchForValue(string searchValue)
        {
            SearchResults sr = new SearchResults();

            using (DataLayer dataLayer = new DataLayer())
            {
                sr.FacilityProviderRelationships = dataLayer.GetFacilityProviderRelationshipList(searchValue);
                sr.Facilities = dataLayer.GetFacilityList(searchValue);
                sr.Providers = dataLayer.GetProviderList(searchValue);
                sr.Vendors = dataLayer.GetVendorList(searchValue);

                //Remove duplicate objects (Facility/Provider/Vendor) if Facility Relationship already exist
                foreach (FacilityProviderRelationship facRel in sr.FacilityProviderRelationships)
                {
                    sr.Facilities.RemoveAll(f => f.ID == facRel.Facility.ID);
                    sr.Providers.RemoveAll(p => p.ID == facRel.Provider.ID);
                    sr.Vendors.RemoveAll(v => v.ID == facRel.Vendor.ID);

                    facRel.BehavioralHealthAttributes = dataLayer.GetBHAttributeByRelationshipID(facRel.RelationshipID);
                }
            }

            return sr;
        }
        #endregion

        #region FUNCTION: GetFacilityProviderRelationshipList(string searchValue)

        public List<FacilityProviderRelationship> GetFacilityProviderRelationshipList(string searchValue)
        {
            List<FacilityProviderRelationship> fpRelationship = new List<FacilityProviderRelationship>();

            using (DataLayer dataLayer = new DataLayer())
            {
                fpRelationship = dataLayer.GetFacilityProviderRelationshipList(searchValue);

                foreach (FacilityProviderRelationship relationship in fpRelationship)
                {
                    relationship.Vendor = dataLayer.GetVendorByFacilityID(relationship.Facility.ID);
                    relationship.BehavioralHealthAttributes = dataLayer.GetBHAttributeByRelationshipID(relationship.RelationshipID);
                }
            }

            return fpRelationship;
        }

        #endregion

        #region FUNCTION: FacilityProviderRelationship GetFacilityProviderRelationshipByID(int relationshipID)
        /// <summary>
        /// This function returns the FacilityProviderRelationship object based on the database RelationshipID value. 
        /// This is the Facility Provider Relationship/Profile data.
        /// </summary>
        /// <param name="relationshipID"></param>
        /// <returns></returns>
        public FacilityProviderRelationship GetFacilityProviderRelationshipByID(int relationshipID)
        {
            FacilityProviderRelationship relationship = new FacilityProviderRelationship();
            using (DataLayer dataLayer = new DataLayer())
            {
                relationship = dataLayer.GetFacilityProviderRelationshipByID(relationshipID);
                relationship.Vendor = dataLayer.GetVendorByFacilityID(relationship.Facility.ID);
                relationship.BehavioralHealthAttributes = dataLayer.GetBHAttributeByRelationshipID(relationshipID);
            }

            return relationship;
        }

        #endregion

        #region FUNCTION: SaveProviderDetail(Provider provider)
        /// <summary>
        /// This function returns the Provider ID of the Provider being inserted/updated in the database.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>Provider ID being inserted/updated in the database</returns>
        public int SaveProviderDetail(Provider provider)
        {
            int providerID = 0;

            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    providerID = dataLayer.SaveProviderDetail(provider);

                    //if (dataLayer.SaveCredentialByProviderID(providerID, provider.CredentialList))
                    //{
                    //    //Log that Credentials have been saved correctly
                    //}

                    //if (dataLayer.SaveLanguageByProviderID(providerID, provider.LanguageList))
                    //{
                    //    //Log that Language have been saved correctly
                    //}
                }

                return providerID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: SaveFacility(Facility facility)
        /// <summary>
        /// This function returns the FacilityID of the Facility being inserted/updated in the database.
        /// </summary>
        /// <param name="facility"></param>
        /// <returns>Facility ID being inserted/updated in the database</returns>
        public int SaveFacility(Facility facility)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.SaveFacility(facility);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: SaveFacilityProviderRelationship(FacilityProviderRelationship relationship)
        /// <summary>
        /// This function returns the RelationshipID after the FacilityProviderRelationship object data has been inserted/updated in the database. 
        /// </summary>
        /// <param name="relationship"></param>
        /// <returns>Relationship ID being inserted/updated in the database</returns>
        public int SaveFacilityProviderRelationship(FacilityProviderRelationship relationship)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.SaveFacilityProviderRelationship(relationship);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: SaveAddress(Address address)
        /// <summary>
        /// This function returns the AddressID after the Address object data has been inserted/updated in the database. 
        /// </summary>
        /// <param name="address"></param>
        /// <returns>Address ID being inserted/updated in the database</returns>
        public int SaveAddress(Address address)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.SaveAddress(address);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: MapAddressToFacility(int facilityID, int addressID, string createdBy)
        /// <summary>
        /// This function returns the Mapping Address to Facility ID after the Facility has been mapped to an address record. 
        /// To map an address to a facility, only specified the Address ID value and the Facility ID value. 
        /// The function also requests the name of the application user that is doing this mapping.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="addressID"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public int MapAddressToFacility(int facilityID, int addressID, string createdBy)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.MapAddressToFacility(facilityID, addressID, createdBy);  //TODO
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: GetLanguageList()
        /// <summary>
        /// This function returns the list of Provider spoken languages available in the system.
        /// </summary>
        /// <returns></returns>
        public List<Language> GetLanguageList()
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.GetLanguageList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: GetCredentialList()
        /// <summary>
        /// This function returns the list of Provider Credentials available in the system.    
        /// </summary>
        /// <returns></returns>
        public List<Credential> GetCredentialList()
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.GetCredentialList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: GetVendorList(string searchValue)
        /// <summary>
        ///  This function returns a list of Vendors available in the system, the searchValue parameter can be used to look up specific vendors by name. 
        ///  To get the full vendor list, send an empty value in the parameter.
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public List<Vendor> GetVendorList(string searchValue)
        {
            List<Vendor> vendors = new List<Vendor>();

            using (DataLayer dataLayer = new DataLayer())
            {
                vendors = dataLayer.GetVendorList(searchValue);
            }
            return vendors;
        }

        #endregion

        #region FUNCTION: SaveVendor(Vendor vendor)
        /// <summary>
        /// This function returns the Vendor ID after the Vendor object data has been inserted/updated in the database.
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public int SaveVendor(Vendor vendor)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.SaveVendor(vendor);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: MapAddressToVendor(int vendorID, int addressID, string createdBy)
        /// <summary>
        /// This function returns the Mapping Facility to Vendor ID after the Facility has been mapped to a Vendor record. 
        /// To map a facility to a vendor, only specified the Vendor ID value and the Facility ID value. 
        /// The function also requests the name of the application user that is doing this mapping.
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="addressID"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public int MapAddressToVendor(int vendorID, int addressID, string createdBy)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.MapAddressToVendor(vendorID, addressID, createdBy);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: MapFacilityToVendor(int facilityID, int vendorID, string createdBy)
        /// <summary>
        /// This function returns the Mapping Facility to Vendor ID after the Facility has been mapped to a vendor record. 
        /// To map a facility to a vendor, only specified the Facility ID value and the Vendor ID value. 
        /// The function also requests the name of the application user that is doing this mapping.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="vendorID"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public int MapFacilityToVendor(int facilityID, int vendorID, string createdBy)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.MapFacilityToVendor(facilityID, vendorID, createdBy);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: GetVendorByID(int vendorID)
        /// <summary>
        /// This function returns a Vendor object based on the database Vendor ID value (VENDOR_ID).
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        public Vendor GetVendorByID(int vendorID)
        {
            Vendor vendor = new Vendor();

            using (DataLayer dataLayer = new DataLayer())
            {
                vendor = dataLayer.GetVendorByID(vendorID);
            }

            return vendor;
        }

        #endregion

        #region FUNCTION: GetAddressByVendorID(int vendorID)
        /// <summary>
        /// This function returns a list of Addresses in the system for a specific Vendor.
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public List<Address> GetAddressByVendorID(int vendorID)
        {
            List<Address> vendorAddress = new List<Address>();

            using (DataLayer dataLayer = new DataLayer())
            {
                vendorAddress = dataLayer.GetAddressByVendorID(vendorID);
            }
            return vendorAddress;
        }

        #endregion

        #region FUNCTION: SaveLanguageByProviderID(int providerID, List<Language> languages)
        /// <summary>
        /// This function returns a Boolean (true for success, false for failure) after the Provider Languages have been inserted/updated/deleted in the database.
        /// </summary>
        /// <param name="providerID"></param>
        /// <param name="languages"></param>
        /// <returns></returns>
        public bool SaveLanguageByProviderID(int providerID, List<Language> languages)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.SaveLanguageByProviderID(providerID, languages);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FUNCTION: SaveCredentialByProviderID(int providerID, List<Credential> credentials)
        /// <summary>
        /// This function returns a Boolean (true for success, false for failure) after the Provider Credentials have been inserted/updated/deleted in the database.
        /// </summary>
        /// <param name="providerID"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public bool SaveCredentialByProviderID(int providerID, List<Credential> credentials)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.SaveCredentialByProviderID(providerID, credentials);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: AdvancedSearch(Dictionary<string, string> args)
        /// <summary>
        /// This function returns a list of Facility Provider Profile based on all of the Search fields
        /// specified in the Advanced Search page, (Single to Multiple values). 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<FacilityProviderRelationship> AdvancedSearch(Dictionary<string, List<string>> args)
        {
            //List<FacilityProviderRelationship> relationshipList = new List<FacilityProviderRelationship>();
            bool retVal = true;

            List<string> bhealthAttributes = args.Where(w => w.Key == "BHAttributeSet").SelectMany(i => i.Value).ToList();

            List<FacilityProviderRelationship> relationshipListWithCriteria = new List<FacilityProviderRelationship>();

            using (DataLayer dataLayer = new DataLayer())
            {
                List<FacilityProviderRelationship> relationshipList = dataLayer.AdvancedSearch(args);

                foreach (FacilityProviderRelationship relationship in relationshipList)
                {
                    relationship.BehavioralHealthAttributes = dataLayer.GetBHAttributeByRelationshipID(relationship.RelationshipID);

                    //if the relationship object has attributes and a list of attributes was specified
                    if (relationship.BehavioralHealthAttributes.Count > 0 && bhealthAttributes.Count > 0)
                    {
                        foreach (string setID in bhealthAttributes)
                        {
                            if (relationship.BehavioralHealthAttributes.Exists(element => element.SetID == Convert.ToInt32(setID)) == true)
                            {
                                retVal = true;
                            }
                            else
                            {
                                retVal = false;
                                break;
                            }
                        }
                    }

                    if (retVal == true)
                    {
                        relationshipListWithCriteria.Add(relationship);
                    }
                }
            }
            return relationshipListWithCriteria;
        }

        #endregion

        #region FUNCTION: GetBehavioralHealthAttributeByID(BHAttributeType bHAttributeType)
        /// <summary>
        /// This function returns a list of the Behavioral Health Characteristics based on a specific BHealth Characteristic Type.
        /// </summary>
        /// <param name="bHAttributeType"></param>
        /// <returns></returns>
        public List<BehavioralHealthAttribute> GetBehavioralHealthAttributeByID(BHAttributeType bHAttributeType)
        {
            List<BehavioralHealthAttribute> bhAttributeList = new List<BehavioralHealthAttribute>();

            using (DataLayer dataLayer = new DataLayer())
            {
                bhAttributeList = dataLayer.GetBehavioralHealthAttributeByID(bHAttributeType);
            }
            return bhAttributeList;
        }

        #endregion

        #region FUNCTION: GetBHAttributeByRelationshipID(int relationshipID)
        /// <summary>
        /// This function returns a list of the Behavioral Health Characteristics values for all the Characteristic types 
        /// based on a specific relationshipID
        /// </summary>
        /// <param name="relationshipID"></param>
        /// <returns></returns>
        public List<BehavioralHealthAttribute> GetBHAttributeByRelationshipID(int relationshipID)
        {
            List<BehavioralHealthAttribute> bhAttributeList = new List<BehavioralHealthAttribute>();

            using (DataLayer dataLayer = new DataLayer())
            {
                bhAttributeList = dataLayer.GetBHAttributeByRelationshipID(relationshipID);
            }
            return bhAttributeList;
        }

        #endregion

        #region FUNCTION: SaveBHAttributeToRelationship(int relationshipID, List<BehavioralHealthAttribute> bhAttributeList) 
        /// <summary>
        /// This function returns a Boolean (true for success, false for failure) after the Behavioral Health attributes have been inserted/updated/deleted in the database.
        /// </summary>
        /// <param name="relationshipID"></param>
        /// <param name="bhAttributeList"></param>
        /// <returns></returns>
        public bool SaveBHAttributeToRelationship(int relationshipID, List<BehavioralHealthAttribute> bhAttributeList)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.SaveBHAttributeToRelationship(relationshipID, bhAttributeList);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: SaveFacilityAndAddress(Facility facility)

        public int SaveFacilityAndAddress(Facility facility)
        {
            int facilityID = 0;
            int addressID = 0;
            int facilityAddressID = 0;

            try
            {
                facilityID = SaveFacility(facility);

                if (facilityID > 0 & facility.FacilityAddress != null)
                {
                    addressID = SaveAddress(facility.FacilityAddress);

                    if (addressID > 0)
                    {
                        if (IsAddressMappedToFacility(facilityID, addressID) != true)
                        {
                            facilityAddressID = MapAddressToFacility(facilityID, addressID, facility.LastUpdatedBy);
                        }
                    }
                }

                return facilityID;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: IsAddressMappedToFacility(int facilityID, addressID)

        public bool IsAddressMappedToFacility(int facilityID, int addressID)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.IsAddressMappedToFacility(facilityID, addressID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FUNCTION: GetRelationshipDataByFacilityID(int facilityID)

        public List<FacilityProviderRelationship> GetRelationshipDataByFacilityID(int facilityID)
        {
            List<FacilityProviderRelationship> relationships = new List<FacilityProviderRelationship>();

            using (DataLayer dataLayer = new DataLayer())
            {
                relationships = dataLayer.GetRelationshipDataByFacilityID(facilityID);
            }

            return relationships;
        }

        #endregion

        #region FUNCTION: GetRelationshipDataByProviderID(int providerID)

        public List<FacilityProviderRelationship> GetRelationshipDataByProviderID(int providerID)
        {
            List<FacilityProviderRelationship> relationships = new List<FacilityProviderRelationship>();

            using (DataLayer dataLayer = new DataLayer())
            {
                relationships = dataLayer.GetRelationshipDataByProviderID(providerID);
            }

            return relationships;
        }

        #endregion

        #region FUNCTION: MapProviderToFacility(int providerID, int facilityID, string createdBy)

        public int MapProviderToFacility(int providerID, int facilityID, string createdBy)
        {
            int relationshipID = 0;

            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    if (dataLayer.IsProviderAndFacilityMapped(providerID, facilityID) != true)
                    {
                        relationshipID = dataLayer.MapProviderToFacility(providerID, facilityID, createdBy);
                    }
                }

                return relationshipID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
