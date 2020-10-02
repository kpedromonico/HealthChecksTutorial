using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthChecksTutorial.Data;
using HealthChecksTutorial.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace HealthChecksTutorial
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var x = Configuration.GetConnectionString("DefaultConnection");            

            // Inject the db context
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Don't forget to DI the health checks service
            // Also, include the tags so you're able to filter!
            services.AddHealthChecks()
                .AddDbContextCheck<DataContext>("Database Check",
                    tags: new string[] { "database" })
                .AddUrlGroup(new Uri("https://www.kpedromonico.com"),
                    name: "KaiosWebSite",
                    tags: new List<string> { "http" })
                // Our new custom FileHealthCheck
                .AddCheck("fileHealthCheck",
                    //  Here is the class instance, passing to the constructor
                    instance: new FileHealthCheck(@"/Users/kpedromonico/Documents/i_exist.txt"),
                    tags: new string[] { "file" });              

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Our new call to the health checks middleware
            app.UseMyHealthChecks();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });

                // Calling the endpoint approach - It's suggested on Microsoft's website
                endpoints.HealthChecksMapper();
            });
        }
    }
}


