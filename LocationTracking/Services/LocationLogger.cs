using System.Text.Json;
using LocationTracking.Abstractions;
using LocationTracking.Models;

namespace LocationTracking.Services;

/// <summary>
/// Logs locations to a local file in JSON format and provides access/export features.
/// </summary>
public class LocationLogger : ILocationLogger
{
    private readonly string _logFilePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public LocationLogger()
    {
        var logFileName = "tracked_locations.json";
        var localFolder = FileSystem.AppDataDirectory;
        _logFilePath = Path.Combine(localFolder, logFileName);
    }

    public async Task LogAsync(TrackedLocation location)
    {
        await _semaphore.WaitAsync();
        try
        {
            var locations = (await ReadLogsInternalAsync()).ToList();
            locations.Add(location);

            var json = JsonSerializer.Serialize(locations, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_logFilePath, json);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<TrackedLocation>> GetAllLogsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return await ReadLogsInternalAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task ClearLogsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (File.Exists(_logFilePath))
                File.Delete(_logFilePath);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task<string> ExportLogsAsync()
    {
        // Just return the log file path (could also zip/email/share, later)
        return Task.FromResult(_logFilePath);
    }

    private async Task<IEnumerable<TrackedLocation>> ReadLogsInternalAsync()
    {
        if (!File.Exists(_logFilePath))
            return [];

        var json = await File.ReadAllTextAsync(_logFilePath);
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            var locations = JsonSerializer.Deserialize<List<TrackedLocation>>(json);
            return locations ?? Enumerable.Empty<TrackedLocation>();
        }
        catch
        {
            // Corrupted file fallback
            return [];
        }
    }
}