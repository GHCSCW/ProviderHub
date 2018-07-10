
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BehavorialHealthWeb.Models;

namespace BehavorialHealthWeb.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    public class LogController : Controller
    {
        [HttpPost]
        public IActionResult Post([FromBody]LogEntry value)
        {
            //write scome code to log this data to a table. 
            IActionResult ret;

            ret = Ok(true);
            return ret;
        }
    }
}