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
    public class ProductController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Product, Permission.Read)]
        public async Task<IActionResult> GetProducts([FromQuery] bool includeHiddenProducts)
        {
            return Ok();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Change)] string id, [FromBody] object updatedProduct)
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

        [Route("{id}/plans")]
        [HttpGet]
        public async Task<IActionResult> GetProductPlans([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Read)] string id, [FromQuery] bool includeHiddenPlans)
        {
            return Ok();
        }

        [Route("{id}/plans/{planId}")]
        [HttpGet]
        public async Task<IActionResult> GetProductPlan([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Read)] string id, [FromRoute] string planId, [FromQuery] bool includeHiddenPlans)
        {
            return Ok();
        }

        [Route("{id}/plans/{planId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProductPlan([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Change)] string id, [FromRoute] string planId, [FromBody] object updatedProduct)
        {
            return Ok();
        }

        [Route("{id}/plans")]
        [HttpPost]
        public async Task<IActionResult> CreateProductPlan([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Change)] string id, [FromBody] object productPlan)
        {
            return Ok();
        }
    }
}
