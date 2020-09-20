using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands
{
    /// <summary>
    /// A command which executes a single command against the underlying data source.
    /// </summary>
    /// <typeparam name="TResult">The return value of the command.</typeparam>
    /// <typeparam name="TDataSource">The type of data source (readable or read-write) to execute against.</typeparam>
    public interface ICommand<TResult, TDataSource> where TDataSource : DataSources.IDataSource
    {
        /// <summary>
        /// Executes this command against a data source.
        /// </summary>
        /// <param name="dataSource">The data source to execute the command on.</param>
        /// <returns>The result of executing the command.</returns>
        Task<TResult> Execute(TDataSource dataSource);
    }
}
