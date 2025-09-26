using System;
using System.Windows;
using VideoPlayer.Wpf.ViewModels;

namespace VideoPlayer.Wpf.Views;

public partial class MainWindow : Window
{
    private MainViewModel Vm => (MainViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        Vm.PlayRequested += () => Dispatcher.Invoke(() => { if (Player.Source != null) Player.Play(); });
        Vm.PauseRequested += () => Dispatcher.Invoke(() => Player.Pause());
        Vm.StopRequested += () => Dispatcher.Invoke(() => Player.Stop());
    }

    protected override void OnClosed(EventArgs e)
    {
        Player.Close();
        base.OnClosed(e);
    }
}
