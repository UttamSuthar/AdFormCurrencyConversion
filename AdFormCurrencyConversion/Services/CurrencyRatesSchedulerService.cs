
using AdFormCurrencyConversion.Models;

namespace AdFormCurrencyConversion.Services
{
    public class CurrencyRatesSchedulerService : IHostedService
    {
        private Timer? _timer;
        private IRatesService _ratesService;
        private IServiceScopeFactory _serviceScopeFactory;
        public CurrencyRatesSchedulerService(IRatesService ratesService, IServiceScopeFactory serviceScopeFactory)
        {

            _ratesService = ratesService;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async _ => await RefreshRatesAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(60));

            return Task.CompletedTask;
        }
        private async Task RefreshRatesAsync()
        {
            try
            {
                var liveCurrencyRates = await _ratesService.FetchCurrencyRates();
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AdFormCurrencyContext>();

                foreach (var currency in liveCurrencyRates.Currencies)
                {
                    //Add to table
                    var existingCurrency = context.ExchangeRates.FirstOrDefault(a => a.CurrencyCode == currency.Code);
                    if (existingCurrency != null)
                    {
                        var newRate = Convert.ToDecimal(currency.Rate);
                        if (existingCurrency.Rate != newRate)
                        {
                            existingCurrency.Rate = newRate;
                            existingCurrency.LastUpdated = Convert.ToDateTime(liveCurrencyRates.Date);
                            await context.SaveChangesAsync();
                        }
                    }
                    else
                    {

                        context.ExchangeRates.Add(new ExchangeRate
                        {
                            CurrencyCode = currency.Code,
                            CurrencyDesc = currency.Description,
                            LastUpdated = DateTime.Now,
                            Rate = Convert.ToDecimal(currency.Rate)
                        });
                    }
                }
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //log
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
