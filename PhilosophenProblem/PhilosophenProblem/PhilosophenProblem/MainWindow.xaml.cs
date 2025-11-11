using DiningPhilosophers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace PhilosophenProblem
{
    public partial class MainWindow : Window
    {
        private readonly List<Philosopher> _philosophers = new();
        private readonly List<Fork> _forks = new();
        private bool _running;

        public MainWindow()
        {
            InitializeComponent();
            InitStatuses();
        }

        private void InitStatuses()
        {
            foreach (var tb in new[] { Status0, Status1, Status2, Status3, Status4 })
            {
                tb.Text = "bereit";
            }
        }

        private int ReadInt(TextBox tb, int fallback)
        {
            if (int.TryParse(tb.Text.Trim(), out var v)) return v;
            tb.Text = fallback.ToString();
            return fallback;
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_running) return;

            // Eingaben-Lieferanten (delegates, damit live veränderbar)
            Func<int> avgThink = () => AvgThinkBox.Dispatcher.Invoke(() => ReadInt(AvgThinkBox, 1000));
            Func<int> varThink = () => VarThinkBox.Dispatcher.Invoke(() => ReadInt(VarThinkBox, 200));
            Func<int> avgEat = () => AvgEatBox.Dispatcher.Invoke(() => ReadInt(AvgEatBox, 200));
            Func<int> varEat = () => VarEatBox.Dispatcher.Invoke(() => ReadInt(VarEatBox, 40));
            Func<int> pickup = () => PickupBox.Dispatcher.Invoke(() => ReadInt(PickupBox, 40));

            // Forks anlegen
            _forks.Clear();
            for (int i = 0; i < 5; i++) _forks.Add(new Fork());

            // Namen
            var names = new[] { Name0.Text, Name1.Text, Name2.Text, Name3.Text, Name4.Text };
            var statusBoxes = new[] { Status0, Status1, Status2, Status3, Status4 };
            

            // Philosophen anlegen: jeder nimmt links, dann rechts (symmetrisch!)
            _philosophers.Clear();
            for (int i = 0; i < 5; i++)
            {
                var left = _forks[i];
                var right = _forks[(i + 1) % 5];
                var p = new Philosopher(
                    names[i],
                    left, right,
                    statusBoxes[i], 
                    avgThink, varThink, avgEat, varEat, pickup
                );
                _philosophers.Add(p);
            }

            // Logs leeren

            // Threads starten
            foreach (var p in _philosophers) p.Start();

            _running = true;
            StartBtn.IsEnabled = false;
            StopBtn.IsEnabled = true;
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_running) return;

            // Schranke setzen
            foreach (var p in _philosophers) p.Stop();

            // „Anstupsen“, falls sie irgendwo schlafen
            foreach (var p in _philosophers)
            {
                try { Thread.Sleep(1); } catch { }
            }

            _running = false;
            StartBtn.IsEnabled = true;
            StopBtn.IsEnabled = false;
        }
    }
}
