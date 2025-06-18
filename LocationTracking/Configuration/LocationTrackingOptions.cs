using System;
using LocationTracking.Enums;

namespace LocationTracking.Configuration;

public class LocationTrackingOptions
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(5);
    public LocationAccuracy Accuracy { get; set; } = LocationAccuracy.Balanced;
    public bool EnableBackgroundTracking { get; set; } = true;
    
    /// <summary>
    /// Whether to log to internal logger or external service.
    /// </summary>
    public bool LogInternally { get; set; } = true;
}