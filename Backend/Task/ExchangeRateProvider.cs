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

            string currencyList = null;

            HttpResponseMessage currencyResponse = await client.GetAsync("https://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.xml");

            if (currencyResponse.IsSuccessStatusCode)
            {
                currencyList = await currencyResponse.Content.ReadAsAsync<string>();
            }

            return currencyList;
        }
        public ExchangeRateList GetExchangeRatesList()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("exchangeRateUpdater", "test");

            string xmlString = null;
            
            HttpResponseMessage response = client.GetAsync("https://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.xml").Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content;
                var readed = content.ReadAsStringAsync();
                xmlString = readed.Result;

                return StringtoXML(xmlString);
            }
            return null;
        }
        public ExchangeRateList StringtoXML(string xmlString)
        {
            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            byte[] byteArray = Encoding.ASCII.GetBytes(xmlString);
            MemoryStream stream = new MemoryStream(byteArray);

            XmlSerializer deSerializer = new XmlSerializer(typeof(List<ExchangeRate>));
            object obj = deSerializer.Deserialize(stream);

            ExchangeRateList XmlData = (ExchangeRateList)obj;

            return XmlData;
        }
    }
}



    