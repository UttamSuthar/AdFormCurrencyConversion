using AdFormCurrencyConversion.Domains;
using AdFormCurrencyConversion.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AdFormCurrencyConversion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private IRatesService _ratesService;
        public CurrencyController(IRatesService ratesService) {
            _ratesService = ratesService;
        }

        [HttpGet]
        public async Task<DailyRates> GetAllCurrencyRates()
        {
            var allCurrencyRates = await _ratesService.FetchCurrencyRates();
            return fds;
        }


    }
}
