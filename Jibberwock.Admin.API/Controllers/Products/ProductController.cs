using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Shared.Http.Authorization;
using Jibberwock.DataModels.Products;
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
using Jibberwock.Admin.API.ActionModels.Products;
using Jibberwock.Shared.Http;

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

        /// <summary>
        /// Gets all available products.
        /// </summary>
        /// <param name="includeHiddenProducts">If set to <c>true</c>, includes products with a Visible property of <c>false</c>.</param>
        /// <response code="200" nullable="false">The retrieved set of <see cref="Product"/> objects.</response>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        [ResourcePermissions(SecurableResourceType.Product, Permission.Read)]
        public async Task<IActionResult> GetProducts([FromQuery] bool includeHiddenProducts)
        {
            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var listProductsCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.ListProducts(Logger, currentUser, includeHiddenProducts);

            var products = await listProductsCommand.Execute(SqlServerDataSource);

            return Ok(products);
        }

        /// <summary>
        /// Updates a single product.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="updatedProduct">The desired state of the product.</param>
        /// <response code="200" nullable="false">The <see cref="Product"/> object updated.</response>
        /// <response code="400" nullable="false">Unable to update the product, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="Product"/> with the provided ID does not exist.</response>
        [Route("{id:int}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Change)] long id, [FromBody] ProductCreationOptions updatedProduct)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }
            if (updatedProduct == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var prodModel = new Product()
            {
                Id = id,
                Name = updatedProduct.Name,
                Description = updatedProduct.Description,
                MoreInformationUrl = updatedProduct.MoreInformationUrl,
                Visible = updatedProduct.Visible,
                ApplicableCharacteristics = updatedProduct.ApplicableCharacteristicIDs.Select(x => new ProductCharacteristic() { Id = x })
            };
            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var updateProductCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.UpdateProduct(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null, prodModel);

            var updateSuccessful = await updateProductCommand.Execute(SqlServerDataSource);

            if (updateSuccessful.Result != null)
            { return Ok(updateSuccessful.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Creates a single product.
        /// </summary>
        /// <param name="product">The expected state of the identified <see cref="Product"/>.</param>
        /// <response code="201" nullable="false">The <see cref="Product"/> object created.</response>
        /// <response code="400" nullable="false">Unable to create the product, see response for details.</response>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.CreateProduct)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreationOptions product)
        {
            if (product == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            product.ApplicableCharacteristicIDs = product.ApplicableCharacteristicIDs ?? Enumerable.Empty<int>();

            var productModel = new Jibberwock.DataModels.Products.Product()
            {
                Name = product.Name,
                Description = product.Description,
                MoreInformationUrl = product.MoreInformationUrl,
                Visible = product.Visible,
                ApplicableCharacteristics = product.ApplicableCharacteristicIDs?.Select(i => new Jibberwock.DataModels.Products.ProductCharacteristic() { Id = i }).ToArray()
            };

            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var createProductCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.CreateProduct(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null, productModel);

            var creationSuccessful = await createProductCommand.Execute(SqlServerDataSource);

            return Created(string.Empty, creationSuccessful.Result);
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
