using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Shared.Http.Controllers
{
    /// <summary>
    /// Base class for all Jibberwock API controllers. Provides easy access to the SQL Server database.
    /// </summary>
    public abstract class JibberwockControllerBase : ControllerBase
    {
        /// <summary>
        /// The SQL Server data source.
        /// </summary>
        protected SqlServerDataSource SqlServerDataSource { get; private set; }

        /// <summary>
        /// Provides access to create new <see cref="ILogger{TCategoryName}"/> instances.
        /// </summary>
        protected ILoggerFactory LoggerFactory { get; private set; }

        /// <summary>
        /// The <see cref="ILogger"/> to use for logging data.
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// The web API's configuration.
        /// </summary>
        protected WebApiConfiguration WebApiConfiguration { get; private set; }

        /// <summary>
        /// A class capable of returning the currently logged-in user.
        /// </summary>
        protected ICurrentUserRetriever CurrentUserRetriever { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="JibberwockControllerBase"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="sqlServerDataSource">The SQL Server data source.</param>
        /// <param name="options">The web API configuration.</param>
        protected JibberwockControllerBase(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource, IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever)
            : base()
        {
            SqlServerDataSource = sqlServerDataSource;
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(GetType());
            WebApiConfiguration = options.Value;
            CurrentUserRetriever = currentUserRetriever;
        }
    }
}
