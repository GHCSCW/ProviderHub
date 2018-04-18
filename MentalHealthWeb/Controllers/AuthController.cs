
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MentalHealthWeb.Models;
using System.Diagnostics;
using System.Threading.Tasks;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MentalHealthWeb.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : Controller
    {
        [HttpGet("[action]")]
        public IActionResult GetUser()
        {
            Debug.Write($"AuthenticationType: {User.Identity.AuthenticationType}");
            Debug.Write($"IsAuthenticated: {User.Identity.IsAuthenticated}");
            Debug.Write($"Name: {User.Identity.Name}");

            if (User.Identity.IsAuthenticated)
            {
                //return Ok($"Authenticated: {User.Identity.Name}");
                return Ok($"{User.Identity.Name}");
            }
            else
            {
                return BadRequest("Not authenticated");
            }
        }
    }
}
