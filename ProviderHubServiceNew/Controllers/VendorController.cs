using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProviderHubService;

namespace ProviderHubServiceNew.Controllers
{
    [Route("api/vendor")]
    public class VendorController : Controller
    {
        // GET api/vendor/list
        //MODIFY FOR FRAMEWORK [HttpGet("list")]
        public List<Vendor> GetVendorList(string searchValue)
        {
            List<Vendor> vendors = new List<Vendor>();

            using (DataLayer dataLayer = new DataLayer())
            {
                vendors = dataLayer.GetVendorList(searchValue);
            }
            return vendors;
        }

        //POST api/vendor/save (sameQ as all save functions above)
        //MODIFY FOR FRAMEWORK [HttpPost("save")]
        public int SaveVendor(Vendor vendor)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.SaveVendor(vendor);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET api/vendor/{vendorID}
        //MODIFY FOR FRAMEWORK [HttpGet("{vendorID}")]
        public Vendor GetVendorByID(int vendorID)
        {
            Vendor vendor = new Vendor();

            using (DataLayer dataLayer = new DataLayer())
            {
                vendor = dataLayer.GetVendorByID(vendorID);
            }

            return vendor;
        }
    }
}