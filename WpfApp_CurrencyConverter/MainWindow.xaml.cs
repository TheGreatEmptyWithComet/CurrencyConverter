using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_CurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CurrencyConverterVM currencyConverterVM = new CurrencyConverterVM();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = currencyConverterVM;
        }

        private void SecondCurrency_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            currencyConverterVM.SetSecondCurrencyValueIsEdited.Execute(null);
        }

        private void SecondCurrency_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            currencyConverterVM.UnsetSecondCurrencyValueIsEdited.Execute(null);
        }
    }
}
