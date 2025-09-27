using Microsoft.AspNetCore.Mvc;
using VideoApi.Services;

namespace VideoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    public record LoginRequest(string UserId, string Pass);
    public record LoginResponse(bool Success, string? Message);

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.Pass))
            return BadRequest(new LoginResponse(false, "UserId and Pass are required."));

        var ok = await _auth.AuthenticateAsync(req.UserId, req.Pass, ct);
        if (!ok) return Unauthorized(new LoginResponse(false, "Invalid credentials."));

        return Ok(new LoginResponse(true, null));
    }
}
