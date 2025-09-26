using Microsoft.AspNetCore.Mvc;
using VideoApi.Models;
using VideoApi.Services;

namespace VideoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideosController : ControllerBase
{
    private readonly VideoLibrary _library;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;

    public VideosController(IConfiguration config, IWebHostEnvironment env)
    {
        _config = config;
        _env = env;
        _library = new VideoLibrary(_config["VideoFolder"] ?? "");
    }

    // GET api/videos
    [HttpGet]
    public ActionResult<IEnumerable<VideoInfo>> List()
    {
        var files = _library.GetVideoFiles();
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var list = files.Select((f, idx) => new VideoInfo
        {
            Id = idx + 1,
            FileName = f.Name,
            DisplayName = Path.GetFileNameWithoutExtension(f.Name),
            SizeBytes = f.Length,
            StreamUrl = $"{baseUrl}/api/videos/{idx + 1}/stream"
        }).ToList();

        return Ok(list);
    }

    // GET api/videos/{id}/stream
    [HttpGet("{id:int}/stream")]
    public IActionResult Stream(int id)
    {
        var file = _library.GetByIndex(id);
        if (file is null || !file.Exists) return NotFound();

        // enable HTTP range processing for seeking
        var contentType = "video/mp4";
        return PhysicalFile(file.FullName, contentType, enableRangeProcessing: true);
    }
}
