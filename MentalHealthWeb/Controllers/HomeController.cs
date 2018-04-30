using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProviderHubService;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using Microsoft.AspNetCore.Cors;

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
        }
        public HomeController()
        {
            username = Environment.UserName;
        }
        ProviderHubService.IProviderHubService ProviderHubService = new ProviderHubServiceClient();

        #region FUNCTION: GetFacilityByID(int id)   
        [HttpGet("[action]/{id}")]
        public async Task<Facility> GetFacilityById(int id)
        {
            Facility facility = await ProviderHubService.GetFacilityByIDAsync(id);
            return facility;
        }
        #endregion

        #region FUNCTION: GetCredentialList  
        [HttpGet("[action]")]
        public async Task<IActionResult> GetCredentialList()
        {
            Credential[] credentials = await ProviderHubService.GetCredentialListAsync();

            return Json(credentials);
        }
        #endregion

        #region FUNCTION: GetCredentialListByProviderId(int id)
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> getCredentialListByProviderId(int id)
        {
            Credential[] credentials = await ProviderHubService.GetProviderCredentialByIDAsync(id);
            return Json(credentials);
        }

        #endregion

        #region FUNCTION: GetVendorById(int id)
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetVendorById(int id)
        {
            Vendor vendor = await ProviderHubService.GetVendorByIDAsync(id);
            return Json(vendor);
        }
        #endregion

        #region FUNCTION: GetFacilityList(string search)
        [HttpGet("[action]")]
        public async Task<IActionResult> GetFacilityList(string search)
        {
            search = "";
            Facility[] list = await ProviderHubService.GetFacilityListAsync(search);
            return Json(list);
        }

        #endregion

        #region FUNCTION: GetVendorList(string search)
        [HttpGet("[action]")]
        public async Task<IActionResult> GetVendorList(string search)
        {
            Vendor[] vendor = await ProviderHubService.GetVendorListAsync(search);
            return Json(vendor);
        }

        #endregion

        #region FUNCTION: GetLanguageList()
        [HttpGet("[action]")]
        public async Task<IActionResult> GetLanguageList()
        {
            // int id = 5;
            Language[] languages = await ProviderHubService.GetLanguageListAsync();

            return Json(languages);
        }

        #endregion

        #region FUNCTION: GetFacilityProviderRelationshipId(int id)
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetFacilityProviderRelationshipById(int id)
        {
            FacilityProviderRelationship facilityProviderRelationship = await ProviderHubService.GetFacilityProviderRelationshipByIDAsync(id);
            return Json(facilityProviderRelationship);
        }
        #endregion

        #region FUNCTION: GetProviderById(int id)

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetProviderById(int id)
        {
            Provider provider = await ProviderHubService.GetProviderByIDAsync(id);
            return Json(provider);
        }

        #endregion

        #region FUNCTION: GetProviderList()

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProviderList(string search)
        {
            Provider[] list = await ProviderHubService.GetProviderListAsync(search);
            return Json(list);
        }

        #endregion

        #region FUNCTION: GetBehavioralHealthAttributeById(BHAttributeType id)
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetBehavioralHealthAttributeByID(ProviderHubService.BHAttributeType id)
        {
            BehavioralHealthAttribute[] list = await ProviderHubService.GetBehavioralHealthAttributeByIDAsync(id);
            return Json(list);
        }

        #endregion

        #region FUNCTION: SearchForValues(string values)
        [HttpGet("[action]/{values}")]
        public async Task<IActionResult> SearchForValues(string values)
        {
            SearchResults list = await ProviderHubService.SearchForValueAsync(values);
            return Json(list);
        }
        #endregion

        #region FUNCTION: AdvancedSearchMethod(List<AdvancedSearch> values)
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
        #endregion

        #region FUNCTION: MapAddressToFacility(facilityId,addressID,createdBy)
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
        #endregion

        #region FUNCTION: MapAddressToVendor(vendorID,addressID,createdBy)
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
        #endregion

        #region FUNCTION: MapFaciltyToVendor(faciltiyID,vendorID,createdBy))
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
        #endregion

        #region FUNCTION: AllFacilityProviderRelationships()
        [HttpGet("[action]")]
        public async Task<IActionResult> AllFacilityProviderRelationships()
        {
            string blank = "";
            SearchResults list = await ProviderHubService.SearchForValueAsync(blank);
            return Json(list);
        }
        #endregion

        #region FUNCTION: Create Address(Address address)
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
        #endregion

        #region FUNCTION: CreateProvider(Provider provider)
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
                bool y = await ProviderHubService.SaveCredentialByProviderIDAsync(x, provider.CredentialList);
                bool z = await ProviderHubService.SaveLanguageByProviderIDAsync(x, provider.LanguageList);

                if (y == true && z == true)
                {
                    return Ok(provider);
                }
                else
                {
                    return NotFound("There was an issue save credentials or languages");
                }
            }
            else
            {
                return NotFound("There was an error Creating the Provider");
            }
        }
        #endregion

        #region FUNCTION: UpdateCredentials(Credentia[] credentialUpdate, int id)
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
        #endregion

        #region FUNCTION: UpdateBhAttributes(BehavioralHealthAttribute[] attribute, int id)
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateBhAttributes([FromBody]BehavioralHealthAttribute[] attribute, int id)
        {
            bool x = await ProviderHubService.SaveBHAttributeToRelationshipAsync(id, attribute);
            if (x == true)
            {
                return Ok(attribute);
            }
            else
            {
                return NotFound("Credential Update failed");
            }
        }
        #endregion

        #region FUNCTION: UpdateLanguage(Language[] languageUpdate, int id)
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

        #endregion

        #region FUNCTION: UpdateProvider(Provider providerUpdate)
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateProvider([FromBody]Provider providerUpdate)
        {
            providerUpdate.LastUpdatedBy = username;
            int x = await ProviderHubService.SaveProviderDetailAsync(providerUpdate);
            if (x > 0)
            {
                bool y = await ProviderHubService.SaveCredentialByProviderIDAsync(x, providerUpdate.CredentialList);
                bool z = await ProviderHubService.SaveLanguageByProviderIDAsync(x, providerUpdate.LanguageList);

                if (y == true && z == true)
                {
                    return Ok(providerUpdate);
                }
                else
                {
                    return NotFound("There was an issue save credentials or languages");
                }
            }
            else
            {
                return NotFound("There was an error Creating/Updating the Provider");
            }
        }

        #endregion

        #region FUNCTION: UpdateFacility(Facility facilityUpdate)
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateFacility([FromBody]Facility facilityUpdate)
        {
            facilityUpdate.LastUpdatedBy = username;
            int x = await ProviderHubService.SaveFacilityAndAddressAsync(facilityUpdate);
            if (x > 0)
            {
                return Ok(facilityUpdate);
            }
            else
            {
                return NotFound("There was an error Creating/Updating the Facility");
            }
        }

        #endregion

        #region FUNCTION: CreateFacility(Facility facilityUpdate)
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

        #endregion

        #region FUNCTION: UpdateFacilityProviderRelationship(FacilityProviderRelationship facilityProvUpdate)
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

        #endregion

        #region FUNCTION: UpdateVendor(Vendor vendorUpdate)
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

        #endregion

        #region FUNCTION: CreateVendor(Vendor vendor)
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
        #endregion

    }
}

