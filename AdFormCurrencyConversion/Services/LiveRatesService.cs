using AdFormCurrencyConversion.Domains;
using AdFormCurrencyConversion.Models;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Serialization;

namespace AdFormCurrencyConversion.Services
{
    public interface ILiveRatesService
    {
        public Task<DailyRates> FetchCurrencyRates();
    }
    public class LiveRatesService : ILiveRatesService
    {
        private HttpClient _httpClient;
        public LiveRatesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<DailyRates?> FetchCurrencyRates()
        {
            try
            {
                //var rates=await _httpClient.GetAsync("https://www.nationalbanken.dk/api/currencyratesxml?lang=da");
                var rates = await _httpClient.GetStreamAsync("https://www.nationalbanken.dk/api/currencyratesxml?lang=da");
                var serializer = new XmlSerializer(typeof(LiveExchangeRates));
                var result = (LiveExchangeRates?)serializer.Deserialize(rates);
                return result?.DailyRates;
            }
            catch (Exception ex)
            {
                //log
                return null;
            }
        }
    }
}
