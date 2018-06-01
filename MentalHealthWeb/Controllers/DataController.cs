
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BehavorialHealthWeb.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BehavorialHealthWeb.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        [HttpPost]
        [Route("save")]
        public IActionResult Save(PostData data)
        {
            return Ok(data.ToString());
        }
    }
}
