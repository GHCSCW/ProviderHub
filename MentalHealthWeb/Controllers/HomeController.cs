using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProviderHubService;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
namespace AngularTemplate.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    public class HomeController : Controller
    {
        public string username;


        public class AdvancedSearch
        {

            public string Key { get; set; }
            public string[] Value { get; set; }
            //public string languages  { get; set; }
        }
        

        public HomeController()
        {
            // System.Security.Claims.ClaimsPrincipal currentUser = User;
            //System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            username = Environment.UserName;
        }


        ProviderHubService.IProviderHubService ProviderHubService = new ProviderHubServiceClient();


        [HttpGet("[action]/{id}")]
        public async Task<Facility> GetFacilityById(int id)
        {
            Facility facility = await ProviderHubService.GetFacilityByIDAsync(id);


            return facility;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCredentialList()
        {
            Credential[] credentials = await ProviderHubService.GetCredentialListAsync();

            return Json(credentials);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> getCredentialListByProviderId(int id)
        {
            Credential[] credentials = await ProviderHubService.GetProviderCredentialByIDAsync(id);

            return Json(credentials);
        }



        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetVendorById(int id)
        {
            // int id = 5;
            Vendor vendor = await ProviderHubService.GetVendorByIDAsync(id);

            return Json(vendor);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetFacilityList(string search)
        {
            search = "";
            Facility[] list = await ProviderHubService.GetFacilityListAsync(search);

            return Json(list);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetVendorList(string search)
        {
            // int id = 5;
            Vendor[] vendor = await ProviderHubService.GetVendorListAsync(search);

            return Json(vendor);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLanguageList()
        {
            // int id = 5;
            Language[] languages = await ProviderHubService.GetLanguageListAsync();

            return Json(languages);
        }


        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetFacilityProviderRelationshipById(int id)
        {
            FacilityProviderRelationship facilityProviderRelationship = await ProviderHubService.GetFacilityProviderRelationshipByIDAsync(id);

            return Json(facilityProviderRelationship);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetProviderById(int id)
        {
            Provider provider = await ProviderHubService.GetProviderByIDAsync(id);
            return Json(provider);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProviderList(string search)
        {
            Provider[] list = await ProviderHubService.GetProviderListAsync(search);
            return Json(list);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetBehavioralHealthAttributeByID(ProviderHubService.BHAttributeType id)
        {
            BehavioralHealthAttribute[] list = await ProviderHubService.GetBehavioralHealthAttributeByIDAsync(id);
            return Json(list);
        }


        [HttpGet("[action]/{values}")]
        public async Task<IActionResult> SearchForValues(string values)
        {
            SearchResults list = await ProviderHubService.SearchForValueAsync(values);
            return Json(list);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AdvancedSearchMethod([FromBody]List<AdvancedSearch> values)
        {

            Dictionary<string, string[]> SearchList = new Dictionary<string, string[]>();

            foreach (AdvancedSearch val in values)
            {
                if (val.Value[0] == null)
                {

                }
                else
                {
                    SearchList.Add(val.Key, val.Value);
                }

            }

            //string[] gender = new string[] { "1" };
            //string[] language = new string[] { "1", "2" };
            //string[] csp = new string[] { "1" };

            //Dictionary<string, string[]> argList = new Dictionary<string, string[]>()
            //{
            //    {"Gender", gender},
            //    {"Language", language},
            //    {"CSP", csp}
            //};

            // FacilityProviderRelationship[] list2 = await ProviderHubService.AdvancedSearchAsync(argList);

            FacilityProviderRelationship[] list = await ProviderHubService.AdvancedSearchAsync(SearchList);
            
            return Json(list);




        }


        //TODO
        [HttpPost("[action]")]
        public async Task<IActionResult> MapAddressToFacility()
        {
            int facilityID = 2;
            int addressID = 3;
            string createdBy = username;
            int x = await ProviderHubService.MapAddressToFacilityAsync(facilityID, addressID, createdBy);
            return Json(x);

        }

        //TODO
        [HttpPost("[action]")]
        public async Task<IActionResult> MapAddressToVendor()
        {
            int vendorID = 2;
            int addressID = 3;
            string createdBy = username;
            int x = await ProviderHubService.MapAddressToVendorAsync(vendorID, addressID, createdBy);
            return Json(x);

        }

        //TODO
        [HttpPost("[action]")]
        public async Task<IActionResult> MapFacilityToVendor()
        {
            int facilityID = 2;
            int vendorID = 3;
            string createdBy = username;
            int x = await ProviderHubService.MapFacilityToVendorAsync(facilityID, vendorID, createdBy);
            return Json(x);

        }


        [HttpGet("[action]")]
        public async Task<IActionResult> AllFacilityProviderRelationships()
        {
            string blank = "";
            SearchResults list = await ProviderHubService.SearchForValueAsync(blank);
            return Json(list);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateAddress([FromBody]Address address)
        {

            address.CreatedBy = username;
            address.LastUpdatedBy = username;
            address.LastUpdatedDate = DateTime.Now;
            address.CreatedDate = DateTime.Now;
            int x = await ProviderHubService.SaveAddressAsync(address);
            if (x > 0)
            {
                return Ok(address);
            }
            else
            {
                return NotFound("There was an error creating the Address");
            }
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateProvider([FromBody]Provider provider)
        {
            provider.CreatedBy = username;
            provider.LastUpdatedBy = username;
            provider.LastUpdatedDate = DateTime.Now;
            provider.CreatedDate = DateTime.Now;

            int x = await ProviderHubService.SaveProviderDetailAsync(provider);
            if (x > 0)
            {
                return Ok(provider);
            }
            else
            {
                return NotFound("There was an error Creating the Provider");
            }
        }

        [HttpPost("[action]/{id}")]
        public async Task<IActionResult> UpdateCredentials([FromBody]Credential[] credentialUpdate, int id)
        {
        
            bool x = await ProviderHubService.SaveCredentialByProviderIDAsync(id, credentialUpdate);
            if (x == true)
            {
                return Ok(credentialUpdate);
             
            }
            else
            {
                return NotFound("Credential Update failed");
            }

        }
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateLanguage([FromBody]Language[] languageUpdate, int id)
        {

            bool x = await ProviderHubService.SaveLanguageByProviderIDAsync(id, languageUpdate);
            if (x == true)
            {
                return Ok(languageUpdate);

            }
            else
            {
                return NotFound("Language Update failed");
            }

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateProvider([FromBody]Provider providerUpdate)
        {
            providerUpdate.LastUpdatedBy = username;
            int x = await ProviderHubService.SaveProviderDetailAsync(providerUpdate);

            if (x > 0)
            {
                return Ok(providerUpdate);
            }
            else
            {
                return NotFound("There was an error updating the Provider");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateFacility([FromBody]Facility facilityUpdate)
        {
            facilityUpdate.LastUpdatedBy = username;

            int x = await ProviderHubService.SaveFacilityAsync(facilityUpdate);

            if (x > 0)
            {
                return Ok(facilityUpdate);
            }
            else
            {
                return NotFound("There was an error updating the Facility");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateFacility([FromBody]Facility facilityUpdate)
        {
            facilityUpdate.LastUpdatedBy = username;
            facilityUpdate.LastUpdatedDate = DateTime.Now;
            facilityUpdate.CreatedBy = username;
            facilityUpdate.CreatedDate = DateTime.Now;

            int x = await ProviderHubService.SaveFacilityAsync(facilityUpdate);

            if (x > 0)
            {
                return Ok(facilityUpdate);
            }
            else
            {
                return NotFound("There was an error creating the provider");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateFacilityProviderRelationship([FromBody]FacilityProviderRelationship facilityProvUpdate)
        {

            facilityProvUpdate.LastUpdatedBy = username;

            int x = await ProviderHubService.SaveFacilityProviderRelationshipAsync(facilityProvUpdate);

            if (x > 0)
            {
                return Ok(facilityProvUpdate);
            }
            else
            {
                return NotFound(" There was an error updating the Provider Relationship" + facilityProvUpdate.RelationshipID);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateVendor([FromBody]Vendor vendorUpdate)
        {
            vendorUpdate.LastUpdatedBy = username;
            int x = await ProviderHubService.SaveVendorAsync(vendorUpdate);

            if (x > 0)
            {
                return Ok(vendorUpdate);
            }
            else
            {
                return NotFound("There was an error updating the Vendor");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateVendor([FromBody]Vendor vendor)
        {
            vendor.LastUpdatedBy = username;
            vendor.LastUpdatedDate = DateTime.Now;
            vendor.CreatedBy = username;
            vendor.CreatedDate = DateTime.Now;

            int x = await ProviderHubService.SaveVendorAsync(vendor);

            if (x > 0)
            {
                return Ok(vendor);
            }
            else
            {
                return NotFound("There was an error creating the Vendor");
            }
        }


    }
}
//   //  = await ProviderHubService.GetProviderByIDAsync(id);
//    return null;
//    // return Json(provider);
//}

//[HttpPost("[action]")]
//public async Task<IActionResult> UpdateFacility()
//{

//    // Provider provider = await ProviderHubService.GetProviderByIDAsync(id);
//    return null;
//    // return Json(provider);
//}

//[HttpPut("[action]")]
//public async Task<IActionResult> LinkToProvider()
//{

//    // Provider provider = await ProviderHubService.GetProviderByIDAsync(id);
//    return null;
//    // return Json(provider);
//}
//[HttpPut("[action]")]
//public async Task<IActionResult> LinkToVendor()
//{

//    // Provider provider = await ProviderHubService.GetProviderByIDAsync(id);
//    return null;
//    // return Json(provider);
//}




