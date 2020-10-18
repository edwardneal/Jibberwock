using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jibberwock.Admin.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .ConfigureAppConfiguration((webHostCtx, iConfigBuilder) =>
                        {
                            var config = iConfigBuilder.Build();
                            var keyVaultName = config.GetValue<string>("Configuration:SensitiveSettingKeyVaultName", null);

                            // Quick sanity check, protecting against obscure errors if the configuration setting isn't present
                            if (!string.IsNullOrEmpty(keyVaultName))
                                iConfigBuilder.AddAzureKeyVault($"https://{keyVaultName}.vault.azure.net/");
                        });
                });
    }
}
