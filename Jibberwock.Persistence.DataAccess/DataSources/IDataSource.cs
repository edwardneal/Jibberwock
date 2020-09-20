using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.DataSources
{
    /// <summary>
    /// The base interface for all data access components which retrieve models from the corresponding data sources.
    /// </summary>
    public interface IDataSource : IDisposable, IAsyncDisposable
    {
        object ConnectionRetryPolicy { get; }

        /// <summary>
        /// Connects this data access component to the underlying data source.
        /// </summary>
        /// <remarks>
        /// Most connections will need to remain open (such as SQL Server.) If a data source isn't connection-based though, this method will no-op.
        /// </remarks>
        Task OpenAsync();
    }
}
