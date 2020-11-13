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
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Jibberwock.Admin.API.Controllers.Products
{
    [ApiController]
    [Route("api/[controller]")]
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
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
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
        /// <response code="200" nullable="false">The updated <see cref="Product"/> object.</response>
        /// <response code="400" nullable="false">Unable to update the product, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="Product"/> with the provided ID does not exist.</response>
        [Route("{id:int}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.CreateProduct)]
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
        /// <response code="201" nullable="false">The created <see cref="Product"/> object.</response>
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

            product.ApplicableCharacteristicIDs ??= Enumerable.Empty<int>();

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

        /// <summary>
        /// Gets all tiers for a specified product.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <param name="includeHiddenTiers">If set to <c>true</c>, includes tiers with a Visible property of <c>false</c>.</param>
        /// <response code="200" nullable="false">The retrieved set of <see cref="Tier"/> objects.</response>
        /// <response code="400" nullable="false">Unable to get the list of tiers, see response for details.</response>
        [Route("{id:int}/tiers")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Tier>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetProductTiers([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Read)] long id, [FromQuery] bool includeHiddenTiers)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var productModel = new Product() { Id = id };
            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var listTiersCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.ListTiers(Logger, currentUser, includeHiddenTiers, productModel);
            var tiers = await listTiersCommand.Execute(SqlServerDataSource);

            return Ok(tiers);
        }

        /// <summary>
        /// Gets a specific tier and its characteristics for a specified product.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <param name="planId">The ID of the tier</param>
        /// <response code="200" nullable="false">The retrieved <see cref="Tier"/> object.</response>
        /// <response code="400" nullable="false">Unable to get the list of tiers, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="Product"/> or <see cref="Tier"/> with the provided ID does not exist.</response>
        [Route("{id:int}/tiers/{tierId:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(Tier), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetSingleProductTier([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Read)] long id, [FromRoute] long tierId)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }
            if (tierId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var getTierCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.GetTier(Logger, currentUser, new Tier() { Id = tierId, Product = new Product() { Id = id } });
            var tier = await getTierCommand.Execute(SqlServerDataSource);

            if (tier == null)
            { return NotFound(); }
            else
            { return Ok(tier); }
        }

        /// <summary>
        /// Updates the details of a specific tier and its characteristics for a specified product.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <param name="planId">The ID of the tier</param>
        /// <response code="200" nullable="false">The updated <see cref="Tier"/> object.</response>
        /// <response code="400" nullable="false">Unable to update the tier, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="Product"/> or <see cref="Tier"/> with the provided ID does not exist.</response>
        [Route("{id:int}/tiers/{tierId:int}")]
        [HttpPut]
        [ProducesResponseType(typeof(Tier), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.CreateProduct)]
        public async Task<IActionResult> UpdateProductTier([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Change)] long id, [FromRoute] long tierId, [FromBody] TierCreationOptions updatedTier)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }
            if (tierId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var suppliedCharacteristicValues = await parseProductCharacteristicsAsync(id, updatedTier.CharacteristicValues);

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var tierModel = new Tier()
            {
                Id = tierId,
                Name = updatedTier.Name,
                StartDate = updatedTier.StartDate,
                EndDate = updatedTier.EndDate,
                Product = new Product() { Id = id },
                Visible = updatedTier.Visible,
                Characteristics = suppliedCharacteristicValues
            };

            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            // Update the tier, then return it to the clients
            var updateTierCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.UpdateTier(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null, tierModel);
            var auditedTierCreation = await updateTierCommand.Execute(SqlServerDataSource);

            if (auditedTierCreation.Result != null)
            { return Ok(auditedTierCreation.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Creates a single product tier.
        /// </summary>
        /// <param name="id">The ID of the product to create the tier for.</param>
        /// <param name="productTier">The details of the tier to create.</param>
        /// <response code="201" nullable="false">The created <see cref="Tier"/> object.</response>
        /// <response code="400" nullable="false">Unable to create the tier, see response for details.</response>
        [Route("{id:int}/tiers")]
        [HttpPost]
        [ProducesResponseType(typeof(Tier), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.CreateProduct)]
        public async Task<IActionResult> CreateProductTier([FromRoute, ResourcePermissions(SecurableResourceType.Product, Permission.Change)] long id, [FromBody] TierCreationOptions productTier)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }
            if (productTier == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            // This is a three-step operation (because the tier characteristic values need to be interpreted)
            // First, get the list of characteristics (and their associated data types)
            // Second, perform the conversion from a string/object into a usable form
            // Third, perform the upload
            var suppliedCharacteristicValues = await parseProductCharacteristicsAsync(id, productTier.CharacteristicValues);

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var tierModel = new Tier()
            {
                Name = productTier.Name,
                ExternalId = productTier.ExternalId,
                StartDate = productTier.StartDate,
                EndDate = productTier.EndDate,
                Product = new Product() { Id = id },
                Visible = productTier.Visible,
                Characteristics = suppliedCharacteristicValues
            };

            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            // Create the tier, then pass the details along to a GetTierById command to populate the full details
            var createTierCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.CreateTier(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null, tierModel);
            var auditedTierCreation = await createTierCommand.Execute(SqlServerDataSource);

            var getCreatedTierCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.GetTier(Logger, currentUser, auditedTierCreation.Result);
            var resultantTier = await getCreatedTierCommand.Execute(SqlServerDataSource);

            return Created(string.Empty, resultantTier);
        }

        private async Task<IEnumerable<TierProductCharacteristic>> parseProductCharacteristicsAsync(long productId, IEnumerable<TierCharacteristicValue> characteristicValues)
        {
            var suppliedCharacteristicValues = new List<TierProductCharacteristic>();

            if (characteristicValues != null && characteristicValues.Any())
            {
                // Get the characteristics, filtering to Enabled ones for sanity's sake.
                // Focus on the global characteristics list, then the product's applicable characteristics
                var listCharacteristicsCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.ListAllCharacteristics(Logger);
                var globalCharacteristicsList = await listCharacteristicsCommand.Execute(SqlServerDataSource);
                var globalCharacteristics = globalCharacteristicsList.Where(c => c.Enabled).ToDictionary(ch => ch.Id);

                var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
                var getProductCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.GetById(Logger, currentUser, new Product() { Id = productId });
                var productDetails = await getProductCommand.Execute(SqlServerDataSource);
                var productCharacteristics = productDetails.ApplicableCharacteristics.Where(c => c.Enabled).ToDictionary(ch => ch.Id);

                foreach (var charValue in characteristicValues)
                {
                    // Discard any supplied characteristics which don't exist in the lists
                    if (!globalCharacteristics.ContainsKey(charValue.CharacteristicId) | !productCharacteristics.ContainsKey(charValue.CharacteristicId))
                    {
                        ModelState.AddModelError(ErrorResponses.InvalidCharacteristic, $"{{ \"id\": {charValue.CharacteristicId} }}");
                        continue;
                    }

                    var prodChar = productCharacteristics[charValue.CharacteristicId];
                    var resultantTpc = new TierProductCharacteristic()
                    {
                        ProductCharacteristic = prodChar
                    };

                    // Handle the translation, converting from strings to booleans, longs or strings
                    switch (prodChar.ValueType)
                    {
                        case ProductCharacteristicValueType.Boolean:
                            if (!bool.TryParse(charValue.Value, out var parsedBool))
                            {
                                ModelState.AddModelError(ErrorResponses.InvalidCharacteristicValue, $"{{ \"id\": {charValue.CharacteristicId}, \"type\": {(int)prodChar.ValueType} }}");
                                continue;
                            }

                            resultantTpc.CharacteristicValue = parsedBool;
                            break;
                        case ProductCharacteristicValueType.Numeric:
                            if (!long.TryParse(charValue.Value, out var parsedLong))
                            {
                                ModelState.AddModelError(ErrorResponses.InvalidCharacteristicValue, $"{{ \"id\": {charValue.CharacteristicId}, \"type\": {(int)prodChar.ValueType} }}");
                                continue;
                            }

                            resultantTpc.CharacteristicValue = parsedLong;
                            break;
                        case ProductCharacteristicValueType.String:
                            resultantTpc.CharacteristicValue = charValue.Value;
                            break;
                    }

                    suppliedCharacteristicValues.Add(resultantTpc);
                }
            }

            return suppliedCharacteristicValues;
        }
    }
}
