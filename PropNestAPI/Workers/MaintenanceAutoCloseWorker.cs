using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PropNest.Models;

namespace PropNestAPI.Workers
{
    public class MaintenanceAutoCloseWorker : BackgroundService
    {
        private readonly MaintenanceRequestRepository _repo;
        private readonly ILogger<MaintenanceAutoCloseWorker> _logger;

        public MaintenanceAutoCloseWorker(MaintenanceRequestRepository repo, ILogger<MaintenanceAutoCloseWorker> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MaintenanceAutoCloseWorker starting");

            try
            {
                var closed = _repo.AutoCloseOldRequests();
                _logger.LogInformation("Auto-closed {count} maintenance requests on startup", closed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running initial auto-close");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                    var closed = _repo.AutoCloseOldRequests();
                    _logger.LogInformation("Auto-closed {count} maintenance requests during scheduled run", closed);
                }
                catch (TaskCanceledException)
                {
                    // shutdown requested
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during auto-close loop");
                }
            }

            _logger.LogInformation("MaintenanceAutoCloseWorker stopping");
        }
    }
}
