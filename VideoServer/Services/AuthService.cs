using Microsoft.EntityFrameworkCore;
using VideoApi.Data;
using VideoApi.Models;

namespace VideoApi.Services;

public interface IAuthService
{
    Task<bool> AuthenticateAsync(string userId, string pass, CancellationToken ct = default);
}

public class AuthService : IAuthService
{
    private readonly VideoStreamDbContext _db;
    public AuthService(VideoStreamDbContext db) => _db = db;

    public async Task<bool> AuthenticateAsync(string userId, string pass, CancellationToken ct = default)
    {
        // 1) Execute stored proc
        var list = await _db.AuthResults
            .FromSqlInterpolated($@"EXEC dbo.sp_Login_Authenticate @UserId={userId}, @Pass={pass}")
            .AsNoTracking()
            .ToListAsync(ct);             // <-- materialize first (no composition)

        // 2) Now query in memory
        var result = list.FirstOrDefault();
        return result?.IsValid == 1;
    }
}
