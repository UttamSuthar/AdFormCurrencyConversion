namespace AdFormCurrencyConversion.DTOs
{
    

    public class ConversionRatesHistoryDTO
    {
        public string FromCurrency { get; set; }

        public string ToCurrency { get; set; }

        public decimal InputAmount { get; set; }

        public decimal ConvertedAmount { get; set; }

        public DateTime? ConversionDate { get; set; }
    }
}
