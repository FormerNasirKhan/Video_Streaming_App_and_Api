using System;
using System.Windows;
using System.Windows.Threading;
using VideoPlayer.Wpf.ViewModels;

namespace VideoPlayer.Wpf.Views
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private MainViewModel Vm => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            // Hook up ViewModel events
            Vm.PlayRequested += OnPlayRequested;
            Vm.PauseRequested += OnPauseRequested;
            Vm.StopRequested += OnStopRequested;
            Vm.SeekRequested += OnSeekRequested;

            // Timer for progress updates
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) =>
            {
                if (Player.NaturalDuration.HasTimeSpan)
                {
                    Vm.DurationSeconds = Player.NaturalDuration.TimeSpan.TotalSeconds;
                    Vm.PositionSeconds = Player.Position.TotalSeconds;
                }
            };
            _timer.Start();
        }

        private void OnPlayRequested()
        {
            if (Vm.SelectedVideo != null)
            {
                // Always set Source when playing new video
                Player.Source = Vm.SelectedVideo.StreamUrl;
                Player.Play();
            }
        }

        private void OnPauseRequested() => Player.Pause();

        private void OnStopRequested()
        {
            Player.Stop();
            Player.Position = TimeSpan.Zero;
        }

        private void OnSeekRequested(double seconds)
        {
            Player.Position = TimeSpan.FromSeconds(seconds);
        }

        protected override void OnClosed(EventArgs e)
        {
            Player.Close();
            base.OnClosed(e);
        }
    }
}
