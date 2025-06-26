using Android.Content;
using Android.Gms.Location;
using Android.OS;
using LocationTracking.Abstractions;
using LocationTracking.Configuration;
using LocationTracking.Enums;
using LocationTracking.Models;
using LocationTracking.Services;

namespace LocationTracking;

/// <summary>
/// Android implementation of ILocationTracker using FusedLocationProviderClient.
/// </summary>
public class LocationTrackerManager(ILocationLogger logger, LocationTrackingOptions options) : ILocationTracker
{
    private readonly Context _context = Android.App.Application.Context;

    private IFusedLocationProviderClient? _locationClient;
    private LocationRequest? _locationRequest;
    private LocationCallback? _locationCallback;

    public bool IsTracking { get; private set; }

    public async Task StartTrackingAsync()
    {
        if (IsTracking) return;

        var hasPermission = await CheckPermissionsAsync();
        if (!hasPermission)
            throw new PermissionDeniedException("Location permission not granted");

        if (options.EnableBackgroundTracking)
        {
            var intent = new Intent(_context, typeof(AndroidLocationService));
            _context.StartForegroundService(intent);
        }
        else
        {
            _locationClient ??= LocationServices.GetFusedLocationProviderClient(_context);

            _locationRequest = new LocationRequest.Builder(GetPriority(options.Accuracy), (long)options.Interval.TotalMilliseconds)
                .SetMinUpdateIntervalMillis((long)(options.Interval.TotalMilliseconds / 2))
                .Build();

            _locationCallback = new FusedLocationCallback(logger);

            _locationClient.RequestLocationUpdates(_locationRequest, _locationCallback, Looper.MainLooper);
        }
        
        IsTracking = true;
    }

    public async Task StopTrackingAsync()
    {
        if (!IsTracking) return;

        var intent = new Intent(_context, typeof(AndroidLocationService));
        _context.StopService(intent);
        
        if (_locationClient != null && _locationCallback != null)
        {
            await _locationClient.RemoveLocationUpdatesAsync(_locationCallback);
        }

        IsTracking = false;
    }

    private static int GetPriority(LocationAccuracy accuracy) => accuracy switch
    {
        LocationAccuracy.Lowest => Priority.PriorityLowPower,
        LocationAccuracy.Low => Priority.PriorityLowPower,
        LocationAccuracy.Balanced => Priority.PriorityBalancedPowerAccuracy,
        LocationAccuracy.High => Priority.PriorityHighAccuracy,
        LocationAccuracy.Best => Priority.PriorityHighAccuracy,
        _ => Priority.PriorityBalancedPowerAccuracy
    };

    private static async Task<bool> CheckPermissionsAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationAlways>();
        }
        return status == PermissionStatus.Granted;
    }

    /// <summary>
    /// Internal callback to handle received location updates.
    /// </summary>
    private sealed class FusedLocationCallback(ILocationLogger logger) : LocationCallback
    {
        public override async void OnLocationResult(LocationResult result)
        {
            try
            {
                if (result.Locations.Count == 0) return;

                foreach (var loc in result.Locations)
                {
                    var tracked = new TrackedLocation
                    {
                        Latitude = loc.Latitude,
                        Longitude = loc.Longitude,
                        Accuracy = loc.HasAccuracy ? loc.Accuracy : null,
                        Altitude = loc.HasAltitude ? loc.Altitude : null,
                        Timestamp = DateTime.UtcNow,
                        Source = "Android"
                    };

                    await logger.LogAsync(tracked);
                }
            }
            catch (Exception)
            {
                // await logger.LogAsync(e);
            }
        }
    }

    private class PermissionDeniedException(string message) : Exception(message);
}