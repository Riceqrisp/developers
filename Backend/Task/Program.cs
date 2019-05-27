using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateUpdater
{
    public static class Program
    {
        private static IEnumerable<Currency> currencies = new[]
        {
            new Currency("USD"),
            new Currency("EUR"),
            new Currency("CZK"),
            new Currency("JPY"),
            new Currency("KES"),
            new Currency("RUB"),
            new Currency("THB"),
            new Currency("TRY"),
            new Currency("XYZ")
        };

        public static void Main(string[] args)
        {
            try
            {
                var provider = new ExchangeRateProvider();
                var rates = provider.GetExchangeRates(currencies);
                //var ratesTest = provider.GetExchangeRatesList();
                var ratesStringTest = provider.GetStringOfExchangeRates().Result;
                var ratesTest = provider.ResponseToList(ratesStringTest, currencies.ToList());
                

                Console.WriteLine($"Successfully retrieved {ratesTest.Count()} exchange rates:");
                foreach (var rate in ratesTest)
                {

                    Console.WriteLine(rate.ToString());

                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not retrieve exchange rates: '{e.Message}'.");
            }

            Console.ReadLine();
        }
    }
}
