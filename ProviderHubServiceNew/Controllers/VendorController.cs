using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Dynamic;
using System.IO;
using Newtonsoft.Json;
using ProviderHubService;

namespace ProviderHubServiceNew.Controllers
{
    [Route("api/vendor")]
    public class VendorController : Controller
    {
        //http://localhost:51660/Vendor/ByID/46
        public ActionResult ByID(int id)
        {
            dynamic toReturn = new ExpandoObject();
            using (DataLayer dataLayer = new DataLayer())
            {
                toReturn = dataLayer.PHGetVendorByID(id);
            }
            var json = JsonConvert.SerializeObject(toReturn, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat, DateTimeZoneHandling = DateTimeZoneHandling.Unspecified });
            return Content(json, "application/json");
            //return Json(facility,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(int type, int id)
        {
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
            //0="Vendor HEADER"
            //body = { Name:val("Name"), NPI: val("NPI"), TaxID + EpicID, User: "GHC-HMO\\spillai" };
            if (type == 0)
            {
                //actually save now and set toReturn.result if successful
                using (DataLayer dataLayer = new DataLayer())
                {
                    toReturn.result = dataLayer.SaveVendorHeader(inputJSON); toReturn.success = true;
                }
            }
            //1="Vendor Addr"
            /*body = { AddressID:x, Address1: val2("Address1"), Address2: val2("Address2"), City: val2("City"), State: val2("State"), Zip: val2("ZipCode"), User: "GHC-HMO\\spillai" };
                 body.PhoneNumber = val2("PhoneNumber"); body.FaxNumber = val2("FaxNumber"); body.Website = val2("Website");*/
            if (type == 1)
            {
                using (DataLayer dataLayer = new DataLayer())
                {
                    toReturn.result = dataLayer.SaveVendorAddress(inputJSON); toReturn.success = true;
                }
            }
            //return JSON
            var json = JsonConvert.SerializeObject(toReturn);
            return Content(json, "application/json");
        }

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