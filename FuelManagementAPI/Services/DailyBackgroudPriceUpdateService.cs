using Microsoft.Extensions.Hosting;

namespace FuelManagementAPI.Services
{
    public class DailyBackgroundPriceUpdateService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DailyBackgroundPriceUpdateService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextRunTime = now.Date.AddDays(1);
                var delay = nextRunTime - now;

                await Task.Delay(delay, stoppingToken);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var updater = scope.ServiceProvider.GetRequiredService<DailyPriceUpdaterService>();
                    await updater.UpdateDailyPricesAsync();
                }
            }
        }
    }
}
