using System.Windows.Controls;
using System.Text.Json;
using System.IO;

namespace addons
{
    public partial class settingsPanel : UserControl
    {
        public event Action usun; //dodaje nową kacje

        private void back(object sender, System.Windows.RoutedEventArgs e)
        {
            usun?.Invoke(); //wywołuję zdarzenie
        }

        private void listazmiana(object sender, SelectionChangedEventArgs e)
        {
            if (listawalut.SelectedItem != null)
            {
                string selectedCurrency = listawalut.SelectedItem.ToString();
                File.WriteAllText("data/currency.txt", selectedCurrency);
            }
        }

        private void listazmianacustom1(object sender, SelectionChangedEventArgs e)
        {
            if (listawalutcustom1.SelectedItem != null)
            {
                string custom1Currency = listawalutcustom1.SelectedItem.ToString();
                File.WriteAllText("data/custom1.txt", custom1Currency);
            }
        }

        private void listazmianacustom2(object sender, SelectionChangedEventArgs e)
        {
            if (listawalutcustom2.SelectedItem != null)
            {
                string custom2Currency = listawalutcustom2.SelectedItem.ToString();
                File.WriteAllText("data/custom2.txt", custom2Currency);
            }
        }

        public settingsPanel()
        {
            InitializeComponent();

            listawalut.ItemsSource = File.ReadAllLines("data/currencies.txt");
            listawalutcustom1.ItemsSource = File.ReadAllLines("data/currencies.txt");
            listawalutcustom2.ItemsSource = File.ReadAllLines("data/currencies.txt");
        }
    }
}
