
using AdFormCurrencyConversion.Models;

namespace AdFormCurrencyConversion.Services
{
    public class CurrencyRatesSchedulerService : IHostedService
    {
        private Timer? _timer;
        private ILiveRatesService _liveRatesService;
        private IServiceScopeFactory _serviceScopeFactory;
        private ILogger<CurrencyRatesSchedulerService> _logger;
        public CurrencyRatesSchedulerService(ILiveRatesService liveRatesService, IServiceScopeFactory serviceScopeFactory,ILogger<CurrencyRatesSchedulerService> logger)
        {
            _liveRatesService = liveRatesService;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async _ => await RefreshRatesAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(60));

            return Task.CompletedTask;
        }
        public async Task RefreshRatesAsync()
        {
            try
            {
                var liveCurrencyRates = await _liveRatesService.FetchCurrencyRates();
                using var scope = _serviceScopeFactory.CreateScope();
                var exchangeRateService = scope.ServiceProvider.GetRequiredService<IExchangeRateService>();
                await exchangeRateService.ConversionRatesUpdate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while refresing the rates!!!");
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
