using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.DataSources
{
    /// <summary>
    /// Configuration options which describe how SQL Server connections should behave.
    /// </summary>
    public class SqlServerDataSourceOptions : IOptions<SqlServerDataSourceOptions>
    {
        /// <summary>
        /// The connection string which should be used for read-only connections.
        /// </summary>
        public string ReadOnlyConnectionString { get; set; }

        /// <summary>
        /// The connection string which should be used for read-write connections.
        /// </summary>
        public string ReadWriteConnectionString { get; set; }

        SqlServerDataSourceOptions IOptions<SqlServerDataSourceOptions>.Value => this;
    }
}
