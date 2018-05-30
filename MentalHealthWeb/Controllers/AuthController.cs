
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

        private readonly ILogger _logger;


        public class Roles
        {
            public string RoleName { get; set; }
            
            public Boolean InRole { get; set; }
            
        }

        public AuthController(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger("BehavorialHealthAuthController");

        }
        [HttpGet("[action]")]
        public IActionResult GetUser()
        {

            if (User.Identity.IsAuthenticated)
            {
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
                if (User.IsInRole(@"GHC-HMO\App_BehavioralHealth_Super_User"))
                {
                    rolenames.RoleName = "SuperUser";
                    rolenames.InRole = true;
                    roles.Add(rolenames);
                }
                else if (User.IsInRole(@"GHC-HMO\App_BehavioralHealth_Provider_Editor"))
                {
                    rolenames.RoleName = "Editor";
                    rolenames.InRole = true;
                    roles.Add(rolenames);
                }
               else if (User.IsInRole(@"GHC-HMO\App_BehavioralHealth_Provider_User"))
                {
                    rolenames.RoleName = "User";
                    rolenames.InRole = true;
                    roles.Add(rolenames);
                }
                else
                {
                    rolenames.RoleName = "Anonymous";
                    rolenames.InRole = true;
                    roles.Add(rolenames);

                }
       
                foreach(Roles r in roles)
                {
                    _logger.LogInformation("User Roles: {@Identity} with Role: {@RoleName}", User.Identity.Name, r.RoleName); 
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

