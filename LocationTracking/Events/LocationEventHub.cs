using LocationTracking.Models;

namespace LocationTracking.Events;

/// <summary>
/// Optional event hub to listen to real-time location updates.
/// Use this only if you need UI or app layer to react instantly.
/// </summary>
public static class LocationEventHub
{
    /// <summary>
    /// Event raised whenever a new location is received by platform-specific tracker.
    /// Only works when app is alive.
    /// </summary>
    public static event Action<TrackedLocation>? LocationUpdated;

    internal static void Raise(TrackedLocation location)
    {
        try
        {
            LocationUpdated?.Invoke(location);
        }
        catch (Exception ex)
        {
            // Optionally log, or swallow to avoid crashing event loop
            System.Diagnostics.Debug.WriteLine($"[LocationEventHub] Error: {ex}");
        }
    }
}