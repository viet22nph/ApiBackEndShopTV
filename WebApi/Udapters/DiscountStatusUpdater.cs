
using Microsoft.AspNetCore.Http;
using Services.Concrete;
using Services.Interfaces;

namespace WebApi.Udapters
{
    public class DiscountStatusUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DiscountStatusUpdater> _logger;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(30); // Update every 5 minutes

        public DiscountStatusUpdater(IServiceScopeFactory scopeFactory, ILogger<DiscountStatusUpdater> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _discountService = scope.ServiceProvider.GetRequiredService<IDiscountService>();
                    Console.WriteLine("Hello World!");
                    _logger.LogInformation("Updating discount statuses at: {time}", DateTimeOffset.Now);
                    await _discountService.UpdateAllDiscountStatus();
                }
               
                await Task.Delay(_updateInterval, stoppingToken);
            }
        }
    }
}
