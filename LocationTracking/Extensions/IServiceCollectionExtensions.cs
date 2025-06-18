using System;
using LocationTracking.Abstractions;
using LocationTracking.Configuration;
using LocationTracking.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LocationTracking.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the location tracking service and logger with dynamic configuration.
    /// </summary>
    public static IServiceCollection AddLocationTracking(this IServiceCollection services,
        Action<LocationTrackingOptions>? configure)
    {
        var options = new LocationTrackingOptions();
        configure?.Invoke(options);
        
        // Register config object
        services.AddSingleton(options);
        
        // Logger implementation
        services.AddSingleton<ILocationLogger, LocationLogger>();

        // Note: Platform-specific ILocationTracker must be registered in your MAUI app
        // e.g., via #if directives or partial class DI extensions
        // Only manager is registered here
        services.AddSingleton<ILocationTracker, LocationTrackerManager>(); // Platform delegation inside

        return services;
    }
}