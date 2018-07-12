using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProviderHubService;

namespace ProviderHubServiceNew.Controllers
{
    [Route("api/address")]
    public class AddressController : Controller
    {
        // POST api/address/save (sameQuestion as saveProvider)
        //MODIFY FOR FRAMEWORK [HttpPost("save")]
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

        // POST api/address/mapToFacility (best performing way to take in params from Angular/ajax? how they do now?)
        //MODIFY FOR FRAMEWORK [HttpPost("mapToFacility")]
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


        // POST api/address/mapToVendor (best performing way to take in params from Angular/ajax? how they do now?)
        //MODIFY FOR FRAMEWORK [HttpPost("mapToVendor")]
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

        // GET api/address/byVendorID/{vendorID}
        //MODIFY FOR FRAMEWORK [HttpGet("byVendorID/{vendorID}")]
        public List<Address> GetAddressByVendorID(int vendorID)
        {
            List<Address> vendorAddress = new List<Address>();

            using (DataLayer dataLayer = new DataLayer())
            {
                vendorAddress = dataLayer.GetAddressByVendorID(vendorID);
            }
            return vendorAddress;
        }


    }
}