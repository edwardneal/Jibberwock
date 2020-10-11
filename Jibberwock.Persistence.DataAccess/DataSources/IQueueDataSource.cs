using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.DataSources
{
    /// <summary>
    /// A data source which is readable and writable, acting as a queue.
    /// </summary>
    public interface IQueueDataSource : IReadWriteDataSource
    {
    }
}
