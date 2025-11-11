using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiningPhilosophers
{
    public sealed class Philosopher
    {
        private readonly Random _rng;
        private readonly Fork _left;
        private readonly Fork _right;
        private readonly TextBox _status;
        private readonly TextBox _log;
        private readonly Func<int> _avgThink;
        private readonly Func<int> _varThink;
        private readonly Func<int> _avgEat;
        private readonly Func<int> _varEat;
        private readonly Func<int> _pickup;

        private Thread _thread;

        public string Name { get; }
        public volatile bool Running;

        public Philosopher(
            string name,
            Fork left,
            Fork right,
            TextBox statusBox,
            Func<int> avgThink, Func<int> varThink,
            Func<int> avgEat, Func<int> varEat,
            Func<int> pickup)
        {
            Name = name;
            _left = left;
            _right = right;
            _status = statusBox;
            _avgThink = avgThink;
            _varThink = varThink;
            _avgEat = avgEat;
            _varEat = varEat;
            _pickup = pickup;
            _rng = new Random(Guid.NewGuid().GetHashCode());
        }

        public void Start()
        {
            Running = true;
            _thread = new Thread(Run) { IsBackground = true, Name = $"Philosopher-{Name}" };
            _thread.Start();
        }

        public void Stop()
        {
            Running = false;
            try { _thread?.Interrupt(); } catch { /* who cares */ }
        }

        private int Jitter(Func<int> avg, Func<int> var)
        {
            var a = Math.Max(0, avg());
            var v = Math.Max(0, var());
            int delta = _rng.Next(-v, v + 1);
            int result = a + delta;
            return result < 0 ? 0 : result;
        }

        private void UI(string text, Brush brush)
        {
            try
            {
                _status.Dispatcher.Invoke(() =>
                {
                    _status.Text = text;
                    _status.Background = brush;
                });
            }
            catch { /* UI closed */ }
        }

        

        private void Run()
        {
            try
            {
                while (Running)
                {
                    // Denken
                    UI("denkt", Brushes.White);
                    Thread.Sleep(Jitter(_avgThink, _varThink));

                    // Hunger -> greifen: LINKS, dann RECHTS (absichtlich symmetrisch => Deadlock-Gefahr)
                    UI("wartet", Brushes.Red);

                    // Simulation: Greifzeit
                    Thread.Sleep(Math.Max(0, _pickup()));

                    // linke Gabel blockierend nehmen
                    _left.Take(Name);

                    // nochmal kleine Greifzeit
                    Thread.Sleep(Math.Max(0, _pickup()));

                    // rechte Gabel blockierend nehmen (hier können wir hängen)
                    _right.Take(Name);

                    // Essen
                    UI("isst", Brushes.LightGreen);
                    Thread.Sleep(Jitter(_avgEat, _varEat));

                    // Ablegen
                    _right.PutDown();
                    _left.PutDown();
                }
            }
            catch (ThreadInterruptedException)
            {
                // Aufräumen falls nötig
                try { _right.TryPutDownOwner(Name); } catch { }
                try { _left.TryPutDownOwner(Name); } catch { }
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                UI("gestoppt", Brushes.White);
            }
        }
    }
}
