using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Shared.Http.Authorization;
using Jibberwock.DataModels.Security;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http.Authentication;

namespace Jibberwock.Admin.API.Controllers.Products
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProductController : JibberwockControllerBase
    {
        public ProductController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever) : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        { }

        [Route("")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Product, Permission.Read)]
        public async Task<IActionResult> GetProducts([FromQuery] bool includeHiddenProducts)
        {
            return Ok();
        }

        [Route("{id:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Change)] long id, [FromBody] object updatedProduct)
        {
            return Ok();
        }

        [Route("")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Product, Permission.CreateProduct)]
        public async Task<IActionResult> CreateProduct([FromBody] object product)
        {
            return Ok();
        }

        [Route("{id:int}/plans")]
        [HttpGet]
        public async Task<IActionResult> GetProductPlans([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Read)] long id, [FromQuery] bool includeHiddenPlans)
        {
            return Ok();
        }

        [Route("{id:int}/plans/{planId:int}")]
        [HttpGet]
        public async Task<IActionResult> GetProductPlan([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Read)] long id, [FromRoute] long planId, [FromQuery] bool includeHiddenPlans)
        {
            return Ok();
        }

        [Route("{id:int}/plans/{planId:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProductPlan([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Change)] long id, [FromRoute] long planId, [FromBody] object updatedProduct)
        {
            return Ok();
        }

        [Route("{id:int}/plans")]
        [HttpPost]
        public async Task<IActionResult> CreateProductPlan([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Change)] long id, [FromBody] object productPlan)
        {
            return Ok();
        }
    }
}
