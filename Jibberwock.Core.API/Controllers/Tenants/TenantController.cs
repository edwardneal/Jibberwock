using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.API.Controllers.Tenants
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : JibberwockControllerBase
    {
        public TenantController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever)
            : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        { }

        /// <summary>
        /// Gets all <see cref="Tenant"/>s which the current <see cref="User"/> has access to.
        /// </summary>
        /// <response code="200" nullable="false">An array of <see cref="Tenant"/>s accessible by the current <see cref="User"/>.</response>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Tenant>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccessibleTenants()
        {
            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            // Get every tenant associated with the user
            var getTenantsCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.GetTenantsByUser(Logger, currUser, true);
            var allTenants = await getTenantsCommand.Execute(SqlServerDataSource);

            return Ok(allTenants);
        }
    }
}
