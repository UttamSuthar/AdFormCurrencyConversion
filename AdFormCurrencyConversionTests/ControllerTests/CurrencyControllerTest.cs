
using AdFormCurrencyConversion.Controllers;
using AdFormCurrencyConversion.DTOs;
using AdFormCurrencyConversion.Models;
using AdFormCurrencyConversion.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AdFormCurrencyConversion.Tests
{
    public class CurrencyControllerTest
    {
        private readonly Mock<IExchangeRateService> _mockService;
        private readonly CurrencyController _controller;

        public CurrencyControllerTest()
        {
            _mockService = new Mock<IExchangeRateService>();
            _controller = new CurrencyController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllCurrencyRates_ReturnsListOfRates()
        {
            // Arrange
            var rates = new List<ExchangeRate>
            {
                new ExchangeRate { CurrencyCode = "USD", Rate = 80 },
                new ExchangeRate { CurrencyCode = "EUR", Rate = 90 }
            };
            _mockService.Setup(s => s.getAllExchangeRates()).ReturnsAsync(rates);

            // Act
            var result = await _controller.GetAllCurrencyRates();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.CurrencyCode == "USD");
        }

        [Fact]
        public async Task GetExchangeRate_WithValidCode_ReturnsOk()
        {
            // Arrange
            var rate = new ExchangeRate { CurrencyCode = "USD", Rate = 80 };
            _mockService.Setup(s => s.getExchangeRate("USD")).ReturnsAsync(rate);

            // Act
            var result = await _controller.GetExchangeRate("USD");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(rate, okResult.Value);
        }

        [Fact]
        public async Task GetExchangeRate_WithInvalidCode_ReturnsNoContent()
        {
            // Arrange
            _mockService.Setup(s => s.getExchangeRate("XYZ")).ReturnsAsync((ExchangeRate)null);

            // Act
            var result = await _controller.GetExchangeRate("XYZ");

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task ConvertCurrency_ReturnsConvertedCurrency()
        {
            // Arrange
            var request = new ConvertCurrencyRequestDTO { CurrencyCode = "USD", Amount = 10 };
            var response = new ConvertedCurrencyResponseDTO { FromCurrencyCode = "USD", ActualAmount = 10, ConvertedAmount = "800",RateDate=DateTime.Now};
            _mockService.Setup(s => s.convertCurrency(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.ConvertCurrency(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("800", result.ConvertedAmount);
        }

        [Fact]
        public async Task ConversionHistory_ReturnsList()
        {
            // Arrange
            var history = new List<ConversionRatesHistoryDTO>
            {
                new ConversionRatesHistoryDTO { FromCurrency = "USD",ToCurrency="DKK",  InputAmount= 10, ConvertedAmount= 800, ConversionDate= DateTime.Now }
            };
            _mockService.Setup(s => s.ConversionHistory("USD", It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).ReturnsAsync(history);

            // Act
            var result = await _controller.ConversionHistory("USD", null, null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<ConversionRatesHistoryDTO>>>(result);
            Assert.Equal(history, actionResult.Value);
        }
    }
}
