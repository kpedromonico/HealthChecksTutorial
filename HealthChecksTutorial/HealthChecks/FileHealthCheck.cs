using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecksTutorial.HealthChecks
{
    // Implement the IHealthCheck
    public class FileHealthCheck : IHealthCheck
    {
        private readonly string _filePath;

        public FileHealthCheck(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }        

        public async Task<HealthCheckResult> CheckHealthAsync
            (HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Just check through File.Exists if the filepath has a file or not
                return await Task.FromResult(File.Exists(_filePath)
                    ? HealthCheckResult.Healthy() 
                    : HealthCheckResult.Unhealthy());
                
            }
            catch (Exception ex)
            {
                // Just in case we have a exception, return it properly in a check layout
                return new HealthCheckResult(
                    context.Registration.FailureStatus,
                    description: "Oops, something went wrong!",
                    exception: ex);
            }
        }
    }
}
