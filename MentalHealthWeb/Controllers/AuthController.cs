
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MentalHealthWeb.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : Controller
    {
        
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

                Int32 max = username.Length-9;
                 username = username.Substring(9, max);
              
                return Ok(username);
            }
            else
            {
                return BadRequest("Not authenticated");
            }
        }
    }
}
