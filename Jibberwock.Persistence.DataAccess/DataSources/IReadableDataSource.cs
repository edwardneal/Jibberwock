using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.DataSources
{
    /// <summary>
    /// A data source which is readable, but not necessarily writable.
    /// </summary>
    /// <remarks>
    /// One example would be SQL Server, with ApplicationIntent configured correctly in the connection string.
    /// </remarks>
    public interface IReadableDataSource : IDataSource
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
