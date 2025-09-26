using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using VideoPlayer.Wpf.Models;

namespace VideoPlayer.Wpf.Infrastructure;

public class ApiClient
{
    private readonly HttpClient _http;
    public ApiClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new System.Uri(baseUrl) };
    }

    public Task<List<VideoItem>?> GetVideosAsync()
        => _http.GetFromJsonAsync<List<VideoItem>>("api/videos");
}
