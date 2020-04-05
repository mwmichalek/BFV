using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BFV.Services.Appliance;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BFV.Simulation.WorkerService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly IAppliance _appliance;

        public Worker(ILogger<Worker> logger, IAppliance appliance) {
            _logger = logger;
            _appliance = appliance;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _appliance.Refresh();
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
