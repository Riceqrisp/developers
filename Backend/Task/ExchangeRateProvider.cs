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
            var xmlResponse = GetUrl();
            List<ExchangeRate> exchangeRateTempList = new List<ExchangeRate>();
            var tempList  = xmlResponse.Result.tabulka.radek;

            foreach (var item in tempList)
            {
                decimal decimalVal;
                decimalVal = Convert.ToDecimal(item.kurz);
                exchangeRateTempList.Add(new ExchangeRate(new Currency(item.kod), new Currency("CZK"), decimalVal));
            }

            return CheckInResponseIfTheyBelongToCurrenciesGiven(currencies, exchangeRateTempList);
        }

        public IEnumerable<ExchangeRate> CheckInResponseIfTheyBelongToCurrenciesGiven(IEnumerable<Currency> currencies, IEnumerable<ExchangeRate> exchangeRates)
        {
            List<ExchangeRate> tempExchangeRateList = exchangeRates.ToList();
            List<Currency> tempListOfCurrency = currencies.ToList();

            var query = tempExchangeRateList.Where(sourceCurrency => currencies.Any(currency => currency.Code.Equals(sourceCurrency)));

            return query;
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
        public async Task<kurzy> GetUrl()
        {
            var _url = "https://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.xml";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(_url);
                var stream = await response.Content.ReadAsStreamAsync();

                XmlSerializer serializer = new XmlSerializer(typeof(kurzy));
                var source = (kurzy)serializer.Deserialize(stream);

                return source;
            }
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

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class kurzy
        {

            private kurzyTabulka tabulkaField;

            private string bankaField;

            private string datumField;

            private byte poradiField;

            /// <remarks/>
            public kurzyTabulka tabulka
            {
                get
                {
                    return this.tabulkaField;
                }
                set
                {
                    this.tabulkaField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string banka
            {
                get
                {
                    return this.bankaField;
                }
                set
                {
                    this.bankaField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string datum
            {
                get
                {
                    return this.datumField;
                }
                set
                {
                    this.datumField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte poradi
            {
                get
                {
                    return this.poradiField;
                }
                set
                {
                    this.poradiField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class kurzyTabulka
        {

            private kurzyTabulkaRadek[] radekField;

            private string typField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("radek")]
            public kurzyTabulkaRadek[] radek
            {
                get
                {
                    return this.radekField;
                }
                set
                {
                    this.radekField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string typ
            {
                get
                {
                    return this.typField;
                }
                set
                {
                    this.typField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class kurzyTabulkaRadek
        {

            private string kodField;

            private string menaField;

            private ushort mnozstviField;

            private string kurzField;

            private string zemeField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string kod
            {
                get
                {
                    return this.kodField;
                }
                set
                {
                    this.kodField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string mena
            {
                get
                {
                    return this.menaField;
                }
                set
                {
                    this.menaField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ushort mnozstvi
            {
                get
                {
                    return this.mnozstviField;
                }
                set
                {
                    this.mnozstviField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string kurz
            {
                get
                {
                    return this.kurzField;
                }
                set
                {
                    this.kurzField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string zeme
            {
                get
                {
                    return this.zemeField;
                }
                set
                {
                    this.zemeField = value;
                }
            }
        }


    }
}