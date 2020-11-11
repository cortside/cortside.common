using System;
using System.Threading.Tasks;
using Cortside.Common.Health.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Health.Checks {
    public class DbContextCheck : Check {
        private readonly IServiceProvider serviceProvider;

        public DbContextCheck(CheckConfiguration check, IMemoryCache cache, ILogger<Check> logger, IServiceProvider serviceProvider, IAvailabilityRecorder recorder) : base(check, cache, logger, recorder) {
            this.serviceProvider = serviceProvider;
        }

        public override async Task<ServiceStatusModel> ExecuteAsync() {
            var serviceStatusModel = new ServiceStatusModel() {
                Healthy = false,
                Required = check.Required,
                Timestamp = DateTime.UtcNow
            };

            using (var scope = serviceProvider.CreateScope()) {
                try {
                    var dbScope = scope.ServiceProvider.GetRequiredService<DbContext>();
                    dbScope.Database.SetCommandTimeout(check.Timeout);

                    await dbScope.Database.ExecuteSqlCommandAsync("select @@VERSION");

                    serviceStatusModel.Healthy = true;
                    serviceStatusModel.Status = ServiceStatus.Ok;
                    serviceStatusModel.StatusDetail = "Successful";
                } catch (Exception ex) {
                    serviceStatusModel.Status = ServiceStatus.Failure;
                    serviceStatusModel.StatusDetail = ex.Message;
                }
            }

            return serviceStatusModel;
        }
    }
}

