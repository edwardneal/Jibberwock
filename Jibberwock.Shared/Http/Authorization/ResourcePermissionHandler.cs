using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Shared.Http.Authorization
{
    /// <summary>
    /// References the <see cref="ResourcePermissionsAttribute"/>s on the target parameters and the current user
    /// in order to make permission decisions.
    /// </summary>
    public class ResourcePermissionHandler : IAuthorizationHandler
    {
        private readonly HttpContext _httpContext;
        private readonly ILogger<ResourcePermissionHandler> _logger;
        private readonly WebApiConfiguration _configuration;
        private readonly ICurrentUserRetriever _currentUserRetriever;
        private readonly SqlServerDataSource _dataSource;

        public ResourcePermissionHandler(IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever, SqlServerDataSource dataSource)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _logger = loggerFactory?.CreateLogger<ResourcePermissionHandler>();
            _configuration = options?.Value;
            _currentUserRetriever = currentUserRetriever;
            _dataSource = dataSource;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            _logger.LogDebug($"Processing authorization handler for path \"{_httpContext?.Request?.Path ?? "(unknown)"}\".");

            var endpoint = context.Resource as RouteEndpoint;
            var controllerContext = endpoint.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();

            var resourcePermissionChecks = new List<Jibberwock.Persistence.DataAccess.Commands.Security.ResourcePermissionCheck>();

            // First, look for any [ResourcePermissions] tags on the method. If these are there, it's because there's
            // no fixed ID to work with. The attribute will have a ResourceType of Service, and we should use the
            // ID specified in the configuration.
            var actionAttributes = controllerContext.MethodInfo.GetCustomAttributes(typeof(ResourcePermissionsAttribute), true)
                .OfType<ResourcePermissionsAttribute>();

            foreach (var rpa in actionAttributes)
            {
                if (rpa.ResourceType != DataModels.Security.SecurableResourceType.Service)
                {
                    _logger.LogWarning($"When processing this route, discovered a ResourcePermissions attribute on the corresponding method which did not have a type of Service. This attribute will be ignored.");
                }
                else
                {
                    resourcePermissionChecks.Add(new Persistence.DataAccess.Commands.Security.ResourcePermissionCheck()
                    {
                        PermissionsRequired = rpa.PermissionsRequired,
                        ResourceId = _configuration.Authorization.DefaultServiceId,
                        ResourceType = rpa.ResourceType
                    });
                }
            }

            // Next, look on the method parameters. If there's a [ResourcePermissions] attribute on here, we know that
            // this parameter stores the ID. Add them to the list wholesale, there are no restrictions here.
            var parameterAttributes = from param in controllerContext.Parameters.OfType<ControllerParameterDescriptor>()
                                      let attrs = param.ParameterInfo.GetCustomAttributes(typeof(ResourcePermissionsAttribute), true)
                                        .OfType<ResourcePermissionsAttribute>()
                                      from attr in attrs
                                      select new
                                      { ResourceId = Convert.ChangeType(_httpContext.GetRouteValue(param.Name), param.ParameterType), PermissionsAttribute = attr };
            
            foreach (var rpa in parameterAttributes)
            {
                resourcePermissionChecks.Add(new Persistence.DataAccess.Commands.Security.ResourcePermissionCheck()
                {
                    PermissionsRequired = rpa.PermissionsAttribute.PermissionsRequired,
                    ResourceId = (long)rpa.ResourceId,
                    ResourceType = rpa.PermissionsAttribute.ResourceType
                });
            }

            // If we've got permission checks to run, get the current user and run them.
            if (resourcePermissionChecks.Any())
            {
                _logger.LogDebug($"Performing resource permission checks for this route.");

                var currentUser = await _currentUserRetriever.GetCurrentUserAsync();
                var checkPermissionsCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.CheckPermissions(_logger, currentUser, resourcePermissionChecks);

                if (currentUser == null)
                {
                    _logger.LogInformation("User is not currently logged in.");
                    context.Fail();
                    return;
                }

                var isAccessValid = await checkPermissionsCommand.Execute(_dataSource);

                // If the user fails their permission checks, block access to the resource.
                if (!isAccessValid)
                {
                    _logger.LogInformation("Jibberwock permissions checks failed for this user on this route.");
                    context.Fail();
                    return;
                }
                else
                {
                    _logger.LogInformation("Jibberwock permissions checks passed for this user on this route.");
                }
            }

            foreach (var req in context.Requirements.OfType<IAuthorizationHandler>())
                await req.HandleAsync(context);
        }
    }
}
