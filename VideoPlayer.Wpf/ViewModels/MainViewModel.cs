using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VideoPlayer.Wpf.Infrastructure;
using VideoPlayer.Wpf.Models;
using VideoPlayer.Wpf.Infrastructure;

namespace VideoPlayer.Wpf.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ApiClient _api;
    public ObservableCollection<VideoItem> Videos { get; } = new();

    private VideoItem? _selectedVideo;
    public VideoItem? SelectedVideo
    {
        get => _selectedVideo;
        set { _selectedVideo = value; OnPropertyChanged(); }
    }

    public RelayCommand RefreshCommand { get; }
    public RelayCommand PlayCommand { get; }
    public RelayCommand PauseCommand { get; }
    public RelayCommand StopCommand { get; }

    // events for the View to act on (keeps MVVM separation light)
    public event Action? PlayRequested;
    public event Action? PauseRequested;
    public event Action? StopRequested;

    public MainViewModel()
    {
        // Update this if your API uses a different port
        _api = new ApiClient(" http://localhost:5282");

        RefreshCommand = new RelayCommand(async _ => await LoadVideos());
        PlayCommand = new RelayCommand(_ => PlayRequested?.Invoke(), _ => SelectedVideo != null);
        PauseCommand = new RelayCommand(_ => PauseRequested?.Invoke());
        StopCommand = new RelayCommand(_ => StopRequested?.Invoke());

        // auto-load at startup
        _ = LoadVideos();
    }

    private async Task LoadVideos()
    {
        Videos.Clear();
        var list = await _api.GetVideosAsync() ?? new();
        foreach (var v in list) Videos.Add(v);
        SelectedVideo = Videos.FirstOrDefault();
        // When SelectedVideo changes, UI binding will update the MediaElement.Source.
        // If LoadedBehavior="Play", it will start automatically.
        PlayRequested?.Invoke();
        RaiseCanExecutes();
    }

    private void RaiseCanExecutes()
    {
        PlayCommand.RaiseCanExecuteChanged();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
