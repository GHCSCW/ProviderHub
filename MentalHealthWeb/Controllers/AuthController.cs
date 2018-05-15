
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MentalHealthWeb.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : Controller
    {

        [Flags]
        public enum UserPermissions
        {
            //#region Base Flags

            //Viewer = 0,
            //Editor = 1,
            //DoEverything = 1 << 1,
      
         

            //#endregion

            //#region Role Flags

            //SuperUser = DoEverything| Editor | Viewer ,
            //BehavorialHealthEditor = Viewer| Editor ,
            //BehavorialHealthUser = Viewer,
            //Anonymous = Viewer
        

            //#endregion
        }
        public class Roles
        {
            public string RoleName { get; set; }
            
            public Boolean InRole { get; set; }
            
        }

        public AuthController()
        {

        }
        [HttpGet("[action]")]
        public IActionResult GetUser()
        {

            if (User.Identity.IsAuthenticated)
            {
                //string hmo = "GHCHMO";
                string username = User.Identity.Name;

                Int32 max = username.Length - 8;
                username = username.Substring(8, max);

                return Ok(username);
            }
            else
            {
                return BadRequest("Not authenticated");
            }
        }
        [HttpGet("[action]")]
  
        public IActionResult GetUserRoles()
        {
            List<Roles> roles = new List<Roles>();
            if (User.Identity.IsAuthenticated)
            {
                Roles rolenames = new Roles();
                if (User.IsInRole(@"GHC-HMO\App_SmallGroupRenewals_Editor"))
                {
                    rolenames.RoleName = "SuperUser";
                    rolenames.InRole = true;
                    roles.Add(rolenames);
                }
                if (User.IsInRole(@"GHC-HMO\App_BehavioralHealth_Provider_Editor"))
                {
                    rolenames.RoleName = "Editor";
                    rolenames.InRole = true;
                    roles.Add(rolenames);
                }
                if (User.IsInRole(@"GHC-HMO\App_BehavorialHealth_Viewer"))
                {
                    rolenames.RoleName = "Viewer";
                    rolenames.InRole = true;
                    roles.Add(rolenames);

                }
                if(User.IsInRole(@"GHC-HMO\App_BehavioralHealth_Anonymous"))
                {
                    rolenames.RoleName = "Anonymous";
                    rolenames.InRole = true;
                    roles.Add(rolenames);
                }
                return Json(roles);
            }

            else
            {
                return BadRequest("Failure");
            }
            

        }

    }
}

