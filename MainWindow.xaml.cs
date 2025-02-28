using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Policy;
using System;
using OxyPlot.Axes;
using addons;

namespace tradelens
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void wykres_kreator(object sender, EventArgs e)
        {
            string currency = ((Button)sender).Tag as string; //pobiera wartosc z Tag (walute)

            var plotModel = new PlotModel 
            { 
                Title = currency+"/"+File.ReadAllText("data/currency.txt"), //tytul
                TitleColor = OxyColors.White //kolor
            };

            var lineSeries = new LineSeries //nowa linia na wykresie
            {
                Title = currency+ "/" + File.ReadAllText("data/currency.txt"),
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                StrokeThickness = 2,
                Color = OxyColors.White //kolor linii

            };

            var X = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Data",
                StringFormat = "dd-MM-yy",
                IntervalType = DateTimeIntervalType.Days, //odstępy w dniach
                MinorIntervalType = DateTimeIntervalType.Hours, //mniejszy odstep
                TitleColor = OxyColors.White,     //tytul kolor
                TextColor = OxyColors.White,    //kolor tekstu
                TicklineColor = OxyColors.White  //kolor linii
            };

            var Y = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Cena",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,  
                TicklineColor = OxyColors.White
            };

            plotModel.Axes.Add(X);
            plotModel.Axes.Add(Y);

            string[] files = Directory.GetFiles("data/" + currency);

            int n = 0;

            foreach ( string file in files) 
            {
                string jsonContent = File.ReadAllText(file);
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))//zamieniam tekst na jsondocument
                {
                    double y = Math.Round(doc.RootElement.GetProperty(currency).GetProperty(File.ReadAllText("data/currency.txt")).GetDouble(), 4); //y ustawiam na odczytana wartosc pln z listy currency z dokladnoscia 4 miejsc p.p

                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.Date.AddDays(-n)), y)); //dodaje punkt o wartosci x jako date dzisiaj -iteracje i wartoscia y jako cene w tym dniu
                }
                n++;
            }

            plotModel.Series.Add(lineSeries); //dodaje linie do wykresu

            wykres.Model = plotModel; //rysuje wykres na istniejacym obiekcie
        }
        static double convert(string path, string currency)
        {
            string jsonContent = File.ReadAllText(path); //czyta plik path

            using (JsonDocument doc = JsonDocument.Parse(jsonContent)) //zamienia tekst na jsondocument
            {
                return doc.RootElement.GetProperty(currency).GetProperty(File.ReadAllText("data/currency.txt")).GetDouble(); //zwraca wartosc waluty z listy currency (np usd) i zamienia na dobule
            }
        }

        private void show_settings(object sender, RoutedEventArgs e)
        {
            var settingsPanel = new settingsPanel();
            settingsPanel.usun += hide_settings; //dodaje zdarzenie
            settings.Content = settingsPanel;
        }

        private void hide_settings()
        {
            settings.Content = null; //usuwam settingspanel
            currenciesButtons();
            custom1Button();
            custom2Button();
            downloadcustom();
        }

        private void currenciesButtons()
        {
            List<string> currencies = ["usd", "eur", "gbp", "jpy", "chf", "cad", "btc", "eth"]; //lista wszystkich walut

            DateTime date = DateTime.Now.AddDays(-1); //wczorajsza data

            foreach (string currency in currencies)
            {
                string url = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/" + currency + ".json"; //najnowszy plik z cenami
                string url_old = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@" + date.ToString("yyyy-MM-dd") + "/v1/currencies/" + currency + ".json"; //wczorajszy plik

                string path = "data/curr_" + currency + ".json";
                string path_old = "data/curr_old_" + currency + ".json";

                using (WebClient webClient = new WebClient()) //pobieram pliki
                {
                    webClient.DownloadFile(url, path);
                    webClient.DownloadFile(url_old, path_old);
                }

                double pln1 = convert(path, currency); //do pln1 przypisuje wynik funkcji pln z atrybutami path i currency

                double pln2 = convert(path_old, currency); //-||-

                Label label = FindName(currency) as Label; //zapisuje element o nazwie 'currency' jako label
                Button label2 = FindName(currency + "_button") as Button; //zapisuje element o nazwie currency_button jako label2 (button)

                if (pln1 > pln2)
                {
                    label2.Background = new SolidColorBrush(Color.FromRgb(23, 179, 28)); //zmiana kolory
                    label.Content = Math.Round(pln1, 2); //zaokrąglenie do 2 miejsc p.p
                }
                else if (pln2 > pln1)
                {
                    label2.Background = new SolidColorBrush(Color.FromRgb(194, 17, 17));
                    label.Content = Math.Round(pln1, 2);
                }
                else
                {
                    label2.Background = new SolidColorBrush(Colors.Gray);
                    label.Content = Math.Round(pln1, 2);
                }

            }

        }
        private void custom1Button()
        {
            string customcurrency1 = File.ReadAllText("data/custom1.txt");

            DateTime date = DateTime.Now.AddDays(-1); //wczorajsza data

            string url = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/" + customcurrency1 + ".json"; //najnowszy plik z cenami
            string url_old = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@" + date.ToString("yyyy-MM-dd") + "/v1/currencies/" + customcurrency1 + ".json"; //wczorajszy plik

            string path = "data/curr_" + customcurrency1 + ".json";
            string path_old = "data/curr_old_" + customcurrency1 + ".json";

            using (WebClient webClient = new WebClient()) //pobieram pliki
            {
                webClient.DownloadFile(url, path);
                webClient.DownloadFile(url_old, path_old);
            }

            double pln1 = convert(path, customcurrency1); //do pln1 przypisuje wynik funkcji pln z atrybutami path i currency

            double pln2 = convert(path_old, customcurrency1); //-||-

            custom1.Content = Math.Round(pln1, 2); //zaokrąglenie do 2 miejsc p.p

            custom_name1.Content = customcurrency1;
            custom1_button.Tag = customcurrency1;

            if (pln1 > pln2)
            {
                custom1_button.Background = new SolidColorBrush(Color.FromRgb(23, 179, 28)); //zmiana kolory
            }
            else if (pln2 > pln1)
            {
                custom1_button.Background = new SolidColorBrush(Color.FromRgb(194, 17, 17));
            }
            else
            {
                custom1_button.Background = new SolidColorBrush(Colors.Gray);
            }
        }

        private void custom2Button()
        {
            string customcurrency1 = File.ReadAllText("data/custom2.txt");

            DateTime date = DateTime.Now.AddDays(-1); //wczorajsza data

            string url = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/" + customcurrency1 + ".json"; //najnowszy plik z cenami
            string url_old = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@" + date.ToString("yyyy-MM-dd") + "/v1/currencies/" + customcurrency1 + ".json"; //wczorajszy plik

            string path = "data/curr_" + customcurrency1 + ".json";
            string path_old = "data/curr_old_" + customcurrency1 + ".json";

            using (WebClient webClient = new WebClient()) //pobieram pliki
            {
                webClient.DownloadFile(url, path);
                webClient.DownloadFile(url_old, path_old);
            }

            double pln1 = convert(path, customcurrency1); //do pln1 przypisuje wynik funkcji pln z atrybutami path i currency

            double pln2 = convert(path_old, customcurrency1); //-||-

            custom2.Content = Math.Round(pln1, 2); //zaokrąglenie do 2 miejsc p.p

            custom_name2.Content = customcurrency1;
            custom2_button.Tag = customcurrency1;

            if (pln1 > pln2)
            {
                custom2_button.Background = new SolidColorBrush(Color.FromRgb(23, 179, 28)); //zmiana kolory
            }
            else if (pln2 > pln1)
            {
                custom2_button.Background = new SolidColorBrush(Color.FromRgb(194, 17, 17));
            }
            else
            {
                custom2_button.Background = new SolidColorBrush(Colors.Gray);
            }
        }

        private void downloadall()
        {
            string customcurrency1 = File.ReadAllText("data/custom1.txt");
            string customcurrency2 = File.ReadAllText("data/custom2.txt");
            List<string> currencies = ["usd", "eur", "gbp", "jpy", "chf", "cad", "btc", "eth", customcurrency1, customcurrency2]; //lista wszystkich walut
            foreach (string currency in currencies)
            {
                List<string> urls = [ //lista adresow url plikow z cenami do pobrania
                    "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/" + currency + ".json"];

                for (int i = 1; i < 7; i++) //dodaje tyle plikow ile chce zeby wykres pokazywal (6 plikow tydzien, 30 miesiac itd)
                {
                    urls.Add("https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@" + DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd") + "/v1/currencies/" + currency + ".json");
                }

                int n = 0;

                foreach (string url in urls)
                { 
                    using (WebClient webClient = new WebClient()) //pobieram plik z neta
                    {
                        if (!Directory.Exists("data/" + currency + "/chart" + n + ".json"))
                        {
                            Directory.CreateDirectory("data/" + currency);
                        }
                        webClient.DownloadFile(url, "data/" + currency + "/chart" + n + ".json");
                    }

                    n++;
                }
            }
        }

        private async Task downloadasync()
        {
            download_text.Content = "Pobieranie danych...";
            await Task.Run(() => downloadall());
            download_text.Content = "";
        }

        private void downloadcustom()
        {
            string customcurrency1 = File.ReadAllText("data/custom1.txt");
            string customcurrency2 = File.ReadAllText("data/custom2.txt");
            List<string> currencies = [customcurrency1, customcurrency2]; //lista wszystkich walut
            foreach (string currency in currencies)
            {
                List<string> urls = [ //lista adresow url plikow z cenami do pobrania
                    "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/" + currency + ".json"];

                for (int i = 1; i < 7; i++) //dodaje tyle plikow ile chce zeby wykres pokazywal (6 plikow tydzien, 30 miesiac itd)
                {
                    urls.Add("https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@" + DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd") + "/v1/currencies/" + currency + ".json");
                }

                int n = 0;

                foreach (string url in urls)
                {
                    using (WebClient webClient = new WebClient()) //pobieram plik z neta
                    {
                        if (!Directory.Exists("data/" + currency + "/chart" + n + ".json"))
                        {
                            Directory.CreateDirectory("data/" + currency);
                        }
                        webClient.DownloadFile(url, "data/" + currency + "/chart" + n + ".json");
                    }

                    n++;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _ = downloadasync();

            currenciesButtons();

            custom1Button();

            custom2Button();

        }

    }
}