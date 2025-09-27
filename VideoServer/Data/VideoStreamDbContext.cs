using Microsoft.EntityFrameworkCore;
using VideoApi.Models;

namespace VideoApi.Data;

public class VideoStreamDbContext : DbContext
{
    public VideoStreamDbContext(DbContextOptions<VideoStreamDbContext> options) : base(options) { }

    public DbSet<AuthResult> AuthResults => Set<AuthResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthResult>(e => e.HasNoKey());
        base.OnModelCreating(modelBuilder);
    }
}
