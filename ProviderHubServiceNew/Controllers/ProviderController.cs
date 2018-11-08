using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Http.Cors;
using System.Dynamic;
using Newtonsoft.Json;
using ProviderHubService;
using System.IO;

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
            Provider provider = new Provider(); dynamic toReturn = new ExpandoObject();
            using (DataLayer dataLayer = new DataLayer())
            {
                provider = dataLayer.GetProviderByID(id,true);
                List<Language> l = dataLayer.GetLanguageList(true);
                List<Credential> c = dataLayer.GetCredentialList(true);
                List<Specialty> s = dataLayer.GetSpecialtyList(true);
                List<NetworkTab> n = dataLayer.GetNetworkTabByPID(id);
                string ha = dataLayer.GetHospitalAffiliationByPID(id);
                var uname = User.Identity;
                toReturn.p = provider; toReturn.l = l; toReturn.c = c; toReturn.n = uname; toReturn.s = s; toReturn.net = n; toReturn.ha = ha;
            }
            var json = JsonConvert.SerializeObject(toReturn, new JsonSerializerSettings{ DateFormatHandling = DateFormatHandling.MicrosoftDateFormat, DateTimeZoneHandling = DateTimeZoneHandling.Unspecified });
            return Content(json, "application/json");
            //return Json(toReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(int type, int id) {
            //get raw POST body (json) as string, deserialize into object
            MemoryStream memstream = new MemoryStream();
            Request.InputStream.CopyTo(memstream);
            memstream.Position = 0; dynamic inputJSON = new ExpandoObject();
            using (StreamReader reader = new StreamReader(memstream))
            {
                string text = reader.ReadToEnd(); 
                inputJSON = JsonConvert.DeserializeObject<object>(text);
            }
            //return everything passed in: type, id, and POST body object
            dynamic toReturn = new ExpandoObject(); inputJSON.type = type; inputJSON.ID = id;
            inputJSON.User = (!User.Identity.IsAuthenticated) ? "Not Authorized" : User.Identity.Name;
            toReturn.result = false; toReturn.POSTvars = JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(inputJSON));
            //handle each type accordingly
            //0="PROVIDER HEADER"
            if (type == 0) {
                inputJSON.Credstr = "";
                for (var i = 0; i < inputJSON.Credentials.Count; i++) { inputJSON.Credstr += inputJSON.Credentials[i]; if (i != inputJSON.Credentials.Count - 1) { inputJSON.Credstr += ","; } }
                var _v = 0; if (inputJSON.Gender == "Male") { _v = 2; } if (inputJSON.Gender == "Female") { _v = 1; } inputJSON.Gender = _v; toReturn.POSTvars.Gender = _v;
                //actually save now and set toReturn.result if successful
                using (DataLayer dataLayer = new DataLayer()) {
                    toReturn.result = dataLayer.SaveProviderHeader(inputJSON); toReturn.success = true;
                }
            }
            //1="PROVIDER DEMO"
            if (type == 1) {
                inputJSON.Langstr = "";
                for (var i = 0; i < inputJSON.Languages.Count; i++) { inputJSON.Langstr += inputJSON.Languages[i]; if (i != inputJSON.Languages.Count - 1) { inputJSON.Langstr += ","; } }
                //convert Yes/No(/Unknown) into BIT values 1/0(/NULL) if applicable
                inputJSON.MedicareIndicator = (inputJSON.MedicareIndicator == "Yes") ? 1 : (inputJSON.MedicareIndicator == "No") ? (int?)0 : null;
                inputJSON.MedicaidIndicator = (inputJSON.MedicaidIndicator == "Yes") ? 1 : (inputJSON.MedicaidIndicator == "No") ? (int?)0 : null;
                //If Yes for any of the indicators, show corresponding indicators' other fields
                //Medicare: MedicarePTAN, MedicareEffectiveDate, MedicareTerminationDate | Medicaid: MedicaidProviderID
                inputJSON.MedicarePTAN = (inputJSON.MedicareIndicator == 1) ? inputJSON.MedicarePTAN : null;//need ==true since in C#, null does not eval to false
                inputJSON.MedicareEffectiveDate = (inputJSON.MedicareIndicator == 1) ? inputJSON.MedicareEffectiveDate : null;
                inputJSON.MedicareTerminationDate = (inputJSON.MedicareIndicator == 1) ? inputJSON.MedicareTerminationDate : null;
                inputJSON.MedicaidProviderID = (inputJSON.MedicaidIndicator == 1) ? inputJSON.MedicaidProviderID : null;
                using (DataLayer dataLayer = new DataLayer()) {
                    toReturn.result = dataLayer.SaveProviderDemo(inputJSON); toReturn.success = true;
                }
            }
            //2="PROVIDER SPECS"

            //return JSON
            var json = JsonConvert.SerializeObject(toReturn);
            return Content(json, "application/json");
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
