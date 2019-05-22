using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ExchangeRateUpdater
{
    [XmlRoot("kurzy")]
    public class ExchangeRateList
    {
        [XmlElement("tabulka")]
        public List<ExchangeRate> one = new List<ExchangeRate>();
    }

    
    public class ExchangeRate
    {
        

        public ExchangeRate(Currency sourceCurrency, Currency targetCurrency, decimal value)
        {
            SourceCurrency = sourceCurrency;
            TargetCurrency = targetCurrency;
            Value = value;
        }
        public ExchangeRate()
        {

        }
        [XmlAttribute("kod")]
        public Currency SourceCurrency { get; }

        public Currency TargetCurrency { get; }

        [XmlAttribute("kurz")]
        public decimal Value { get; }

        public override string ToString()
        {
            return $"{SourceCurrency}/{TargetCurrency}={Value}";
        }
    }
}
