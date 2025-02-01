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
                File.WriteAllText("currency.txt", selectedCurrency);
            }
        }

        public settingsPanel()
        {
            InitializeComponent();

            listawalut.ItemsSource = File.ReadAllLines("currencies.txt");
        }
    }
}
