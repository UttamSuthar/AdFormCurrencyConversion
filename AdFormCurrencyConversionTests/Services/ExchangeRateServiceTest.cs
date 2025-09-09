using AdFormCurrencyConversion.Domains;
using AdFormCurrencyConversion.DTOs;
using AdFormCurrencyConversion.Models;
using AdFormCurrencyConversion.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdFormCurrencyConversion.Tests
{
    public class ExchangeRateServiceTests
    {
        private readonly ExchangeRateService _service;
        private readonly AdFormCurrencyContext _context;
        private readonly Mock<ILiveRatesService> _mockLiveRatesService;
        private readonly Mock<ILogger<ExchangeRateService>> _logger;

        public ExchangeRateServiceTests()
        {
            //inmemory database
            var options = new DbContextOptionsBuilder<AdFormCurrencyContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AdFormCurrencyContext(options);

            // table setup
            _context.ExchangeRates.Add(new ExchangeRate
            {
                CurrencyCode = "USD",
                CurrencyDesc = "US Dollar",
                Rate = 80,
                LastUpdated = DateTime.Now,
                CurrencyCodeReference = "DKK"
            });

            _context.CurrencyConversionHistories.Add(new CurrencyConversionHistory
            {
                FromCurrency = "USD",
                ToCurrency = "DKK",
                InputAmount = 10,
                ConvertedAmount = 800,
                ConversionDate = DateTime.Now
            });

            _context.SaveChanges();

            _mockLiveRatesService = new Mock<ILiveRatesService>();

            _logger = new Mock<ILogger<ExchangeRateService>>();
            _service = new ExchangeRateService(_context, _mockLiveRatesService.Object, _logger.Object);
        }

        [Fact]
        public async Task GetAllExchangeRates_ReturnsAllRates()
        {
            var result = await _service.getAllExchangeRates();
            Assert.Single(result);
            Assert.Equal("USD", result.First().CurrencyCode);
        }

        [Fact]
        public async Task GetExchangeRate_ValidCode_ReturnsRate()
        {
            var result = await _service.getExchangeRate("USD");
            Assert.NotNull(result);
            Assert.Equal(80, result.Rate);
        }

        [Fact]
        public async Task GetExchangeRate_InvalidCode_ReturnsNull()
        {
            var result = await _service.getExchangeRate("EUR");
            Assert.Null(result);
        }

        [Fact]
        public async Task ConvertCurrency_ValidRequest_ReturnsConverted()
        {
            var request = new ConvertCurrencyRequestDTO
            {
                CurrencyCode = "USD",
                Amount = 10
            };

            var result = await _service.convertCurrency(request);

            Assert.NotNull(result);
            Assert.Equal("USD", result.FromCurrencyCode);
            Assert.Equal("800.00 DKK", result.ConvertedAmount);
        }

        [Fact]
        public async Task ConversionHistory_WithValidCode_ReturnsList()
        {
            var result = await _service.ConversionHistory("USD", null, null);
            Assert.Single(result);
            Assert.Equal("USD", result[0].FromCurrency);
        }

        [Fact]
        public async Task ConversionRatesUpdate_AddsOrUpdatesRates()
        {
            // Arrange: mock live rates
            _mockLiveRatesService.Setup(x => x.FetchCurrencyRates()).ReturnsAsync(new DailyRates
            {
                Date = DateTime.Now.ToString(),
                Currencies = new List<Currency>
                {
                    new Currency{ Code = "USD", Description = "US Dollar", Rate = "85" },
                    new Currency { Code = "EUR", Description = "Euro", Rate = "90" }
                }
            });

            // Act
            await _service.ConversionRatesUpdate();

            // Assert
            var usdRate = await _context.ExchangeRates.FirstOrDefaultAsync(x => x.CurrencyCode == "USD");

            Assert.Equal(85, usdRate.Rate);
        }
    }
}
