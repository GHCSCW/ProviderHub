using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Http.Cors;
using ProviderHubService;

namespace ProviderHubServiceNew.Controllers
{
    //[Route("api/provider")]
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class ProviderController : Controller
    {
        // GetProviderByID(providerID) - retrieve provider object given ID
        // GET api/provider/{providerID}
        //MODIFY FOR FRAMEWORK [HttpGet("{providerID}")]
        public ActionResult ByID(int id)
        {
            Provider provider = new Provider();
            using (DataLayer dataLayer = new DataLayer())
            {
                provider = dataLayer.GetProviderByID(id,true);
            }
            string uname = User.Identity.Name;
            return Json(provider, JsonRequestBehavior.AllowGet);
        }

        // GetProviderLanguageByID(providerID) - retrieve List of languages for provider with given ID
        // GET api/provider/{providerID}/languages
        //MODIFY FOR FRAMEWORK [HttpGet("{providerID}/languages")]
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
        //MODIFY FOR FRAMEWORK [HttpGet("{providerID}/credentials")]
        public List<Credential> GetProviderCredentiallByID(int providerID)
        {
            List<Credential> credentials = new List<Credential>();
            using (DataLayer dataLayer = new DataLayer())
            {
                credentials = dataLayer.GetProviderCredentialByID(providerID);
            }
            return credentials;
        }

        // POST api/provider/save (Provider object as JSON body? or how does .NET Provider object get passed in?)
        //MODIFY FOR FRAMEWORK [HttpPost("save")]
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

        /* we don't need these for PH, but when fully moving BH to this "new" REST API, we will want to incorporate these in the new service
         * also could use as a reference for Specialty to some extent, since that model is similar to BHAttribute
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
        } */
        /*end stuff*/

        /*Textbook Examples for classic REST in case we want to use them or build this as Explicit REST
         (we are building "REST-like/RESTful" service, not using PUT and DELETE explicitly, but with Select/Insert/Update/Delete operations via GET and POST)
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
        */
    }
}
