using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProviderHubService;

namespace ProviderHubServiceNew.Controllers
{
    [Route("api/language")]
    public class LanguageController : Controller
    {
        // GET api/language/list
        //MODIFY FOR FRAMEWORK [HttpGet("list")]
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

        // POST api/language/saveForProvider/{providerID} (List of Languages passed in request body (from angular/ajax), figure out best/most performant way?)
        //MODIFY FOR FRAMEWORK [HttpPost("saveForProvider/{providerID}")]
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
    }
}