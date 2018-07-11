using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProviderHubService;

namespace ProviderHubRESTService.Controllers
{
    [Route("api/credential")]
    public class CredentialController : Controller
    {
        // GET api/credential/list
        [HttpGet("list")]
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

        // POST api/credential/saveForProvider/{providerID} (same concern as saveLanguageByProviderID)
        [HttpPost("saveForProvider/{providerID}")]
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
    }
}