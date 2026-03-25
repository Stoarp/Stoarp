using System;

namespace Stoarp.Services;

public class CacheEntry
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? LocalPath { get; set; }
    public string? Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime LastAccessedAt { get; set; }
}