using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jibberwock.Admin.API.Controllers.Authentication
{
    [ApiController]
    [Route("auth/[controller]")]
    [Authorize]
    public class RedirectController : JibberwockControllerBase
    {
        public RedirectController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource, IOptions<WebApiConfiguration> options) : base(loggerFactory, sqlServerDataSource, options) { }

        /// <summary>
        /// When used as a return URL from EasyAuth, redirects to one of a hardcoded set of URLs, as defined in static configuration.
        /// </summary>
        /// <param name="type">The type of the URL to go to.</param>
        /// <returns>An <see cref="IActionResult"/> for a HTTP 302 to the correct URL.</returns>
        [Route("{type}")]
        [HttpGet]
        public IActionResult RedirectToUrl(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                ModelState.AddModelError("missing_type", "Redirection type is missing.");
                return BadRequest(ModelState);
            }
            else if (! WebApiConfiguration.PermittedRedirects.TryGetValue(type, out var redirectUrl))
            {
                ModelState.AddModelError("invalid_type", "Invalid redirection type.");
                return BadRequest(ModelState);
            }
            else
            {
                var redirectUrlBuilder = new UriBuilder(redirectUrl)
                {
                    Query = HttpContext.Request.QueryString.ToUriComponent()
                };

                return Redirect(redirectUrlBuilder.Uri.ToString());
            }
        }
    }
}
