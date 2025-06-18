using System.Collections.Generic;
using System.Threading.Tasks;
using LocationTracking.Models;

namespace LocationTracking.Abstractions;

public interface ILocationLogger
{
    /// <summary>
    /// Logs a new tracked location.
    /// </summary>
    Task LogAsync(TrackedLocation location);

    /// <summary>
    /// Returns all logged locations.
    /// </summary>
    Task<IEnumerable<TrackedLocation>> GetAllLogsAsync();

    /// <summary>
    /// Clears all stored location logs.
    /// </summary>
    Task ClearLogsAsync();

    /// <summary>
    /// Export logs (e.g., as a file path or shareable data).
    /// </summary>
    Task<string> ExportLogsAsync();
}