using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kreuzung.Wpf.Crossroads;

public class Crossroad
{

    public object _lockObj { get; set; } = new object();

    public Car? car { get; set; }


    public void Cross(Car c)
    {
        car = c;
        var old = c.position;


        c.position = null;
        Thread.Sleep(1000);

        c.position = (Direction) (((int)old + 2) % 4);

        car = null;

    }

    public bool CanCarGo()
    {
        return car == null;
    }
}
