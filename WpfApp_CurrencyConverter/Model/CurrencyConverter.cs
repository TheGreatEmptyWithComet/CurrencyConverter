using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json

namespace WpfApp_CurrencyConverter
{
    public class CurrencyConverter
    {
        private string shortHref = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date=";
        private string fullHref = string.Empty;
        private DateTime date;
        public ObservableCollection<Currency> Currencies { get; private set; } = new ObservableCollection<Currency>();

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                fullHref = shortHref + date.ToString("yyyyMMdd") + "&json";
                LoadCurrencies();
            }
        }

        public string BankHrefApi
        {
            get
            {
                return fullHref;
            }
        }
        public CurrencyConverter()
        {
            Date = DateTime.Now;
        }
        private void LoadCurrencies()
        {
            WebClient webClient = new WebClient();
            string currencyAsJson = webClient.DownloadString(fullHref);

            var currencies = JsonConvert.DeserializeObject<List<Currency>>(currencyAsJson);
        }
    }
}
