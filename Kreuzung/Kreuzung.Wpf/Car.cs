using Kreuzung.Wpf.Crossroads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kreuzung.Wpf;

public class Car
{

    private static int _nextId = 0;
    public int Id { get; set; } = _nextId;
    public Direction? position { get; set; }
    public Crossroad crossroad { get; set; }

    public TextBox tb { get; set; }


    public Car(Crossroad cr)
    {
        _nextId++;
        crossroad = cr;


        // Direction
        Array values = Enum.GetValues(typeof(Direction));
        Random random = new Random();
        position = (Direction)values.GetValue(random.Next(values.Length));

        tb = new TextBox { Text = $"Car {Id} at {position}" };

        

    }

    public void Drive()
    {
        lock(crossroad._lockObj)
        {

            if (!crossroad.CanCarGo())
            {
                Monitor.Wait(crossroad._lockObj);
            }

            crossroad.Cross(this);

            Monitor.PulseAll(crossroad._lockObj);

        }

        Random random = new Random();

        Thread.Sleep(random.Next(1000, 10000));
        Drive();
    }

   
}
