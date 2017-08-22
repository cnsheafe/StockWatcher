using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.PostgreSql;

using StockWatcher.Model;
using StockWatcher.Model.Services;

namespace StockWatcher
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection"))
            );
            services.AddDbContext<StockDbContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable("SWConnString"))
            );

            services.AddTransient<ManageUser>();
            services.AddTransient<SmsService>();
            services.AddTransient<StockRequestService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc(routes => {
                routes.MapRoute(
                    "default",
                    "{controller=notifications}/{action=get}"
                );
            });

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
