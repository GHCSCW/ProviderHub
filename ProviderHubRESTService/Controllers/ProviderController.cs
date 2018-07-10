using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProviderHubService;

namespace ProviderHubRESTService.Controllers
{
    [Route("api/provider")]
    public class ProviderController : Controller
    {
        /*GETs FOR PROVIDER ENTITY*/
        // GetProviderByID(providerID) - retrieve provider object given ID
        // GET api/provider/{providerID}
        [HttpGet("{providerID}")]
        public Provider GetProviderByID(int providerID)
        {
            Provider provider = new Provider();
            using (DataLayer dataLayer = new DataLayer())
            {
                provider = dataLayer.GetProviderByID(providerID);
            }
            return provider;
        }

        // GetProviderLanguageByID(providerID) - retrieve List of languages for provider with given ID
        // GET api/provider/{providerID}/languages
        [HttpGet("{providerID}/languages")]
        public List<Language> GetProviderLanguageByID(int providerID)
        {
            List<Language> languages = new List<Language>();
            using (DataLayer dataLayer = new DataLayer())
            {
                languages = dataLayer.GetProviderLanguageByID(providerID);
            }
            return languages;
        }

        // GetProviderCredentiallByID(providerID) - retrieve List of credentials for provider with given ID
        // GET api/provider/{providerID}/credentials
        [HttpGet("{providerID}/credentials")]
        public List<Credential> GetProviderCredentiallByID(int providerID)
        {
            List<Credential> credentials = new List<Credential>();
            using (DataLayer dataLayer = new DataLayer())
            {
                credentials = dataLayer.GetProviderCredentialByID(providerID);
            }
            return credentials;
        }

        // GetProviderList(searchValue) - retrieve List of providers that match the (given SQL subquery? or is it just keyword/name search?)
        // GET api/provider/{searchValue}
        [HttpGet("{searchValue}")]
        public List<Provider> GetProviderList(string searchValue)
        {
            List<Provider> providers = new List<Provider>();
            using (DataLayer dataLayer = new DataLayer())
            {
                providers = dataLayer.GetProviderList(searchValue);
            }
            return providers;
        }
        /*END GETs FOR PROVIDER ENTITY*/
        /*GETs FOR FACILITY ENTITY*/
        // GetFacilityByID(facilityID) - retrieve Facility object given ID
        // GET api/(move to facility)/{facilityID}
        [HttpGet("{facilityID}")]
        public Facility GetFacilityByID(int facilityID)
        {
            Facility facility = new Facility();
            using (DataLayer dataLayer = new DataLayer())
            {
                facility = dataLayer.GetFacilityByID(facilityID);
            }
            return facility;
        }

        public List<Facility> GetFacilityList(string searchValue)
        {
            List<Facility> facilities = new List<Facility>();

            using (DataLayer dataLayer = new DataLayer())
            {
                facilities = dataLayer.GetFacilityList(searchValue);
            }
            return facilities;
        }

        public Address GetAddressByFacilityID(int facilityID)
        {
            Address address = new Address();
            using (DataLayer dataLayer = new DataLayer())
            {
                address = dataLayer.GetAddressByFacilityID(facilityID);
            }

            return address;
        }

        /*END GETs FOR FACILITY ENTITY*/
        /*stuff*/
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
                }
            }

            return sr;
        }
        public List<FacilityProviderRelationship> GetFacilityProviderRelationshipList(string searchValue)
        {
            List<FacilityProviderRelationship> fpRelationship = new List<FacilityProviderRelationship>();

            using (DataLayer dataLayer = new DataLayer())
            {
                fpRelationship = dataLayer.GetFacilityProviderRelationshipList(searchValue);

                //Remove duplicate objects (Facility/Provider/Vendor) if Facility Relationship already exist
                foreach (FacilityProviderRelationship relationship in fpRelationship)
                {
                    relationship.Vendor = dataLayer.GetVendorByFacilityID(relationship.Facility.ID);
                    relationship.BehavioralHealthAttributes = dataLayer.GetBHAttributeByRelationshipID(relationship.RelationshipID);
                }
            }

            return fpRelationship;
        }
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
        public int SaveProviderDetail(Provider provider)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.SaveProviderDetail(provider);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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
        public List<Vendor> GetVendorList(string searchValue)
        {
            List<Vendor> vendors = new List<Vendor>();

            using (DataLayer dataLayer = new DataLayer())
            {
                vendors = dataLayer.GetVendorList(searchValue);
            }
            return vendors;
        }
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
        public Vendor GetVendorByID(int vendorID)
        {
            Vendor vendor = new Vendor();

            using (DataLayer dataLayer = new DataLayer())
            {
                vendor = dataLayer.GetVendorByID(vendorID);
            }

            return vendor;
        }
        public List<Address> GetAddressByVendorID(int vendorID)
        {
            List<Address> vendorAddress = new List<Address>();

            using (DataLayer dataLayer = new DataLayer())
            {
                vendorAddress = dataLayer.GetAddressByVendorID(vendorID);
            }
            return vendorAddress;
        }
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
        public List<FacilityProviderRelationship> AdvancedSearch(Dictionary<string, List<string>> args)
        {
            List<FacilityProviderRelationship> relationshipList = new List<FacilityProviderRelationship>();

            using (DataLayer dataLayer = new DataLayer())
            {
                relationshipList = dataLayer.AdvancedSearch(args);

                foreach (FacilityProviderRelationship relationship in relationshipList)
                {
                    relationship.BehavioralHealthAttributes = dataLayer.GetBHAttributeByRelationshipID(relationship.RelationshipID);
                }
            }
            return relationshipList;
        }
        public List<BehavioralHealthAttribute> GetBehavioralHealthAttributeByID(BHAttributeType bHAttributeType)
        {
            List<BehavioralHealthAttribute> bhAttributeList = new List<BehavioralHealthAttribute>();

            using (DataLayer dataLayer = new DataLayer())
            {
                bhAttributeList = dataLayer.GetBehavioralHealthAttributeByID(bHAttributeType);
            }
            return bhAttributeList;
        }
        public List<BehavioralHealthAttribute> GetBHAttributeByRelationshipID(int relationshipID)
        {
            List<BehavioralHealthAttribute> bhAttributeList = new List<BehavioralHealthAttribute>();

            using (DataLayer dataLayer = new DataLayer())
            {
                bhAttributeList = dataLayer.GetBHAttributeByRelationshipID(relationshipID);
            }
            return bhAttributeList;
        }
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
        /*end stuff*/

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
