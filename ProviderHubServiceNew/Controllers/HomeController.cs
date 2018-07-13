using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
                toReturn.Add("isSuperUser",User.IsInRole(@"GHC-HMO\App_BehavioralHealth_Super_User").ToString());
                toReturn.Add("isEditor", User.IsInRole(@"GHC-HMO\App_BehavioralHealth_Provider_Editor").ToString());
                toReturn.Add("isUser", User.IsInRole(@"GHC-HMO\App_BehavioralHealth_Provider_User").ToString());
            }
            return Json(toReturn, JsonRequestBehavior.AllowGet);
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