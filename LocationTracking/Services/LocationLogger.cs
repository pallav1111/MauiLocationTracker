using System.Text.Json;
using LocationTracking.Abstractions;
using LocationTracking.Models;

namespace LocationTracking.Services;

/// <summary>
/// Logs locations to a local file in JSON format and provides access/export features.
/// </summary>
internal class LocationLogger : ILocationLogger
{
    private readonly string _logFilePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public LocationLogger()
    {
        var localFolder = FileSystem.AppDataDirectory;
        _logFilePath = Path.Combine(localFolder, "tracked_locations.json");
    }
#region Location Logs

    public async Task LogAsync(TrackedLocation location)
    {
        await _semaphore.WaitAsync();
        try
        {
            var logs = (await ReadLogsInternalAsync()).ToList();
            logs.Add(location);

            var json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_logFilePath, json);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<TrackedLocation>> GetAllLocationTraceAsync()
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

    public Task<string> ExportLogsAsync() => Task.FromResult(_logFilePath);

    private async Task<IEnumerable<TrackedLocation>> ReadLogsInternalAsync()
    {
        if (!File.Exists(_logFilePath))
            return [];

        var json = await File.ReadAllTextAsync(_logFilePath);
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<TrackedLocation>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    #endregion
}