namespace LocationTracking.Abstractions;

/// <summary>
///     Abstraction for starting and stopping location tracking.
///     Platform-specific implementations will use this contract.
/// </summary>
public interface ILocationTracker
{
    /// <summary>
    ///     Checks if tracking is currently active.
    /// </summary>
    bool IsTracking { get; }

    /// <summary>
    ///     Starts location tracking with current configuration.
    /// </summary>
    Task StartTrackingAsync();

    /// <summary>
    ///     Stops location tracking.
    /// </summary>
    Task StopTrackingAsync();
}