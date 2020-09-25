using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jibberwock.Admin.API.Controllers.Authentication
{
    [ApiController]
    [Route("auth/[controller]")]
    [Authorize]
    public class RedirectController : JibberwockControllerBase
    {
        public RedirectController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource) : base(loggerFactory, sqlServerDataSource) { }

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
