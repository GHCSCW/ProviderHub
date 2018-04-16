
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AngularTemplate.Models;

namespace AngularTemplate.Controllers
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