
namespace AdFormCurrencyConversion.Domains
{
    using System.Xml.Serialization;

    //Root element
    [XmlRoot("exchangerates")]
    public class LiveExchangeRates
    {
        [XmlElement("dailyrates")]
        public DailyRates DailyRates { get; set; } 
    }

    public class DailyRates
    {
        [XmlAttribute("id")]
        public string Date { get; set; } 

        [XmlElement("currency")]
        public List<Currency> Currencies { get; set; } = new List<Currency>();
    }

    public class Currency
    {
        [XmlAttribute("code")]
        public string Code { get; set; }

        [XmlAttribute("desc")]
        public string Description { get; set; } 

        [XmlAttribute("rate")]
        public string Rate { get; set; } 
    }

}
