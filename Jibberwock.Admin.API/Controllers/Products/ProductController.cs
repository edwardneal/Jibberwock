using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> GetProducts([FromQuery] bool includeHiddenProducts)
        {
            return Ok();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromRoute] string id, [FromBody] object updatedProduct)
        {
            return Ok();
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] object product)
        {
            return Ok();
        }

        [Route("{id}/plans")]
        [HttpGet]
        public async Task<IActionResult> GetProductPlans([FromRoute] string id, [FromQuery] bool includeHiddenPlans)
        {
            return Ok();
        }

        [Route("{id}/plans/{planId}")]
        [HttpGet]
        public async Task<IActionResult> GetProductPlan([FromRoute] string id, [FromRoute] string planId, [FromQuery] bool includeHiddenPlans)
        {
            return Ok();
        }

        [Route("{id}/plans/{planId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProductPlan([FromRoute] string id, [FromRoute] string planId, [FromBody] object updatedProduct)
        {
            return Ok();
        }

        [Route("{id}/plans")]
        [HttpPost]
        public async Task<IActionResult> CreateProductPlan([FromRoute] string id, [FromBody] object productPlan)
        {
            return Ok();
        }
    }
}
