using CoreLocation;
using Foundation;
using LocationTracking.Abstractions;
using LocationTracking.Configuration;
using LocationTracking.Enums;
using LocationTracking.Models;

namespace LocationTracking;

/// <summary>
/// iOS implementation of ILocationTracker using CLLocationManager.
/// </summary>
internal class LocationTrackerManager : NSObject, ILocationTracker, ICLLocationManagerDelegate
{
    private readonly CLLocationManager _locationManager;
    private readonly ILocationLogger _logger;
    private readonly LocationTrackingOptions _options;
    
    public bool IsTracking { get; private set; }

    internal LocationTrackerManager(ILocationLogger logger, LocationTrackingOptions options)
    {
        _logger = logger;
        _options = options;
        _locationManager = new CLLocationManager
        {
            Delegate = this,
            PausesLocationUpdatesAutomatically = false,
            AllowsBackgroundLocationUpdates = true
        };
    }
    
    public async Task StartTrackingAsync()
    {
        if (IsTracking) return;

        if (GetLocationAuthorizationStatus() == CLAuthorizationStatus.NotDetermined)
        {
            _locationManager.RequestAlwaysAuthorization();
            await Task.Delay(1000); // give it time (or better: subscribe to delegate later)
        }

        if (CLLocationManager.LocationServicesEnabled)
        {
            _locationManager.DesiredAccuracy = GetAccuracy(_options.Accuracy);
            _locationManager.DistanceFilter = _options.Interval.TotalSeconds < 10 ? 10 : _options.Interval.TotalSeconds;
            _locationManager.AllowsBackgroundLocationUpdates = true;
            _locationManager.PausesLocationUpdatesAutomatically = false;
            
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
    
    public Task<IEnumerable<TrackedLocation>> GetAllLocationTraceAsync() => _logger.GetAllLocationTraceAsync();

    private CLAuthorizationStatus GetLocationAuthorizationStatus() =>
        OperatingSystem.IsIOSVersionAtLeast(14) ? _locationManager.AuthorizationStatus : CLLocationManager.Status;
    
    private static double GetAccuracy(LocationAccuracy accuracy) => accuracy switch
    {
        LocationAccuracy.Lowest => CLLocation.AccuracyNearestTenMeters,
        LocationAccuracy.Low => CLLocation.AccuracyHundredMeters,
        LocationAccuracy.Balanced => CLLocation.AccuracyKilometer,
        LocationAccuracy.High => CLLocation.AccuracyThreeKilometers,
        _ => CLLocation.AccuracyBest
    };
    
    [Export("locationManager:didUpdateLocations:")]
    internal async void LocationManagerDidUpdateLocations(CLLocationManager manager, CLLocation[] locations)
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
                    Source = "iOS"
                };

                await _logger.LogAsync(tracked);
            }
        }
        catch (Exception e)
        {
            // Ignored
        }
    }

    [Export("locationManager:didFailWithError:")]
    public void Failed(CLLocationManager manager, NSError error)
    {
        Console.WriteLine($"Location error: {error.LocalizedDescription}");
    }
}