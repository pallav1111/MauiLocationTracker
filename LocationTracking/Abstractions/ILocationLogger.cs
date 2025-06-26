using LocationTracking.Models;

namespace LocationTracking.Abstractions;

/// <summary>
///     Handles persistence and retrieval of location tracking logs.
/// </summary>
public interface ILocationLogger
{
    /// <summary>
    ///     Logs a new tracked location.
    /// </summary>
    internal Task LogAsync(TrackedLocation location);

    /// <summary>
    ///     Returns all logged locations.
    /// </summary>
    Task<IEnumerable<TrackedLocation>> GetAllLocationTraceAsync();

    /// <summary>
    ///     Clears all stored location logs.
    /// </summary>
    Task ClearLogsAsync();
}