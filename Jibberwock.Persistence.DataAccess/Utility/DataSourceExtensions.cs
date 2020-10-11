using Jibberwock.Persistence.DataAccess.DataSources;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Utility
{
    /// <summary>
    /// Contains a set of extensions which allow the abstraction of a <see cref="IReadableDataSource"/> and <see cref="IReadWriteDataSource"/> to leak safely.
    /// </summary>
    internal static class DataSourceExtensions
    {
        /// <summary>
        /// Gets the correct database connection from this <see cref="IReadableDataSource"/>.
        /// </summary>
        /// <param name="dataSource">The <see cref="IReadableDataSource"/> to get the database connection from.</param>
        /// <returns>The associated database connection.</returns>
        public static async Task<IDbConnection> GetDbConnection(this IReadableDataSource dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource));

            if (dataSource is SqlServerDataSource sqlServerDataSource)
                return await sqlServerDataSource.GetReadOnlyDatabaseConnection();

            throw new InvalidCastException($"{nameof(dataSource)} is not an expected type");
        }

        /// <summary>
        /// Gets the correct database connection from this <see cref="IReadWriteDataSource"/>.
        /// </summary>
        /// <param name="dataSource">The <see cref="IReadWriteDataSource"/> to get the database connection from.</param>
        /// <returns>The associated database connection.</returns>
        public static async Task<IDbConnection> GetDbConnection(this IReadWriteDataSource dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource));

            if (dataSource is SqlServerDataSource sqlServerDataSource)
                return await sqlServerDataSource.GetReadWriteDatabaseConnection();

            throw new InvalidCastException($"{nameof(dataSource)} is not an expected type");
        }

        /// <summary>
        /// Gets the correct queue client from this <see cref="IQueueDataSource"/>.
        /// </summary>
        /// <param name="dataSource">The <see cref="IQueueDataSource"/> to get the queue client from.</param>
        /// <param name="queueName">The queue to get the queue client for.</param>
        /// <returns>The associated queue client.</returns>
        public static QueueClient GetQueueClient(this IQueueDataSource dataSource, string queueName)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource));

            if (dataSource is ServiceBusQueueDataSource serviceBusDataSource)
                return serviceBusDataSource.GetQueueClient(queueName);

            throw new InvalidCastException($"{nameof(dataSource)} is not an expected type");
        }
    }
}
