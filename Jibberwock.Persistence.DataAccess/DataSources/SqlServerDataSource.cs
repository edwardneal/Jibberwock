using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Azure.Services.AppAuthentication;

namespace Jibberwock.Persistence.DataAccess.DataSources
{
    /// <summary>
    /// This is a SQL Server data source, capable of being used for read-only and read-write commands.
    /// </summary>
    public sealed class SqlServerDataSource : IReadableDataSource, IReadWriteDataSource
    {
        private const string SqlServerResourceName = "https://database.windows.net/";
        private readonly AzureServiceTokenProvider _tokenProvider;
        private readonly SqlConnection _readOnlyConnection;
        private readonly SqlConnection _readWriteConnection;

        internal async Task<IDbConnection> GetReadOnlyDatabaseConnection()
        {
            await ((IReadableDataSource)this).OpenAsync();

            return _readOnlyConnection;
        }

        internal async Task<IDbConnection> GetReadWriteDatabaseConnection()
        {
            await ((IReadWriteDataSource)this).OpenAsync();
            return _readWriteConnection;
        }

        public object ConnectionRetryPolicy => throw new NotImplementedException();

        /// <summary>
        /// Configuration options which describe how this SQL Server data source's connections should behave.
        /// </summary>
        public SqlServerDataSourceOptions DataSourceOptions { get; private set; }

        public SqlServerDataSource(IOptions<SqlServerDataSourceOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.Value == null)
                throw new ArgumentNullException(nameof(options));

            DataSourceOptions = options.Value;

            _tokenProvider = new AzureServiceTokenProvider();
            _readOnlyConnection = new SqlConnection(DataSourceOptions.ReadOnlyConnectionString);
            _readWriteConnection = new SqlConnection(DataSourceOptions.ReadWriteConnectionString);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_readOnlyConnection != null)
                _readOnlyConnection.Dispose();
            if (_readWriteConnection != null)
                _readWriteConnection.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_readOnlyConnection != null)
                await _readOnlyConnection.DisposeAsync();

            if (_readWriteConnection != null)
                await _readWriteConnection.DisposeAsync();
        }

        private async Task checkSqlConnectionAzureToken(SqlConnection conn)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));

            // We can only set the access token when the connection is closed. If it's open, we've
            // already got access (and thus don't need the token)
            if (conn.State == ConnectionState.Broken
                || conn.State == ConnectionState.Closed)
            {
                var csb = new SqlConnectionStringBuilder(conn.ConnectionString);

                // If the SQL database being connected to is running in Azure, make sure that we're connecting
                // with an access token. Get this access token from Azure for the current user (the service identity
                // in Azure, or the Visual Studio identity for local dev)
                if (csb.DataSource.Contains(".database.windows.net", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(conn.AccessToken))
                    {
                        conn.AccessToken = await _tokenProvider.GetAccessTokenAsync(SqlServerResourceName);
                    }
                }
            }
        }

        async Task IReadableDataSource.OpenAsync()
        {
            await checkSqlConnectionAzureToken(_readOnlyConnection);

            if (_readOnlyConnection.State == ConnectionState.Broken
                || _readOnlyConnection.State == ConnectionState.Closed)
            {
                await _readOnlyConnection.OpenAsync();
            }
        }

        async Task IReadWriteDataSource.OpenAsync()
        {
            await checkSqlConnectionAzureToken(_readWriteConnection);

            if (_readWriteConnection.State == ConnectionState.Broken
                || _readWriteConnection.State == ConnectionState.Closed)
            {
                await _readWriteConnection.OpenAsync();
            }
        }

        Task IDataSource.OpenAsync()
        {
            throw new NotImplementedException($"Access this class by casting to an {nameof(IReadableDataSource)} or an {nameof(IReadWriteDataSource)}.");
        }
    }
}
