using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Core.Http.Authorization;
using Jibberwock.DataModels.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jibberwock.Admin.API.Controllers.Products
{
    [ApiController]
    [Route("[controller]")]
    public class ProductCharacteristicController : ControllerBase
    {
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
