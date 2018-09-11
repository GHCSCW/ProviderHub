using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Http.Cors;
using ProviderHubService;

namespace ProviderHubServiceNew.Controllers
{
    //[Route("api/search")]
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class SearchController : Controller
    {
        // GetProviderList(searchValue) - retrieve List of providers that match the (given SQL subquery? or is it just keyword/name search?)
        // GET api/search/provider/{searchValue}
        //MODIFY FOR FRAMEWORK [HttpGet("provider/{searchValue}")]
        public ActionResult GetProviderList(string id)
        {
            List<Provider> providers = new List<Provider>();
            using (DataLayer dataLayer = new DataLayer())
            {
                providers = dataLayer.GetProviderList(id,true);
            }
            string uname = User.Identity.Name;
            return Json(providers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVendorList(string id) {
            List<Vendor> vendors = new List<Vendor>();
            using (DataLayer dataLayer = new DataLayer()) {
                vendors = dataLayer.GetVendorList(id, true);
            }
            return Json(vendors, JsonRequestBehavior.AllowGet);
        }

        // GET api/search/facility/{searchValue}
        //MODIFY FOR FRAMEWORK [HttpGet("facility/{searchValue}")]
        public ActionResult GetFacilityList(string id)
        {
            List<Facility> facilities = new List<Facility>();

            using (DataLayer dataLayer = new DataLayer())
            {
                facilities = dataLayer.GetFacilityList(id,true);
            }
            return Json(facilities, JsonRequestBehavior.AllowGet);
        }

        // GET api/search/{searchValue}
        //MODIFY FOR FRAMEWORK [HttpGet("{searchValue}")]
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
        //MODIFY FOR FRAMEWORK [HttpPost("advanced")]
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