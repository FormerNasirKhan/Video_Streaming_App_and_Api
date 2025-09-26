using System.IO;

namespace VideoApi.Services;

public class VideoLibrary
{
    private readonly string _folder;
    private static readonly string[] Allowed = [".mp4", ".mkv", ".mov", ".avi", ".wmv"];

    public VideoLibrary(string folder)
    {
        _folder = folder;
    }

    public IReadOnlyList<FileInfo> GetVideoFiles()
    {
        if (!Directory.Exists(_folder)) return Array.Empty<FileInfo>();
        return new DirectoryInfo(_folder)
            .EnumerateFiles("*.*", SearchOption.TopDirectoryOnly)
            .Where(f => Allowed.Contains(f.Extension, StringComparer.OrdinalIgnoreCase))
            .OrderBy(f => f.Name)
            .ToList();
    }

    public FileInfo? GetByIndex(int id)
    {
        var files = GetVideoFiles();
        return (id >= 1 && id <= files.Count) ? files[id - 1] : null;
    }
}
