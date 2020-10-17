using Jibberwock.Persistence.DataAccess.DependencyInjection;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Cryptography;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Core.Background
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();
            var readOnlyConnectionString = configuration.GetConnectionString("ReadOnlySqlServer");
            var readWriteConnectionString = configuration.GetConnectionString("ReadWriteSqlServer");

            builder.Services.Configure<WebApiConfiguration>(configuration.GetSection("Configuration"));
            builder.Services.Configure<Jibberwock.Persistence.DataAccess.DataSources.SqlServerDataSourceOptions>(opt =>
            {
                opt.ReadOnlyConnectionString = readOnlyConnectionString;
                opt.ReadWriteConnectionString = readWriteConnectionString;
            });

            builder.Services.AddJibberwockPersistence()
                .AddJibberwockCryptography();
        }
    }
}
