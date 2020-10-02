using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace HealthChecksTutorial.HealthChecks
{
    public static class HealthCheckConfig
    {
        // Common middleware approach
        public static IApplicationBuilder UseMyHealthChecks(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseHealthChecks("/health", new HealthCheckOptions {
                ResponseWriter = ResponseWriter
            });

            // Here I'm filtering only the database tags
            applicationBuilder.UseHealthChecks("/dbhealth", new HealthCheckOptions
            {
                ResponseWriter = ResponseWriter,
                Predicate = (x) => x.Tags.Contains("database") // Filter function
            });

            return applicationBuilder;
        }

        // Endpoint apprach
        public static IEndpointConventionBuilder HealthChecksMapper(this IEndpointRouteBuilder endpointsBuilder)
        {
            return endpointsBuilder.MapHealthChecks("/mhealth", new HealthCheckOptions
            {
                ResponseWriter = ResponseWriter
            });
        }

        private static Task ResponseWriter(HttpContext context, HealthReport report)
        {
            var entries = report.Entries.Select(x => new
            {
                name = x.Key,
                description = x.Value.Description,
                duration = x.Value.Duration,
                status = Enum.GetName(typeof(HealthStatus), x.Value.Status),
                error = x.Value.Exception?.Message,
                tags = x.Value.Tags
            }).ToList();

            var json = JsonConvert.SerializeObject(entries);

            return context.Response.WriteAsync(json);
        }
    }
}


