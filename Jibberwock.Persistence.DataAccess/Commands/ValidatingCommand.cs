using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands
{
    /// <summary>
    /// Base class for all further commands. Implements <see cref="ICommand{TResult, TDataSource}"/>, with validation logic as required.
    /// </summary>
    /// <typeparam name="TResult">The return value of the command.</typeparam>
    /// <typeparam name="TDataSource">The type of data source (readable or read-write) to execute against.</typeparam>
    public abstract class ValidatingCommand<TResult, TDataSource> : ICommand<TResult, TDataSource>
        where TDataSource : DataSources.IDataSource
    {
        /// <summary>
        /// Use this property to write logs as a command executes.
        /// </summary>
        protected ILogger Logger { get; private set; }

        protected ValidatingCommand(ILogger logger)
        {
            Logger = logger;
        }

        protected abstract Task<TResult> OnExecute(TDataSource dataSource);

        /// <summary>
        /// Executes this command against a data source, validating any parameters on the command.
        /// </summary>
        /// <param name="dataSource">The data source to execute the command on.</param>
        /// <returns>The result of executing the command.</returns>
        public Task<TResult> Execute(TDataSource dataSource)
        {
            Validator.ValidateObject(this, new ValidationContext(this), true);

            return OnExecute(dataSource);
        }
    }
}
