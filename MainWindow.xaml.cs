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

            var plotModel = new PlotModel { Title = currency+"/pln" }; //tytul wykresy

            var lineSeries = new LineSeries //nowa linia na wykresie
            {
                Title = "Cena",
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                StrokeThickness = 2
            };

            List<string> urls = [ //lista adresow url plikow z cenami do pobrania
            "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/" + currency + ".json"];

            for (int i = 1; i<7; i++) //dodaje tyle plikow ile chce zeby wykres pokazywal (6 plikow tydzien, 30 miesiac itd)
            {
                urls.Add("https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@" + DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd") + "/v1/currencies/" + currency + ".json");
            }

            int n = 0;

            foreach (string Url in urls)
            {
                using (WebClient webClient = new WebClient()) //pobieram plik z neta
                {
                    webClient.DownloadFile(Url, "chart.json");
                }

                string jsonContent = File.ReadAllText("chart.json");

                using (JsonDocument doc = JsonDocument.Parse(jsonContent))//zamieniam tekst na jsondocument
                {
                    double y = Math.Round(doc.RootElement.GetProperty(currency).GetProperty("pln").GetDouble(), 4); //y ustawiam na odczytana wartosc pln z listy currency z dokladnoscia 4 miejsc p.p

                    lineSeries.Points.Add(new DataPoint(double.Parse(DateTime.Now.AddDays(-n).ToString("MMdd")), y)); //dodaje punkt o wartosci x jako date dzisiaj -iteracje i wartoscia y jako cene w tym dniu
                }

                n++;
            }

            plotModel.Series.Add(lineSeries); //dodaje linie do wykresu

            wykres.Model = plotModel; //rysuje wykres na istniejacym obiekcie
        }
        static double pln(string path, string currency)
        {
            string jsonContent = File.ReadAllText(path); //czyta plik path

            using (JsonDocument doc = JsonDocument.Parse(jsonContent)) //zamienia tekst na jsondocument
            {
                return doc.RootElement.GetProperty(currency).GetProperty("pln").GetDouble(); //zwraca wartosc pln z listy currency (np usd) i zamienia na dobule
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            List<string> curriencies = ["usd", "eur", "gbp", "jpy", "chf", "cad"]; //lista wszystkich walut

            DateTime date = DateTime.Now.AddDays(-1); //wczorajsza data

            foreach (string currency in curriencies)
            { 
                string url = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/"+currency+".json"; //najnowszy plik z cenami
                string url_old = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@"+date.ToString("yyyy-MM-dd")+"/v1/currencies/"+currency+".json"; //wczorajszy plik

                string path = "curr.json";
                string path_old = "curr_old.json";

                using (WebClient webClient = new WebClient()) //pobieram pliki
                {
                    webClient.DownloadFile(url, path);
                    webClient.DownloadFile(url_old, path_old);
                }

                double pln1 = pln(path, currency); //do pln1 przypisuje wynik funkcji pln z atrybutami path i currency

                double pln2 = pln(path_old, currency); //-||-

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

    }
}