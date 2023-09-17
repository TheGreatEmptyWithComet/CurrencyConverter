using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace WpfApp_CurrencyConverter
{
    public class CurrencyConverterVM : NotifyPropertyChangedHandler
    {
        private string shortHref = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date=";
        private string fullHref = string.Empty;
        private bool secondCurrencyValueIsEdited = false;
        public ObservableCollection<Currency> Currencies { get; private set; } = new ObservableCollection<Currency>();
        public ObservableCollection<string> CurrencyNames { get; private set; } = new ObservableCollection<string>();

        public ICommand SetSecondCurrencyValueIsEdited;
        public ICommand UnsetSecondCurrencyValueIsEdited;

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
                    // Calculate currency value if currency is changed
                    SetSecondCurrencyValue();
                }
            }
        }

        private string firstCurrencyValue;
        public string FirstCurrencyValue
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
        private string secondCurrencyValue;
        public string SecondCurrencyValue
        {
            get { return secondCurrencyValue; }
            set
            {
                if (secondCurrencyValue != value)
                {
                    secondCurrencyValue = value;
                    OnPropertyChanged(nameof(SecondCurrencyValue));
                    if (secondCurrencyValueIsEdited)
                    {
                        SetFirstCurrencyValue();
                    }
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
                OnPropertyChanged(nameof(Currencies));
                InitCurrencyNamesList();
                SetSecondCurrencyValue();
            }
        }

        public CurrencyConverterVM()
        {
            Date = DateTime.Now;
            SetSecondCurrencyValueIsEdited = new RelayCommand(() => { secondCurrencyValueIsEdited = true; });
            UnsetSecondCurrencyValueIsEdited = new RelayCommand(() => { secondCurrencyValueIsEdited = false; });
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
            var currencyNames = Currencies.OrderBy(c => c.txt).Select(c => c.txt).ToList();
            if (currencyNames != null)
            {
                CurrencyNames = new ObservableCollection<string>(currencyNames);
            }
        }
        private void SetSecondCurrencyValue()
        {
            var firstCurrencyRate = Currencies.Where(c => c.txt == FirstCurrencyName).Select(c => c.rate).FirstOrDefault();
            var secondCurrencyRate = Currencies.Where(c => c.txt == SecondCurrencyName).Select(c => c.rate).FirstOrDefault();

            if (secondCurrencyRate != 0 && double.TryParse(FirstCurrencyValue, out double firstCurrencyValue))
            {
                SecondCurrencyValue = (firstCurrencyValue * firstCurrencyRate / secondCurrencyRate).ToString("0.##");
            }
            else
            {
                SecondCurrencyValue = string.Empty;
            }
        }
        private void SetFirstCurrencyValue()
        {
            var firstCurrencyRate = Currencies.Where(c => c.txt == FirstCurrencyName).Select(c => c.rate).FirstOrDefault();
            var secondCurrencyRate = Currencies.Where(c => c.txt == SecondCurrencyName).Select(c => c.rate).FirstOrDefault();

            if (firstCurrencyRate != 0 && double.TryParse(SecondCurrencyValue, out double secondCurrencyValue))
            {
                FirstCurrencyValue = (secondCurrencyValue * secondCurrencyRate / firstCurrencyRate).ToString("0.##");
            }
            else
            {
                FirstCurrencyValue = string.Empty;
            }
        }
    }
}
