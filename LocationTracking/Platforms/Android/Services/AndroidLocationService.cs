using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.OS;
using LocationTracking.Abstractions;
using LocationTracking.Configuration;
using LocationTracking.Enums;
using LocationTracking.Events;
using LocationTracking.Models;

namespace LocationTracking.Services;

/// <summary>
///     A foreground service that runs location tracking even when the app is backgrounded or killed.
/// </summary>
[Service(Enabled = true, Exported = false, ForegroundServiceType = ForegroundService.TypeLocation)]
public class AndroidLocationService : Service
{
    public const string ChannelId = "location_tracking_channel";
    public const int NotificationId = 1001;
    private LocationCallback? _callback;

    private IFusedLocationProviderClient? _client;
    private ILocationLogger? _logger;
    private PowerManager.WakeLock? _wakeLock;

    public override IBinder? OnBind(Intent? intent) => null;

    public override void OnTaskRemoved(Intent? rootIntent)
    {
        if (ApplicationContext == null) return;

        var restartServiceIntent = new Intent(ApplicationContext, typeof(AndroidLocationService));
        restartServiceIntent.SetPackage(PackageName);

        var restartPendingIntent = PendingIntent.GetService(
            this, 1, restartServiceIntent, PendingIntentFlags.Immutable);

        var alarmManager = GetSystemService(AlarmService) as AlarmManager;
        if (restartPendingIntent != null)
            alarmManager?.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 1000, restartPendingIntent);
    }

    public override void OnCreate()
    {
        _logger = IPlatformApplication.Current?.Services.GetService(typeof(ILocationLogger)) as ILocationLogger;
        RegisterNotificationChannel();
        StartForeground(NotificationId, BuildNotification());
        base.OnCreate();
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        _ = Task.Run(StartTrackingAsync);

        return StartCommandResult.Sticky;
    }

    private async Task StartTrackingAsync()
    {
        try
        {
            if (_wakeLock is null)
            {
                var powerManager = GetSystemService(PowerService) as PowerManager;
                _wakeLock = powerManager?.NewWakeLock(
                    WakeLockFlags.Partial,
                    "LocationTracking"
                );
                _wakeLock?.Acquire();
            }

            _client = LocationServices.GetFusedLocationProviderClient(this);
            var options =
                IPlatformApplication.Current?.Services.GetService(typeof(LocationTrackingOptions)) as
                    LocationTrackingOptions ?? new LocationTrackingOptions();

            var request =
                new LocationRequest.Builder(GetPriority(options.Accuracy), (long)options.Interval.TotalMilliseconds)
                    .SetMinUpdateIntervalMillis((long)(options.Interval.TotalMilliseconds / 2))
                    .Build();

            _callback = new LocationCallbackImpl(_logger ??
                                                 throw new InvalidOperationException("Logger must be initialised."));

            await _client.RequestLocationUpdatesAsync(request, _callback,
                Looper.MainLooper ?? throw new InvalidOperationException("Lopper must be initialised."));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LocationService] Start error: {ex}");
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _client?.RemoveLocationUpdatesAsync(_callback ??
                                            throw new InvalidOperationException(
                                                "Instance of callback not initialised."));
        if (_wakeLock?.IsHeld ?? false)
        {
            _wakeLock?.Release();
        }

        // Schedule restart
        ScheduleRestartJob();
    }

    private void ScheduleRestartJob()
    {
        var jobId = 1010;
        var jobScheduler = GetSystemService(JobSchedulerService) as JobScheduler;

        var componentName = new ComponentName(this, Java.Lang.Class.FromType(typeof(RestartJobService)));
        var jobInfo = new JobInfo.Builder(jobId, componentName)
            .SetMinimumLatency(1000) // 1 second delay
            ?.SetOverrideDeadline(5000) // Must be executed within 5 seconds
            ?.SetRequiredNetworkType(NetworkType.Any)
            ?.Build();

        if (jobInfo != null)
            jobScheduler?.Schedule(jobInfo);
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

    private void RegisterNotificationChannel()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            return;

        var channel = new NotificationChannel(
            ChannelId,
            "Location Tracking",
            NotificationImportance.Default)
        {
            Description = "Used for background location tracking",
            LockscreenVisibility = NotificationVisibility.Public
        };

        var notificationManager = (NotificationManager)GetSystemService(NotificationService)!;
        notificationManager.CreateNotificationChannel(channel);
    }

    private Notification BuildNotification()
    {
        var builder = new Notification.Builder(this, ChannelId)
            .SetContentTitle("Location Tracking Active")
            .SetContentText("Your location is being logged in background.")
            .SetSmallIcon(Resource.Drawable.mtrl_ic_error) // You must create this icon!
            .SetOngoing(true);

        return builder.Build();
    }
}

internal class LocationCallbackImpl(ILocationLogger logger) : LocationCallback
{
    public override async void OnLocationResult(LocationResult result)
    {
        try
        {
            foreach (var loc in result.Locations)
            {
                var tracked = new TrackedLocation
                {
                    Latitude = loc.Latitude,
                    Longitude = loc.Longitude,
                    Accuracy = loc.HasAccuracy ? loc.Accuracy : null,
                    Altitude = loc.HasAltitude ? loc.Altitude : null,
                    Timestamp = DateTime.UtcNow,
                    Source = "AndroidService"
                };

                await logger.LogAsync(tracked);
                LocationEventHub.Raise(tracked);
            }
        }
        catch (Exception)
        {
            // Ignored
        }
    }
}