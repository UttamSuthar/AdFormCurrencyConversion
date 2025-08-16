using AdFormCurrencyConversion.Domains;
using AdFormCurrencyConversion.DTOs;
using AdFormCurrencyConversion.Models;
using AdFormCurrencyConversion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AdFormCurrencyConversion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class CurrencyController : ControllerBase
    {
        private IExchangeRateService _exchangeRateService;

        public CurrencyController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet("AllExchangeRates")]
        public async Task<List<ExchangeRate>> GetAllCurrencyRates()
        {
            var allCurrencyRates = await _exchangeRateService.getAllExchangeRates();
            return allCurrencyRates.ToList();
        }

        //SingleRates
        [HttpGet("ExchangeRate/{code}")]
        public async Task<ActionResult<ExchangeRate>> GetExchangeRate(string code)
        {
            var rate = await _exchangeRateService.getExchangeRate(code.ToUpper());
            if (rate != null)
                return Ok(rate);
            else
                return NoContent();
        }


        [HttpPost("Convert")]
        public async Task<ConvertedCurrencyResponseDTO> ConvertCurrency(ConvertCurrencyRequestDTO convertCurrencyRequest)
        {
            var convertedRates = await _exchangeRateService.convertCurrency(convertCurrencyRequest);
            return convertedRates;
        }

        //SingleRates
        [HttpGet("ConversionHistory")]
        public async Task<ActionResult<List<ConversionRatesHistoryDTO>>> ConversionHistory([FromQuery]string currencycode,
            [FromQuery]DateTime? from, [FromQuery] DateTime? to)
        {
            if ((from.HasValue && to.HasValue) )
            {
                if (from > to)
                {
                    return BadRequest();
                }
            }
            if ((!from.HasValue && to.HasValue))
            {
                return BadRequest();
            }
            var conversionHistory = await _exchangeRateService.ConversionHistory(currencycode.ToUpper(),from,to);
            return conversionHistory;
        }




        [HttpGet("RefreshConversionRates")]
        public async Task<string> Refresh()
        {
            await _exchangeRateService.ConversionRatesUpdate();
            return "Refreshed";
        }

    }
}
