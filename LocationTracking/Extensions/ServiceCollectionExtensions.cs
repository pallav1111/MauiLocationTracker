using LocationTracking.Abstractions;
using LocationTracking.Configuration;
using LocationTracking.Services;

namespace LocationTracking.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the location tracking service and logger with dynamic configuration.
    /// </summary>
    public static IServiceCollection AddLocationTracking(this IServiceCollection services,
        Action<LocationTrackingOptions>? configure)
    {
        var options = new LocationTrackingOptions();
        configure?.Invoke(options);

        // Register config object
        services.AddSingleton(options);

        services.AddSingleton<ILocationLogger, LocationLogger>();
        services.AddSingleton<ILocationTracker, LocationTrackerManager>(); // Platform delegation inside

        return services;
    }
}