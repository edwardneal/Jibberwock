using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        /// <response code="302" nullable="false">A redirect to the correct URL.</response>
        [Route("{type}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public IActionResult RedirectToUrl([FromRoute, BindRequired] string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                ModelState.AddModelError(ErrorResponses.MissingRedirectionType, string.Empty);
                return BadRequest(ModelState);
            }
            else if (! WebApiConfiguration.PermittedRedirects.TryGetValue(type, out var redirectUrl))
            {
                ModelState.AddModelError(ErrorResponses.InvalidRedirectionType, string.Empty);
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
