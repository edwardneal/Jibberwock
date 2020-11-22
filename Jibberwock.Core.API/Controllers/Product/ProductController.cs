using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.API.Controllers.Product
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : JibberwockControllerBase
    {
        public ProductController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever)
            : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        { }
    }
}
