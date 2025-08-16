using AdFormCurrencyConversion.Domains;
using AdFormCurrencyConversion.DTOs;
using AdFormCurrencyConversion.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdFormCurrencyConversion.Services
{
    public interface IExchangeRateService
    {
        Task<IEnumerable<ExchangeRate>> getAllExchangeRates();
        Task<ExchangeRate> getExchangeRate(string code);
        Task<ConvertedCurrencyResponseDTO> convertCurrency(ConvertCurrencyRequestDTO request);
        Task ConversionRatesUpdate();
        Task<List<ConversionRatesHistoryDTO>> ConversionHistory(string code, DateTime? from, DateTime? to);
    }
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly AdFormCurrencyContext _context;
        private readonly ILiveRatesService _liveRatesService;
        public ExchangeRateService(AdFormCurrencyContext context,ILiveRatesService liveRatesService)
        {
            _context = context;
            _liveRatesService = liveRatesService;
        }

        public async Task<IEnumerable<ExchangeRate>> getAllExchangeRates()
        {
            try
            {
                var allRates = await _context.ExchangeRates.ToListAsync();
                return allRates;
            }
            catch (Exception ex)
            {
                return new List<ExchangeRate>();
            }
        }

        public async Task<ExchangeRate> getExchangeRate(string code)
        {
            try
            {
                var exchangeRate = await _context.ExchangeRates.FirstOrDefaultAsync(a => a.CurrencyCode == code);
                return exchangeRate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ConvertedCurrencyResponseDTO> convertCurrency(ConvertCurrencyRequestDTO request)
        {
            //get exchangeRates
            var exchangeRate = await _context.ExchangeRates.FirstOrDefaultAsync(a => a.CurrencyCode == request.CurrencyCode.ToUpper());
            if (exchangeRate != null)
            {
                var convertedAmount = (request.Amount * exchangeRate.Rate);
                //After Converting store in db

                var conversionCurr = new CurrencyConversionHistory
                {
                    ConversionDate = DateTime.Now,
                    ConvertedAmount = convertedAmount,
                    FromCurrency = request.CurrencyCode,
                    InputAmount = request.Amount,
                    ToCurrency = ExchangeRatesCommon.DanishKroneCode
                };
                await _context.CurrencyConversionHistories.AddAsync(conversionCurr);
                await _context.SaveChangesAsync();
                return new ConvertedCurrencyResponseDTO
                {
                    ActualAmount = request.Amount,
                    ConvertedAmount = convertedAmount.ToString("0.00") + " " + ExchangeRatesCommon.DanishKroneCode,
                    FromCurrencyCode = request.CurrencyCode,
                    RateDate = exchangeRate.LastUpdated.Value
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<List<ConversionRatesHistoryDTO>> ConversionHistory(string code, DateTime? from, DateTime? to)
        {
            //get exchangeRates
            var conversionHistoryList = new List<ConversionRatesHistoryDTO>();
            var conversionHistory = _context.CurrencyConversionHistories.Where(
                a => a.FromCurrency == code.ToUpper()
                && (!from.HasValue ||(a.ConversionDate.Date >= from.Value.Date))
                && (!to.HasValue || (a.ConversionDate.Date <= to.Value.Date))
                ).ToList();
            if (conversionHistory != null)
            {

                foreach (var ch in conversionHistory)
                {
                    conversionHistoryList.Add(new ConversionRatesHistoryDTO { 
                        ConversionDate = ch.ConversionDate,
                        ConvertedAmount= ch.ConvertedAmount,
                        FromCurrency = ch.FromCurrency,
                        InputAmount = ch.InputAmount,
                        ToCurrency = ch.ToCurrency                    
                    });
                }
            }
            return conversionHistoryList;
        }


        public async Task ConversionRatesUpdate()
        {
            try
            {
                var liveCurrencyRates = await _liveRatesService.FetchCurrencyRates();
                
                foreach (var currency in liveCurrencyRates.Currencies)
                {
                    //Add to table
                    currency.Rate = currency.Rate.Replace(",", ".");
                    var existingCurrency = _context.ExchangeRates.FirstOrDefault(a => a.CurrencyCode == currency.Code);
                    if (existingCurrency != null)
                    {
                        //assuming here "," as decimal
                        var newRate = Convert.ToDecimal(currency.Rate);
                        if (existingCurrency.Rate != newRate)
                        {
                            existingCurrency.Rate = newRate;
                            existingCurrency.LastUpdated = Convert.ToDateTime(liveCurrencyRates.Date);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {

                        _context.ExchangeRates.Add(new ExchangeRate
                        {
                            CurrencyCode = currency.Code,
                            CurrencyDesc = currency.Description,
                            LastUpdated = DateTime.Now,
                            Rate = Convert.ToDecimal(currency.Rate)
                        });
                    }
                }
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //log
            }
        }

    }
}
