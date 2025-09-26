using System;
using System.Windows;
using System.Windows.Threading;
using VideoPlayer.Wpf.ViewModels;

namespace VideoPlayer.Wpf.Views;

public partial class MainWindow : Window
{
    private DispatcherTimer _timer;
    private MainViewModel Vm => (MainViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        Vm.PlayRequested += () => Dispatcher.Invoke(() => { if (Player.Source != null) Player.Play(); });
        Vm.PauseRequested += () => Dispatcher.Invoke(() => Player.Pause());
        Vm.StopRequested += () => Dispatcher.Invoke(() => Player.Stop());

        // Track position with timer
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

        // When user moves slider manually
        Vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Vm.PositionSeconds) && Math.Abs(Player.Position.TotalSeconds - Vm.PositionSeconds) > 1)
            {
                Player.Position = TimeSpan.FromSeconds(Vm.PositionSeconds);
            }
        };
    }

    protected override void OnClosed(EventArgs e)
    {
        Player.Close();
        base.OnClosed(e);
    }
}
