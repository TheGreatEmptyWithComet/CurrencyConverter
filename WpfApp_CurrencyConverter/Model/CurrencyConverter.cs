using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WpfApp_CurrencyConverter
{
    public class CurrencyConverter : NotifyPropertyChangedHandler
    {
        private string shortHref = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date=";
        private string fullHref = string.Empty;
        public ObservableCollection<Currency> Currencies { get; private set; } = new ObservableCollection<Currency>();
        public ObservableCollection<string> CurrencyNames { get; private set; } = new ObservableCollection<string>();
        public string FirstCurrency { get; set; }
        public string SecondCurrency { get; set; }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                fullHref = shortHref + date.ToString("yyyyMMdd") + "&json";
                LoadCurrencies();
                InitCurrencyNamesList();
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
            if (currencies != null)
            {
                Currencies = new ObservableCollection<Currency>(currencies);
            }
        }
        private void InitCurrencyNamesList()
        {
            var currencyNames = Currencies.Select(c => c.txt).ToList();
            if (currencyNames != null)
            {
                CurrencyNames = new ObservableCollection<string>(currencyNames);
            }
        }
    }
}
