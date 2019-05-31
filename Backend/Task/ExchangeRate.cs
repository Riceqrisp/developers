using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ExchangeRateUpdater
{

    public class ExchangeRateList
    {
        public List<ExchangeRate> testList = new List<ExchangeRate>();

        public class ExchangeRate
        {

            public ExchangeRate(Currency sourceCurrency, Currency targetCurrency, decimal value)
            {
                SourceCurrency = sourceCurrency;
                TargetCurrency = targetCurrency;
                Value = value;
            }
            public Currency SourceCurrency { get; }

            public Currency TargetCurrency { get; }

            public decimal Value { get; }

            public override string ToString()
            {
                return $"{SourceCurrency}/{TargetCurrency}={Value}";
            }
        }
    }
}
