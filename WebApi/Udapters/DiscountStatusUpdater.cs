
using Services.Interfaces;

namespace WebApi.Udapters
{
    public class DiscountStatusUpdater : BackgroundService
    {
        private readonly IDiscountService _discountService;
        private readonly ILogger<DiscountStatusUpdater> _logger;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(720); // Update every 5 minutes

        public DiscountStatusUpdater(IDiscountService discountService, ILogger<DiscountStatusUpdater> logger)
        {
            _discountService = discountService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Updating discount statuses at: {time}", DateTimeOffset.Now);
                _discountService.UpdateAllDiscountStatus();
                await Task.Delay(_updateInterval, stoppingToken);
            }
        }
    }
}
