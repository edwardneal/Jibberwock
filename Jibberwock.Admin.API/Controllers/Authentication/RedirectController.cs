using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jibberwock.Admin.API.Controllers.Authentication
{
    [ApiController]
    [Route("auth/[controller]")]
    public class RedirectController : ControllerBase
    {
        [Route("{type}")]
        [HttpGet]
        public ActionResult RedirectToUrl(string type)
        {
            //create the user in the api database, if required. alternatively, shuffle that feature into a custom middleware to provide authentication/authorisation
            //grab the item in Configuration.PermittedRedirects.{type}, combine it with any query string items and fragments, then redirect to the URL
            return Redirect($"https://admin.jibberwock.com/?type={type}");
        }
    }
}
