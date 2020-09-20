using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.DataSources
{
    /// <summary>
    /// A data source which is both readable and writable.
    /// </summary>
    /// <remarks>
    /// A client class can implement both <see cref="IReadWriteDataSource"/> and <see cref="IReadableDataSource"/>, and handle read-only and read-write commands differently.
    /// </remarks>
    public interface IReadWriteDataSource : IDataSource
    {
        /// <summary>
        /// Connects this data access component to the underlying data source.
        /// </summary>
        /// <remarks>
        /// Most connections will need to remain open (such as SQL Server.) If a data source isn't connection-based though, this method will no-op.
        /// </remarks>
        new Task OpenAsync();
    }
}
