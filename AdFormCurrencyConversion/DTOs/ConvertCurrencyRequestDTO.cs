namespace AdFormCurrencyConversion.DTOs
{
    public class ConvertCurrencyRequestDTO
    {
        public string CurrencyCode{ get; set; }
        public decimal Amount{ get; set; }
    }

    public class ConvertedCurrencyResponseDTO
    {
        public string FromCurrencyCode { get; set; }
        //public string ToCurrencyCode { get; set; }
        public decimal ActualAmount { get; set; }
        public string ConvertedAmount { get; set; }
        public DateTime RateDate { get; set; }
    }
}
