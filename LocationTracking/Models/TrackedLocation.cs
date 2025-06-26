namespace LocationTracking.Models;

public class TrackedLocation
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double? Accuracy { get; init; }
    public double? Altitude { get; init; }
    public string? Source { get; init; } // e.g., "Android", "iOS"   
}