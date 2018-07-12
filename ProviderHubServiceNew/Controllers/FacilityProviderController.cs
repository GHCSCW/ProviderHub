using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProviderHubService;

namespace ProviderHubServiceNew.Controllers
{
    [Route("api/facilityprovider")]
    public class FacilityProviderController : Controller
    {
        // GET api/facilityprovider/relationshipList/{searchValue}
        //MODIFY FOR FRAMEWORK [HttpGet("relationshipList/{searchValue}")]
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

        // GET api/facilityprovider/relationshipByID/{relationshipID}
        //MODIFY FOR FRAMEWORK [HttpGet("relationshipByID/{relationshipID}")]
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

        // POST api/facilityprovider/save (sameQuestion as saveProvider)
        //MODIFY FOR FRAMEWORK [HttpPost("save")]
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


    }
}