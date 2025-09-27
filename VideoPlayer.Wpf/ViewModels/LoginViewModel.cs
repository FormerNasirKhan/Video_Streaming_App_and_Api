using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VideoPlayer.Wpf.Infrastructure;

namespace VideoPlayer.Wpf.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly HttpClient _http = new() { BaseAddress = new System.Uri("http://localhost:5282") };

    private string _userId = "";
    public string UserId { get => _userId; set { _userId = value; OnPropertyChanged(); LoginCommand.RaiseCanExecuteChanged(); } }

    private string _pass = "";
    public string Pass { get => _pass; set { _pass = value; OnPropertyChanged(); LoginCommand.RaiseCanExecuteChanged(); } }

    private string? _error;
    public string? Error { get => _error; set { _error = value; OnPropertyChanged(); } }

    private bool _isBusy;
    public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged(); LoginCommand.RaiseCanExecuteChanged(); } }

    public RelayCommand LoginCommand { get; }

    public event Action? LoginSucceeded;

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(async () => await DoLoginAsync(), () => !IsBusy && !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Pass));
    }

    private async Task DoLoginAsync()
    {
        try
        {
            IsBusy = true;
            Error = null;

            var req = new { UserId, Pass };
            var resp = await _http.PostAsJsonAsync("api/auth/login", req);

            var text = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                try
                {
                    // Try parse as JSON first
                    var body = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(text);
                    Error = body?.Message ?? text;
                }
                catch
                {
                    // Fallback to raw text if not JSON
                    Error = text;
                }
                return;
            }

            // Success
            LoginSucceeded?.Invoke();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }


    private record LoginResponse(bool Success, string? Message);

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
