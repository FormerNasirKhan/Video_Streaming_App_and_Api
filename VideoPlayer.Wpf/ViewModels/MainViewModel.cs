using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VideoPlayer.Wpf.Infrastructure;
using VideoPlayer.Wpf.Models;

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

    // Position (binds to slider)
    private double _positionSeconds;
    public double PositionSeconds
    {
        get => _positionSeconds;
        set { _positionSeconds = value; OnPropertyChanged(); }
    }

    private double _durationSeconds;
    public double DurationSeconds
    {
        get => _durationSeconds;
        set { _durationSeconds = value; OnPropertyChanged(); }
    }

    public RelayCommand RefreshCommand { get; }
    public RelayCommand PlayCommand { get; }
    public RelayCommand PauseCommand { get; }
    public RelayCommand StopCommand { get; }
    public RelayCommand NextCommand { get; }
    public RelayCommand PreviousCommand { get; }

    // Events for View
    public event Action? PlayRequested;
    public event Action? PauseRequested;
    public event Action? StopRequested;
    public event Action<double>? SeekRequested;

    public MainViewModel()
    {
        _api = new ApiClient("http://localhost:5282");

        RefreshCommand = new RelayCommand(async _ => await LoadVideos());
        PlayCommand = new RelayCommand(_ => PlayRequested?.Invoke(), _ => SelectedVideo != null);
        PauseCommand = new RelayCommand(_ => PauseRequested?.Invoke());
        StopCommand = new RelayCommand(_ => StopRequested?.Invoke());
        NextCommand = new RelayCommand(_ => NextVideo(), _ => CanNext());
        PreviousCommand = new RelayCommand(_ => PreviousVideo(), _ => CanPrevious());

        _ = LoadVideos();
    }

    private async Task LoadVideos()
    {
        Videos.Clear();
        var list = await _api.GetVideosAsync() ?? new();
        foreach (var v in list) Videos.Add(v);
        SelectedVideo = Videos.FirstOrDefault();
    }

    private void NextVideo()
    {
        if (SelectedVideo == null) return;
        var idx = Videos.IndexOf(SelectedVideo);
        if (idx < Videos.Count - 1)
            SelectedVideo = Videos[idx + 1];
        PlayRequested?.Invoke();
    }

    private void PreviousVideo()
    {
        if (SelectedVideo == null) return;
        var idx = Videos.IndexOf(SelectedVideo);
        if (idx > 0)
            SelectedVideo = Videos[idx - 1];
        PlayRequested?.Invoke();
    }

    private bool CanNext() => SelectedVideo != null && Videos.IndexOf(SelectedVideo) < Videos.Count - 1;
    private bool CanPrevious() => SelectedVideo != null && Videos.IndexOf(SelectedVideo) > 0;

    // Raise property changed
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
