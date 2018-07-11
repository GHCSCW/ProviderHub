using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProviderHubService;

namespace ProviderHubRESTService.Controllers
{
    [Route("api/search")]
    public class SearchController : Controller
    {
        // GetProviderList(searchValue) - retrieve List of providers that match the (given SQL subquery? or is it just keyword/name search?)
        // GET api/search/provider/{searchValue}
        [HttpGet("provider/{searchValue}")]
        public List<Provider> GetProviderList(string searchValue)
        {
            List<Provider> providers = new List<Provider>();
            using (DataLayer dataLayer = new DataLayer())
            {
                providers = dataLayer.GetProviderList(searchValue);
            }
            return providers;
        }

        // GET api/search/facility/{searchValue}
        [HttpGet("facility/{searchValue}")]
        public List<Facility> GetFacilityList(string searchValue)
        {
            List<Facility> facilities = new List<Facility>();

            using (DataLayer dataLayer = new DataLayer())
            {
                facilities = dataLayer.GetFacilityList(searchValue);
            }
            return facilities;
        }

        // GET api/search/{searchValue}
        [HttpGet("{searchValue}")]
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

        // POST api/search/advanced (Dictionary of key=string and value=List of strings passed in, prob. via JSON, from angular/ajax. figure out best/most performant way?)
        [HttpPost("advanced")]
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
    }
}