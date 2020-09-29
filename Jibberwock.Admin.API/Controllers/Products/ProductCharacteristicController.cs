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
using Jibberwock.Shared.Configuration;
using Microsoft.Extensions.Options;
using Jibberwock.Shared.Http.Authentication;

namespace Jibberwock.Admin.API.Controllers.Products
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProductCharacteristicController : JibberwockControllerBase
    {
        public ProductCharacteristicController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever) : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        { }

        [Route("")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetProductCharacteristics()
        {
            return Ok();
        }

        [Route("{id}")]
        [HttpPut]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> UpdateProductCharacteristic([FromRoute] string id, [FromBody] object updatedCharacteristic)
        {
            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Delete)]
        public async Task<IActionResult> DeleteProductCharacteristic([FromRoute] string id)
        {
            return Ok();
        }

        [Route("")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> CreateCharacteristic([FromBody] object characteristic)
        {
            return Ok();
        }
    }
}
