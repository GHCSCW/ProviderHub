﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using Newtonsoft.Json;
using System.IO;
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
            Facility facility = new Facility(); dynamic toReturn = new ExpandoObject();
            using (DataLayer dataLayer = new DataLayer())
            {
                facility = dataLayer.GetFacilityByID(id,true);
                toReturn.f = facility;
                toReturn.s = dataLayer.GetSpecialtyList(true);
            }
            var json = JsonConvert.SerializeObject(toReturn, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat, DateTimeZoneHandling = DateTimeZoneHandling.Unspecified });
            return Content(json, "application/json");
            //return Json(facility,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVendor(int id)
        {
            dynamic toReturn = new ExpandoObject();
            using (DataLayer dataLayer = new DataLayer())
            {
                toReturn = dataLayer.GetVendorAddressesByFacilityID(id);
            }
            var json = JsonConvert.SerializeObject(toReturn, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat, DateTimeZoneHandling = DateTimeZoneHandling.Unspecified });
            return Content(json, "application/json");
            //return Json(facility,JsonRequestBehavior.AllowGet);
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
            //0="FACILITY HEADER"
            //body = { Name:val("Name"), NPI: val("NPI"), User: "GHC-HMO\\spillai" };
            switch (type) { 
                case 0:
                    //actually save now and set toReturn.result if successful
                    using (DataLayer dataLayer = new DataLayer())
                    {
                        toReturn.result = dataLayer.SaveFacilityHeader(inputJSON); toReturn.success = true;
                    }
                    break;
                //1="FACILITY DEMO"
                /*body = { Address1: val2("Address1"), Address2: val2("Address2"), City: val2("City"), State: val2("State"), Zip: val2("ZipCode"), User: "GHC-HMO\\spillai" };
                     body.PhoneNumber = val2("PhoneNumber"); body.FaxNumber = val2("FaxNumber"); body.Website = val2("Website");*/
                case 1:
                    using (DataLayer dataLayer = new DataLayer())
                    {
                        toReturn.result = dataLayer.SaveFacilityDemo(inputJSON); toReturn.success = true;
                    }
                    break;
                //2="FACILITY SPECS"
                case 2:
                    toReturn.result = new List<dynamic>(); //we have potential for multiple SQL results in this case, so store them all in 'result'
                    for (var i = 0; i < inputJSON.FacilitySpecialties.Count; i++)
                    {
                        dynamic ps = inputJSON.FacilitySpecialties[i];
                        //Stored Proc needs: (@SpecialtyID VARCHAR(10),@User VARCHAR(20), @ID INT,      @SEQ INT, @EDATE DATE,       @TDATE DATE = NULL,   @First BIT = 0)
                        //                    ps.ID                    inputJSON.User     inputJSON.ID  i+1       ps.EffectiveDate   ps.TerminationDate    i==0? 1 : 0
                        dynamic forSP = new ExpandoObject(); forSP.SpecialtyID = ps.ID; forSP.User = inputJSON.User; forSP.ID = inputJSON.ID;
                        forSP.SEQ = i + 1; forSP.EDATE = ps.EffectiveDate; forSP.TDATE = ps.TerminationDate; forSP.First = (i == 0) ? 1 : 0;
                        forSP.Last = (i == inputJSON.FacilitySpecialties.Count - 1) ? 1 : 0;
                        using (DataLayer dataLayer = new DataLayer())
                        {
                            toReturn.result.Add(dataLayer.SaveFacilitySpecialty(forSP));
                        }
                    }
                    break;
                default: //log error
                    break;
            }

            //return JSON
            var json = JsonConvert.SerializeObject(toReturn);
            return Content(json, "application/json");
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