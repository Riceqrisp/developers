using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ExchangeRateUpdater

{
    public class ExchangeRateProvider
    {

        /// <summary>
        /// Should return exchange rates among the specified currencies that are defined by the source. But only those defined
        /// by the source, do not return calculated exchange rates. E.g. if the source contains "EUR/USD" but not "USD/EUR",
        /// do not return exchange rate "USD/EUR" with value calculated as 1 / "EUR/USD". If the source does not provide
        /// 
        /// some of the currencies, ignore them.
        /// </summary>
        public IEnumerable<ExchangeRate> GetExchangeRates(IEnumerable<Currency> currencies)
        {

            return Enumerable.Empty<ExchangeRate>();
        }

        public async Task<string> GetStringOfExchangeRates()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("exchangeRateUpdater", "test");

            string currencies = null;

            HttpResponseMessage currencyResponse = await client.GetAsync("https://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt");

            if (currencyResponse.IsSuccessStatusCode)
            {
                currencies = await currencyResponse.Content.ReadAsStringAsync();
            }

            return currencies;
        }
        public ExchangeRateList StringtoXML(string xmlString)
        {
            xmlString = xmlString.Replace("\n", string.Empty);
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(xmlString);

            byte[] byteArray = Encoding.ASCII.GetBytes(xmlString);
            MemoryStream stream = new MemoryStream(byteArray);
            XmlSerializer deSerializer = new XmlSerializer(typeof(List<ExchangeRate>));
            var streamWriter = new StreamWriter(stream, Encoding.UTF8);

            object obj = deSerializer.Deserialize(stream);

            ExchangeRateList XmlData = (ExchangeRateList)obj;

            return XmlData;
        }
        public List<ExchangeRate> ResponseToList(List<Currency> currencies)
        {

            List<ExchangeRate> listOfRates = new List<ExchangeRate>();
            List<string> tempList = new List<string>();
            string responseString = GetStringOfExchangeRates().Result;

            string[] lines = responseString.Split(Environment.NewLine.ToCharArray());

            foreach (var item in currencies)
            {
                tempList.Add(item.ToString());
            }
            
            foreach (var line in lines)
            {
                string[] splitPipe = line.Split('|');

                if (splitPipe.ToList().Intersect(tempList).Any())
                {
                    decimal decimalVal;
                    string value = splitPipe[4];
                    decimalVal = Convert.ToDecimal(value);
                    listOfRates.Add(new ExchangeRate(new Currency(splitPipe[3]), new Currency("CZK"), decimalVal));
                }

            }
           return listOfRates;
        }
    }
}