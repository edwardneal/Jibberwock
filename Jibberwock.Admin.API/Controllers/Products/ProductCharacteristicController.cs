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
    public class ProductCharacteristicController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetProductCharacteristics()
        {
            return Ok();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProductCharacteristic([FromRoute] string id, [FromBody] object updatedCharacteristic)
        {
            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProductCharacteristic([FromRoute] string id)
        {
            return Ok();
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateCharacteristic([FromBody] object characteristic)
        {
            return Ok();
        }
    }
}
