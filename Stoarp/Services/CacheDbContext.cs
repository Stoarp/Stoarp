using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Stoarp.Services;

public class CacheDbContext : DbContext
{
    public DbSet<CacheEntry> CacheEntries { get; set; } = null!;

    private readonly string _dbPath;

    public CacheDbContext()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Stoarp",
            "cache.db"
        );
        _dbPath = folder;
    }

    public CacheDbContext(DbContextOptions<CacheDbContext> options) : base(options)
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Stoarp",
            "cache.db"
        );
        _dbPath = folder;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CacheEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key);
            entity.Property(e => e.Key).IsRequired();
        });
    }
}