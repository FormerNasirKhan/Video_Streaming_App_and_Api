namespace VideoApi.Models;

public class VideoInfo
{
    public int Id { get; set; }
    public string FileName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public long SizeBytes { get; set; }
    public string StreamUrl { get; set; } = "";
}
