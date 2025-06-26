using CoreLocation;
using Foundation;
using LocationTracking.Abstractions;
using LocationTracking.Configuration;
using LocationTracking.Enums;
using LocationTracking.Events;
using LocationTracking.Models;

namespace LocationTracking;

/// <summary>
///     macOS Catalyst implementation of ILocationTracker using CLLocationManager.
/// </summary>
internal class LocationTrackerManager : NSObject, ILocationTracker, ICLLocationManagerDelegate
{
    private readonly CLLocationManager _locationManager;
    private readonly ILocationLogger _logger;
    private readonly LocationTrackingOptions _options;

    internal LocationTrackerManager(ILocationLogger logger, LocationTrackingOptions options)
    {
        _logger = logger;
        _options = options;
        _locationManager = new CLLocationManager
        {
            AllowsBackgroundLocationUpdates = true
        };

        _locationManager.Delegate = this;
    }

    [Export("locationManager:didFailWithError:")]
    public void Failed(CLLocationManager manager, NSError error)
    {
        Console.WriteLine($"Mac Location error: {error.LocalizedDescription}");
    }

    public bool IsTracking { get; private set; }

    public async Task StartTrackingAsync()
    {
        if (IsTracking) return;

        if (GetLocationAuthorizationStatus() == CLAuthorizationStatus.NotDetermined)
        {
            _locationManager.RequestAlwaysAuthorization();
            await Task.Delay(1000);
        }

        if (CLLocationManager.LocationServicesEnabled)
        {
            _locationManager.DesiredAccuracy = GetAccuracy(_options.Accuracy);
            _locationManager.DistanceFilter = 10;

            _locationManager.StartUpdatingLocation();
            IsTracking = true;
        }
    }

    public Task StopTrackingAsync()
    {
        if (!IsTracking) return Task.CompletedTask;

        _locationManager.StopUpdatingLocation();
        IsTracking = false;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<TrackedLocation>> GetAllLocationTraceAsync()
    {
        return _logger.GetAllLocationTraceAsync();
    }

    private CLAuthorizationStatus GetLocationAuthorizationStatus()
    {
        return OperatingSystem.IsIOSVersionAtLeast(14)
            ? _locationManager.AuthorizationStatus
            : CLLocationManager.Status;
    }

    private static double GetAccuracy(LocationAccuracy accuracy)
    {
        return accuracy switch
        {
            LocationAccuracy.Lowest => 3000,
            LocationAccuracy.Low => 1000,
            LocationAccuracy.Balanced => 100,
            LocationAccuracy.High => 10,
            LocationAccuracy.Best => CLLocation.AccuracyBest,
            _ => 100
        };
    }

    [Export("locationManager:didUpdateLocations:")]
    public async void UpdatedLocation(CLLocationManager manager, CLLocation[] locations)
    {
        try
        {
            foreach (var location in locations)
            {
                var tracked = new TrackedLocation
                {
                    Latitude = location.Coordinate.Latitude,
                    Longitude = location.Coordinate.Longitude,
                    Accuracy = location.HorizontalAccuracy,
                    Altitude = location.VerticalAccuracy,
                    Timestamp = DateTime.UtcNow,
                    Source = "MacCatalyst"
                };

                await _logger.LogAsync(tracked);
                LocationEventHub.Raise(tracked);
            }
        }
        catch (Exception)
        {
            // Ignored
        }
    }
}