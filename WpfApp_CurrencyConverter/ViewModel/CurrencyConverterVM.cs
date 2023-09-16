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
    public class CurrencyConverterVM : NotifyPropertyChangedHandler
    {
        private string shortHref = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date=";
        private string fullHref = string.Empty;
        public ObservableCollection<Currency> Currencies { get; private set; } = new ObservableCollection<Currency>();
        public ObservableCollection<string> CurrencyNames { get; private set; } = new ObservableCollection<string>();
        
        private string firstCurrencyName; 
        public string FirstCurrencyName
        {
            get { return firstCurrencyName; }
            set
            {
                if (firstCurrencyName != value)
                {
                    firstCurrencyName = value;
                    // Calculate opposit currency value if current currency is changed
                    SetSecondCurrencyValue();
                }
            }
        }
        private string secondCurrencyName;
        public string SecondCurrencyName
        {
            get { return secondCurrencyName; }
            set
            {
                if (secondCurrencyName != value)
                {
                    secondCurrencyName = value;
                    // Calculate opposit currency value if current currency is changed
                    SetFirstCurrencyValue();
                }
            }
        }

        private double firstCurrencyValue;
        public double FirstCurrencyValue
        {
            get { return firstCurrencyValue; }
            set
            {
                if (firstCurrencyValue != value)
                {
                    firstCurrencyValue = value;
                    OnPropertyChanged(nameof(FirstCurrencyValue));
                    SetSecondCurrencyValue();
                }
            }
        }
        private double secondCurrencyValue;
        public double SecondCurrencyValue
        {
            get { return secondCurrencyValue; }
            set
            {
                if (secondCurrencyValue != value)
                {
                    secondCurrencyValue = value;
                    OnPropertyChanged(nameof(SecondCurrencyValue));
                    SetFirstCurrencyValue();
                }
            }
        }

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

        public CurrencyConverterVM()
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

            // Add UAH currency to the list
            Currencies.Add(new Currency() { r030 = 980, txt = "Українська гривня", rate = 1, cc = "UAH", exchangedate = Date.ToString("d") });
        }
        private void InitCurrencyNamesList()
        {
            var currencyNames = Currencies.OrderBy(c=>c.txt).Select(c => c.txt).ToList();
            if (currencyNames != null)
            {
                CurrencyNames = new ObservableCollection<string>(currencyNames);
            }
        }
        private void SetSecondCurrencyValue()
        {
            var firstCurrencyRate = Currencies.Where(c => c.txt == FirstCurrencyName).Select(c => c.rate).FirstOrDefault();
            var secondCurrencyRate = Currencies.Where(c => c.txt == SecondCurrencyName).Select(c => c.rate).FirstOrDefault();

            if (secondCurrencyRate != 0)
            {
                SecondCurrencyValue = firstCurrencyValue * firstCurrencyRate / secondCurrencyRate;
            }
        }
        private void SetFirstCurrencyValue()
        {
            var firstCurrencyRate = Currencies.Where(c => c.txt == FirstCurrencyName).Select(c => c.rate).FirstOrDefault();
            var secondCurrencyRate = Currencies.Where(c => c.txt == SecondCurrencyName).Select(c => c.rate).FirstOrDefault();

            if (firstCurrencyRate != 0)
            {
                FirstCurrencyValue = secondCurrencyValue * secondCurrencyRate / firstCurrencyRate;
            }
        }
    }
}
