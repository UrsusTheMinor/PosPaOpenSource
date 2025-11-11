using System;
using System.Threading;

namespace DiningPhilosophers
{
    public sealed class Fork
    {
        private readonly object _sync = new object();
        private string _owner = null;

        // Blockierendes Nehmen: kein TryEnter, keine Ordnung -> Deadlock möglich
        public void Take(string philosopherName)
        {
            lock (_sync)
            {
                // Warten, bis frei. Ja, lock already excludes others;
                // diese Schleife ist nur „ehrlich“ bzgl. Besitzername.
                while (_owner != null)
                {
                    Monitor.Wait(_sync);
                }
                _owner = philosopherName;
            }
        }

        public void PutDown()
        {
            lock (_sync)
            {
                _owner = null;
                Monitor.PulseAll(_sync);
            }
        }

        // Für „Stop“: lege nur ab, wenn ich Besitzer bin
        public void TryPutDownOwner(string philosopherName)
        {
            lock (_sync)
            {
                if (_owner == philosopherName)
                {
                    _owner = null;
                    Monitor.PulseAll(_sync);
                }
            }
        }

        public override string ToString() => _owner == null ? "frei" : $"besetzt von {_owner}";
    }
}
