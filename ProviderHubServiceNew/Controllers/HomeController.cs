using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using Newtonsoft.Json;
using CsvHelper;
using System.IO;
using ProviderHubService;

namespace ProviderHubServiceNew.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Auth() {
            Dictionary<string,string> toReturn = new Dictionary<string, string>();
            string toAdd = (!User.Identity.IsAuthenticated) ? "Not Authorized" : User.Identity.Name;
            toReturn.Add("result", toAdd);
            if (User.Identity.IsAuthenticated) {
                toReturn.Add("username", User.Identity.Name.Substring(8));
                toReturn.Add("isSuperUser",User.IsInRole(@"GHC-HMO\App_ProviderHub_Super_User").ToString());
                toReturn.Add("isEditor", User.IsInRole(@"GHC-HMO\App_ProviderHub_Editor").ToString());
                toReturn.Add("isUser", User.IsInRole(@"GHC-HMO\App_ProviderHub_User").ToString());
            }
            return Json(toReturn, JsonRequestBehavior.AllowGet);
        }
        //uwimport demo
        public ActionResult UWDataImport() {
            dynamic toReturn = new ExpandoObject();
            toReturn.FileName = Request.Files[0].FileName;
            toReturn.Errors = new List<String>();
            if (!Request.Files[0].FileName.Contains(".csv")) {
                toReturn.Errors.Add("Invalid File Type (must be .csv");
            } else {
                var textReader = new StreamReader(Request.Files[0].InputStream);
                var csv = new CsvReader(textReader);
                var records = csv.GetRecords<dynamic>();
                List<dynamic> _records = new List<dynamic>();
                foreach (var record in records) {
                    _records.Add(record);
                }
                //pass in array of dynamic objects mapping to csv, to DataLayer.UWImport
                //toReturn.CSVRowsArray = records;
                toReturn.CSVRowsCount = records.Count();
                DataLayer dl = new DataLayer();
                toReturn.id_result = dl.GetIDResult(_records);
                toReturn.map_result = dl.GetMapResult(toReturn.id_result, _records);
                toReturn.term_result = dl.GetTermResult(toReturn.id_result, toReturn.map_result, _records);
                //clear baseTables for JSON sake
                toReturn.id_result.baseTables = null;
            }
            var json = JsonConvert.SerializeObject(toReturn, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat, DateTimeZoneHandling = DateTimeZoneHandling.Unspecified });
            return Content(json, "application/json");
        }
        //Default routes below, kept only for reference
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}