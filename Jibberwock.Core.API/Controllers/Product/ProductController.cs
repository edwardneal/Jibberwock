using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.API.Controllers.Product
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : JibberwockControllerBase
    {
        public ProductController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever)
            : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        { }

        /// <summary>
        /// Gets all <see cref="Jibberwock.DataModels.Products.Product"/>s which can be signed up for.
        /// </summary>
        /// <response code="200" nullable="false">An array of <see cref="Jibberwock.DataModels.Products.Product"/> containing all available registerable products.</response>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Jibberwock.DataModels.Products.Product>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts()
        {
            var listProductCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.ListPublicProducts(Logger);
            var products = await listProductCommand.Execute(SqlServerDataSource);

            return Ok(products);
        }
    }
}
