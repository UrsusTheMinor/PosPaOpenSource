using Kreuzung.Wpf.Crossroads;
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

namespace Kreuzung.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    public Crossroad cr { get; set; }
    public DispatcherTimer _uiTimer { get; set; }

    public List<Car> CarList { get; set; } = new List<Car>();

    


    public MainWindow()
    {
        InitializeComponent();

        chooseCrossroad.Items.Add(typeof(Crossroad));

    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        start.IsEnabled = false;

        int.TryParse(countCars.Text, out int count);

        var selectedType = chooseCrossroad.SelectedItem;
        if (selectedType == null)
        {
            return;
        }

        try
        {
            object? instance = Activator.CreateInstance((Type)selectedType);

            cr = (Crossroad)instance!;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not create instance of {selectedType}: {ex.Message}");
        }

        for (int i = 0; i < count; i++)
        {
            var c = new Car(cr);
            CarList.Add(c);


            Thread t = new Thread(c.Drive);
            t.IsBackground = true;
            t.Start();

            
        }

        _uiTimer = new DispatcherTimer();
        _uiTimer.Interval = TimeSpan.FromMilliseconds(200);
        _uiTimer.Tick += UpdatePositions;
        _uiTimer.Start();
    }
    private void UpdatePositions(object? sender, EventArgs e)
    {
        foreach (var c in CarList)
        {
            c.tb.Text = $"Car {c.Id} at {c.position}";

            north.Items.Remove(c.tb);
            east.Items.Remove(c.tb);
            south.Items.Remove(c.tb);
            west.Items.Remove(c.tb);
            crossroad.Items.Remove(c.tb); // also remove from the middle listbox

            if (c.position == Direction.Nord)
                north.Items.Add(c.tb);
            else if (c.position == Direction.Ost)
                east.Items.Add(c.tb);
            else if (c.position == Direction.Süd)
                south.Items.Add(c.tb);
            else if (c.position == Direction.West)
                west.Items.Add(c.tb);
            else if (c.position == null)
                crossroad.Items.Add(c.tb);
        }
    }
}