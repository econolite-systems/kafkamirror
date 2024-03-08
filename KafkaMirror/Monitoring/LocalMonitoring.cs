// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Monitoring.Metrics;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace KafkaMirror.Monitoring
{
    public class LocalMonitoring : ILocalMonitor
    {
        private readonly ILogger _logger;

        public LocalMonitoring(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Name);
        }

        public Task OnMonitoringAsync(KeyValuePair<Instrument, long>[] observations, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Monitoring data: {@}", new { Metrics = observations.Select(_ => new { _.Key.Name, _.Value }) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to log monitoring data");
            }

            return Task.CompletedTask;
        }
    }
}
