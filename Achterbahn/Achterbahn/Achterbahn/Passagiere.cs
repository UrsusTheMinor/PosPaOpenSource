using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Achterbahn;

public sealed class Passagiere
{

    public int? id;
    public string? status;
    private Wagen? wagen;


    public Passagiere(int _id, Wagen? _wagen)
    {
        id = _id;
        wagen = _wagen;
        status = "Warteschlange";
    }



  

    public void BoardingP()
    {
        lock (wagen._lockObj)
        {
            while (wagen.CanBoard() == false)
            {
                status = "Warteschlange";
                Monitor.Wait(wagen._lockObj);
            }

            wagen.Wagoon.Add(this);
            wagen.boardedCount++;
            status = "ImWagen";

            if (wagen.boardedCount == wagen.maxPlaetze)
            {
                Monitor.PulseAll(wagen._lockObj);
            }



            while (wagen.cartMoving || wagen.boardingOpen)     //passagiere fahren
            {
                Monitor.Wait(wagen._lockObj);
            }
        }
    }



    public void DeboardingP()
    {
        lock (wagen._lockObj) 
        {

            while (wagen.cartMoving || wagen.boardingOpen)
            {
                Monitor.Wait(wagen._lockObj);
            }


            if (wagen.Wagoon.Remove(this))
            {
                wagen.boardedCount--;
            }

            status = "ImPark";

            if (wagen.boardedCount == 0)
            {
                Monitor.PulseAll(wagen._lockObj);
            }
        }
    }

    public void RunP()
    {


        while (wagen.remainingRides > 0)
        {

            //spazieren 

            

            BoardingP();

            
            DeboardingP();

            Thread.Sleep(1000);
        }


    }


}
