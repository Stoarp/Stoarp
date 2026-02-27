using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Labs.Controls;

namespace Stoarp.Services;

public sealed class CacheService : IDisposable
{
    private readonly string _root;
    private readonly HttpClient _http;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public CacheService(string? rootPath = null, HttpClient? http = null)
    {
        _root = rootPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Stoarp",
            "cache"
        );

        Directory.CreateDirectory(_root);
        _http = http ?? new HttpClient();
    }

    /// <summary>
    /// Generic raw download plus cache
    /// </summary>
    public async Task<Stream> GetAsync(string url, TimeSpan? maxAge = null)
    {
        var path = GetCachePath(url);

        await _lock.WaitAsync();
        try
        {
            if (File.Exists(path) && !IsExpired(path, maxAge))
                return File.OpenRead(path);
        }
        finally
        {
            _lock.Release();
        }

        var bytes = await _http.GetByteArrayAsync(url);
        await WriteAtomicAsync(path, bytes);

        return new MemoryStream(bytes, writable: false);
    }

    /// <summary>
    /// Returns cached GIF/WebP/animated asset as a Stream for AsyncImage
    /// </summary>
    public async Task<Stream> GetAnimatedAsync(string url, TimeSpan? maxAge = null)
    {
        // Just fetch as raw bytes since AsyncImage will decode and animate
        return await GetAsync(url, maxAge);
    }

    /// <summary>
    /// Returns Bitmap for static images
    /// </summary>
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
        return Encoding.UTF8.GetString(data);
    }

    public Task<bool> ExistsAsync(string url)
        => Task.FromResult(File.Exists(GetCachePath(url)));

    public Task ClearAsync()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);

        Directory.CreateDirectory(_root);

        return Task.CompletedTask;
    }

    public Task<long> GetCacheSizeAsync()
    {
        if (!Directory.Exists(_root))
            return Task.FromResult(0L);

        long size = 0;
        foreach (var file in Directory.EnumerateFiles(_root, "*", SearchOption.AllDirectories))
            size += new FileInfo(file).Length;

        return Task.FromResult(size);
    }

    private static async Task<byte[]> ReadAllAsync(Stream input)
    {
        using var ms = new MemoryStream();
        await input.CopyToAsync(ms);
        return ms.ToArray();
    }

    private static bool IsExpired(string path, TimeSpan? maxAge)
    {
        if (maxAge is null) return false;
        return DateTime.UtcNow - File.GetLastWriteTimeUtc(path) > maxAge;
    }

    private async Task WriteAtomicAsync(string path, byte[] data)
    {
        var temp = path + ".tmp";

        await File.WriteAllBytesAsync(temp, data);

        if (File.Exists(path))
            File.Delete(path);

        File.Move(temp, path);
    }

    private string GetCachePath(string url)
    {
        var hash = Hash(url);
        var ext = Path.GetExtension(new Uri(url).AbsolutePath);

        // Force .gif for animated assets if extension is GIF
        if (!string.IsNullOrWhiteSpace(ext))
            ext = ext.ToLower();

        if (string.IsNullOrWhiteSpace(ext))
            ext = ".bin";

        return Path.Combine(_root, $"{hash}{ext}");
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder(bytes.Length * 2);

        foreach (var b in bytes)
            sb.Append(b.ToString("x2"));

        return sb.ToString();
    }

    public void Dispose()
    {
        _http.Dispose();
        _lock.Dispose();
    }
}