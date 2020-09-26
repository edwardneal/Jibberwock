using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.DependencyInjection;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.Shared.Http.Middleware;
using MaximeRouiller.Azure.AppService.EasyAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jibberwock.Admin.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public WebApiConfiguration StronglyTypedConfiguration
        {
            get
            {
                var config = new WebApiConfiguration();
                Configuration.Bind("Configuration", config);
                return config;
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<WebApiConfiguration>(Configuration.GetSection("Configuration"));
            services.Configure<SqlServerDataSourceOptions>(o =>
                {
                    o.ReadOnlyConnectionString = Configuration?.GetConnectionString("ReadOnlySqlServer");
                    o.ReadWriteConnectionString = Configuration?.GetConnectionString("ReadWriteSqlServer");
                })
                .AddJibberwockPersistence();

            services.AddHttpContextAccessor();

            services.AddAuthentication("EasyAuth")
                .AddEasyAuthAuthentication(o => { });
            services.AddJibberwockSecurity();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseMiddleware<EasyAuthDebugMiddleware>(StronglyTypedConfiguration.EasyAuth);
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseAuthentication()
                .UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}