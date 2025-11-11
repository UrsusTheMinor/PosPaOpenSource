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
using System.Windows.Threading;

namespace Achterbahn;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

    }

    private int maxPlaetze;
    private int anzahlPassagiere;
    private int fahrtenAnzahl;
    private Wagen wagen;
    private List<Passagiere> pListe = new List<Passagiere>();
    private DispatcherTimer _uiTimer;

    public void Btn_Start(object sender, RoutedEventArgs e)
    {
        Button_Start.IsEnabled = false;
        Button_Stop.IsEnabled = true;

        int.TryParse(TxtPassagiere.Text, out anzahlPassagiere);
        int.TryParse(TxtFahrten.Text, out fahrtenAnzahl);
        int.TryParse(TxtPlaetze.Text, out maxPlaetze);

        wagen = new Wagen(maxPlaetze, fahrtenAnzahl);

        for(int i = 0; i < anzahlPassagiere; i++)
        {
            pListe.Add(new Passagiere(i, wagen));
        }

        foreach (Passagiere p in pListe) {
            Thread t = new Thread(p.RunP);
            t.IsBackground = true;
            t.Start();
        }

        Thread w = new Thread(wagen.WagenRun);
        w.IsBackground = true;
        w.Start();

        _uiTimer = new DispatcherTimer();
        _uiTimer.Interval = TimeSpan.FromMilliseconds(200);
        _uiTimer.Tick += UpdatePassengerList;
        _uiTimer.Start();

    }

    private void UpdatePassengerList(object? sender, EventArgs e)
    {
        lock (wagen._lockObj)
        {
            PassengerListView.Items.Clear();

            foreach (var p in pListe)
            {
                PassengerListView.Items.Add(new
                {
                    Id = p.id,
                    Status = p.status
                });
            }
        }
    }



    public void Btn_Stop(object sender, RoutedEventArgs e)
    {
        Button_Start.IsEnabled = true;
        Button_Stop.IsEnabled = false;

        lock (wagen._lockObj) //wagen freundlich zum still stand bringen
        {
            wagen.remainingRides = 0;
            wagen.boardingOpen = false;
            wagen.cartMoving = false;
            Monitor.PulseAll(wagen._lockObj);
        }

        _uiTimer.Stop();
    }
}