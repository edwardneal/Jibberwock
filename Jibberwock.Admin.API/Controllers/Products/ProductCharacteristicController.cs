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
using Jibberwock.Admin.API.ActionModels.Products;
using Jibberwock.DataModels.Products;
using Jibberwock.Shared.Http;
using System.Net;

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

        /// <summary>
        /// Gets all available product characteristics.
        /// </summary>
        /// <response code="200" nullable="false">The retrieved set of <see cref="ProductCharacteristic"/> objects.</response>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductCharacteristic>), StatusCodes.Status200OK)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetProductCharacteristics()
        {
            var listCharacteristicsCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.ListAllCharacteristics(Logger);
            var characteristics = await listCharacteristicsCommand.Execute(SqlServerDataSource);

            return Ok(characteristics);
        }

        /// <summary>
        /// Updates a single product characteristic.
        /// </summary>
        /// <param name="id">The ID of the <see cref="ProductCharacteristic"/> to change.</param>
        /// <param name="updatedCharacteristic">The expected state of the identified <see cref="ProductCharacteristic"/>.</param>
        /// <response code="200" nullable="false">The updated <see cref="ProductCharacteristic"/> object.</response>
        /// <response code="400" nullable="false">Unable to update the product characteristic, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="ProductCharacteristic"/> with the provided ID does not exist.</response>
        [Route("{id:int}")]
        [HttpPut]
        [ProducesResponseType(typeof(ProductCharacteristic), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> UpdateProductCharacteristic([FromRoute] int id, [FromBody] ProductCharacteristicChangeSetting updatedCharacteristic)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }
            if (updatedCharacteristic == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var charModel = new ProductCharacteristic()
                { Id = id, Name = updatedCharacteristic.Name, Description = updatedCharacteristic.Description,
                    Enabled = updatedCharacteristic.Enabled, Visible = updatedCharacteristic.Visible };
            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var updateCharacteristicCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.UpdateCharacteristic(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null, charModel);

            var updateSuccessful = await updateCharacteristicCommand.Execute(SqlServerDataSource);

            if (updateSuccessful.Result != null)
            { return Ok(updateSuccessful.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Deletes a product characteristic.
        /// </summary>
        /// <param name="id">The ID of the <see cref="ProductCharacteristic"/> to change.</param>
        /// <response code="204" nullable="false">The <see cref="ProductCharacteristic"/> ID to delete.</response>
        /// <response code="400" nullable="false">Unable to delete the product characteristic, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="ProductCharacteristic"/> with the provided ID does not exist.</response>
        [Route("{id:int}")]
        [HttpDelete]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Delete)]
        public async Task<IActionResult> DeleteProductCharacteristic([FromRoute] int id)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var charModel = new ProductCharacteristic() { Id = id };
            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var deleteCharacteristicCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.DeleteCharacteristic(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null, charModel);

            var deleteSuccessful = await deleteCharacteristicCommand.Execute(SqlServerDataSource);

            if (deleteSuccessful.Result)
            { return StatusCode(StatusCodes.Status204NoContent); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Creates a single product characteristic.
        /// </summary>
        /// <param name="characteristic">The expected state of the identified <see cref="ProductCharacteristic"/>.</param>
        /// <response code="201" nullable="false">The <see cref="ProductCharacteristic"/> object to create.</response>
        /// <response code="400" nullable="false">Unable to create the product characteristic, see response for details.</response>
        [Route("")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> CreateCharacteristic([FromBody] ProductCharacteristicChangeSetting characteristic)
        {
            if (characteristic == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var charModel = new ProductCharacteristic()
            {
                Name = characteristic.Name,
                Description = characteristic.Description,
                Enabled = characteristic.Enabled,
                Visible = characteristic.Visible
            };
            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var createCharacteristicCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.CreateCharacteristic(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null, charModel);

            var creationSuccessful = await createCharacteristicCommand.Execute(SqlServerDataSource);

            return Created(string.Empty, creationSuccessful.Result);
        }
    }
}
