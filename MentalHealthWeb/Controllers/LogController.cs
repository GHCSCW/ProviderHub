using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProviderHubService;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using AngularTemplate.Models;

namespace AngularTemplate.Controllers
{

    
    [Route("api/[controller]")]
    [Authorize]
    public class LogController : Controller
    {
    //    [HttpPost]
    //    public IHttpActionResult Post([FromBody]LogEntry value)
    //    {
    //        //write scome code to log this data to a table. 
    //        IHttpActionResult ret;

    //        ret = Ok(true);
    //       return ret;
    //    }
    }
}