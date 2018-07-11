using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProviderHubService;

namespace ProviderHubRESTService.Controllers
{
    [Route("api/facility")]
    public class FacilityController : Controller
    {
        // GetFacilityByID(facilityID) - retrieve Facility object given ID
        // GET api/facility/{facilityID}
        [HttpGet("{facilityID}")]
        public Facility GetFacilityByID(int facilityID)
        {
            Facility facility = new Facility();
            using (DataLayer dataLayer = new DataLayer())
            {
                facility = dataLayer.GetFacilityByID(facilityID);
            }
            return facility;
        }

        // GET api/facility/{facilityID}/address
        [HttpGet("{facilityID}/address")]
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
        [HttpPost("save")]
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
        [HttpPost("mapToVendor")]
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