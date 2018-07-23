using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProviderHubService;

namespace ProviderHubServiceNew.Controllers
{
    [Route("api/facility")]
    public class FacilityController : Controller
    {
        // GetFacilityByID(facilityID) - retrieve Facility object given ID
        // GET api/facility/{facilityID}
        //MODIFY FOR FRAMEWORK [HttpGet("{facilityID}")]
        public ActionResult ByID(int id)
        {
            Facility facility = new Facility();
            using (DataLayer dataLayer = new DataLayer())
            {
                facility = dataLayer.GetFacilityByID(id,true);
            }
            return Json(facility,JsonRequestBehavior.AllowGet);
        }

        // GET api/facility/{facilityID}/address
        //MODIFY FOR FRAMEWORK [HttpGet("{facilityID}/address")]
        public Address GetAddressByFacilityID(int facilityID)
        {
            Address address = new Address();
            using (DataLayer dataLayer = new DataLayer())
            {
                address = dataLayer.GetAddressByFacilityID(facilityID);
            }

            return address;
        }

        // POST api/facility/save (sameQuestion as saveProvider)
        //MODIFY FOR FRAMEWORK [HttpPost("save")]
        public int SaveFacility(Facility facility)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.SaveFacility(facility);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST api/facility/mapToVendor (best performing way to take in params from Angular/ajax? how they do now?)
        //MODIFY FOR FRAMEWORK [HttpPost("mapToVendor")]
        public int MapFacilityToVendor(int facilityID, int vendorID, string createdBy)
        {
            try
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    return dataLayer.MapFacilityToVendor(facilityID, vendorID, createdBy);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}