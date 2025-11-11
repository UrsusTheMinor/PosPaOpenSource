using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Achterbahn;

public class Wagen
{
    public List<Passagiere> Wagoon;

    public int maxPlaetze;
    public int maxFahrten;
    public int boardedCount;

    public bool boardingOpen;
    public bool cartMoving;

    public object _lockObj;

    public int remainingRides;




    public Wagen(int _maxPlaetze, int _maxFahrten)
    {
        Wagoon = new List<Passagiere>();
        maxPlaetze = _maxPlaetze;
        maxFahrten = _maxFahrten;
        boardedCount = 0;
        boardingOpen = true;
        cartMoving = false;
        remainingRides = maxFahrten;
        _lockObj = new object();
    }

    public bool CanBoard()
    {
        if (boardingOpen && cartMoving == false && boardedCount < maxPlaetze)
        {
            return true;
        }
        return false;
    }

    public void BoardingWagen()
    {

        lock (_lockObj)
        {

            boardingOpen = true;
            Monitor.PulseAll(_lockObj);

            while (boardedCount < maxPlaetze)         //warten auf alle gäste
            {
                Monitor.Wait(_lockObj);
            }

            boardingOpen = false;
            cartMoving = true;
        }

        Thread.Sleep(500);
    }

    public void DeboardingWagen()
    {


        lock (_lockObj)
        {
            cartMoving = false;
            Monitor.PulseAll(_lockObj);

            while (boardedCount > 0)
            {
                Monitor.Wait(_lockObj);
            }
            remainingRides--;
        }
    }



    public void WagenRun()
    {


        while (remainingRides > 0)
        {
            BoardingWagen();

            DeboardingWagen();

            if(remainingRides == 0)
            {
                lock (_lockObj)
                {
                    cartMoving = false;
                    boardingOpen = false;
                    Monitor.PulseAll(_lockObj);
                }
            }

        }

    }
}
