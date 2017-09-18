using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.PostgreSql;

using Npgsql;

using StockWatcher.Model;
using StockWatcher.Model.Services;

namespace StockWatcher
{
    public class Startup
    {
        /// <summary>
        /// Parses connection string of the form "postgres://username:password@host:portNumber/database"
        /// into connection string of form "Username=username;Password=password;Host=host;Port=portNumber;Database=database;"
        /// </summary>
        /// <param name="rawConnectionString">
        /// Connection string of the form "postgres://username:password@host:portNumber/database"
        /// </param>
        /// <returns>
        /// Connection string of the form "Username=username;Password=password;Host=host;Port=portNumber;Database=database;"
        /// </returns>
        private string ConnectionStringParser(string rawConnectionString)
        {
            int firstIndex = rawConnectionString.IndexOf("//");
            int lastIndex = rawConnectionString.IndexOf("@");

            string usernamePassword = rawConnectionString.Substring(firstIndex + 2, lastIndex - firstIndex - 2);
            string[] tmp = usernamePassword.Split(":");
            string username = tmp[0];
            string password = tmp[1];

            firstIndex = lastIndex + 1;
            lastIndex = rawConnectionString.LastIndexOf("/");

            string hostPort = rawConnectionString.Substring(firstIndex, lastIndex - firstIndex);

            tmp = hostPort.Split(":");
            string host = tmp[0];
            int port = int.Parse(tmp[1]);

            string database = rawConnectionString.Substring(lastIndex + 1);
            
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder();
            connectionStringBuilder.Username = username;
            connectionStringBuilder.Password = password;
            connectionStringBuilder.Host = host;
            connectionStringBuilder.Port = port;
            connectionStringBuilder.Database = database;
            connectionStringBuilder.SslMode = SslMode.Prefer;
            connectionStringBuilder.TrustServerCertificate = true;

            return connectionStringBuilder.ConnectionString;
        }
        private void ServicesHelper(IServiceCollection services, string connectionString) 
        {
            services.AddMvc();
            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(connectionString)
            );

            services.AddDbContext<StockDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            services.AddTransient<StockRequestService>();
            services.AddTransient<QueryCompanyService>();
            services.AddTransient<AlphaVantageService>();
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services) 
        {
            string connectionString = Configuration.GetConnectionString("PostGresConnection");
            ServicesHelper(services, connectionString);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = ConnectionStringParser(Environment.GetEnvironmentVariable("DATABASE_URL"));
            ServicesHelper(services, connectionString);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                Console.WriteLine("Is Development");
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(
                    new WebpackDevMiddlewareOptions
                    {
                        HotModuleReplacement = true,
                        ReactHotModuleReplacement = true
                    }
                );
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");

                // routes.MapSpaFallbackRoute(
                //     name: "spa-fallback",
                //     defaults: new { controller = "Home", action = "Index" });
            });

            // app.UseHangfireDashboard();
            
            app.UseHangfireServer(
                // Heroku Postgres has max connections of 20
                new BackgroundJobServerOptions{ WorkerCount = 5}
            );
        }
    }
}
