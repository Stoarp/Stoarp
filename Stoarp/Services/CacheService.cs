using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Microsoft.EntityFrameworkCore;

namespace Stoarp.Services;

public sealed class CacheService : IDisposable
{
    private readonly string _root;
    private readonly HttpClient _http;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly CacheDbContext _db;

    public CacheService(string? rootPath = null, HttpClient? http = null)
    {
        _root = rootPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Stoarp",
            "cache"
        );

        Directory.CreateDirectory(_root);
        _http = http ?? new HttpClient();
        
        _db = new CacheDbContext();
        _db.Database.EnsureCreated();
    }

    public async Task<Stream> GetAsync(string url, TimeSpan? maxAge = null)
    {
        await _lock.WaitAsync();
        try
        {
            var entry = await _db.CacheEntries
                .FirstOrDefaultAsync(e => e.Key == url);

            if (entry != null && !IsExpired(entry, maxAge) && !string.IsNullOrEmpty(entry.LocalPath))
            {
                var fullPath = Path.Combine(_root, entry.LocalPath);
                if (File.Exists(fullPath))
                {
                    entry.LastAccessedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                    return File.OpenRead(fullPath);
                }
            }
        }
        finally
        {
            _lock.Release();
        }

        var bytes = await _http.GetByteArrayAsync(url);
        var ext = GetExtension(url);
        var fileName = $"{Guid.NewGuid()}{ext}";
        
        await _lock.WaitAsync();
        try
        {
            await WriteAtomicAsync(Path.Combine(_root, fileName), bytes);
            
            var cacheEntry = new CacheEntry
            {
                Id = Guid.NewGuid(),
                Key = url,
                LocalPath = fileName,
                CreatedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow,
                ExpiresAt = maxAge.HasValue ? DateTime.UtcNow + maxAge.Value : null
            };
            
            _db.CacheEntries.Add(cacheEntry);
            await _db.SaveChangesAsync();
        }
        finally
        {
            _lock.Release();
        }

        return new MemoryStream(bytes, writable: false);
    }

    public async Task<Stream> GetAnimatedAsync(string url, TimeSpan? maxAge = null)
    {
        return await GetAsync(url, maxAge);
    }

    public async Task<Bitmap> GetBitmapAsync(string url, TimeSpan? maxAge = null)
    {
        var stream = await GetAsync(url, maxAge);
        return new Bitmap(stream);
    }

    public async Task<byte[]> GetBytesAsync(string url, TimeSpan? maxAge = null)
    {
        await using var stream = await GetAsync(url, maxAge);
        return await ReadAllAsync(stream);
    }

    public async Task<string> GetTextAsync(string url, TimeSpan? maxAge = null)
    {
        var data = await GetBytesAsync(url, maxAge);
        return System.Text.Encoding.UTF8.GetString(data);
    }

    public async Task<bool> ExistsAsync(string url)
    {
        var entry = await _db.CacheEntries
            .FirstOrDefaultAsync(e => e.Key == url);
        
        if (entry == null || string.IsNullOrEmpty(entry.LocalPath))
            return false;
            
        return File.Exists(Path.Combine(_root, entry.LocalPath));
    }

    public async Task ClearAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (Directory.Exists(_root))
                Directory.Delete(_root, recursive: true);

            Directory.CreateDirectory(_root);
            
            _db.CacheEntries.RemoveRange(_db.CacheEntries);
            await _db.SaveChangesAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<long> GetCacheSizeAsync()
    {
        if (!Directory.Exists(_root))
            return 0;

        long size = 0;
        foreach (var file in Directory.EnumerateFiles(_root, "*", SearchOption.AllDirectories))
            size += new FileInfo(file).Length;

        return size;
    }

    public async Task<CacheEntry?> GetCachedEntryAsync(string key)
    {
        return await _db.CacheEntries
            .FirstOrDefaultAsync(e => e.Key == key);
    }

    public async Task SetCacheEntryAsync(string key, string? localPath = null, string? value = null, TimeSpan? maxAge = null)
    {
        var existing = await _db.CacheEntries
            .FirstOrDefaultAsync(e => e.Key == key);

        if (existing != null)
        {
            existing.LastAccessedAt = DateTime.UtcNow;
            if (localPath != null) existing.LocalPath = localPath;
            if (value != null) existing.Value = value;
            if (maxAge.HasValue) existing.ExpiresAt = DateTime.UtcNow + maxAge.Value;
        }
        else
        {
            var entry = new CacheEntry
            {
                Id = Guid.NewGuid(),
                Key = key,
                LocalPath = localPath,
                Value = value,
                CreatedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow,
                ExpiresAt = maxAge.HasValue ? DateTime.UtcNow + maxAge.Value : null
            };
            _db.CacheEntries.Add(entry);
        }

        await _db.SaveChangesAsync();
    }

    private static async Task<byte[]> ReadAllAsync(Stream input)
    {
        using var ms = new MemoryStream();
        await input.CopyToAsync(ms);
        return ms.ToArray();
    }

    private static bool IsExpired(CacheEntry entry, TimeSpan? maxAge)
    {
        if (maxAge is null && entry.ExpiresAt is null) return false;
        if (maxAge is null) return DateTime.UtcNow > entry.ExpiresAt;
        return DateTime.UtcNow - entry.LastAccessedAt > maxAge;
    }

    private async Task WriteAtomicAsync(string path, byte[] data)
    {
        var temp = path + ".tmp";

        await File.WriteAllBytesAsync(temp, data);

        if (File.Exists(path))
            File.Delete(path);

        File.Move(temp, path);
    }

    private static string GetExtension(string url)
    {
        try
        {
            var ext = Path.GetExtension(new Uri(url).AbsolutePath);
            if (!string.IsNullOrWhiteSpace(ext))
                return ext.ToLower();
        }
        catch
        {
        }
        return ".bin";
    }

    public void Dispose()
    {
        _http.Dispose();
        _lock.Dispose();
        _db.Dispose();
    }
}