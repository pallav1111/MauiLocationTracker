
# <img src="https://raw.githubusercontent.com/pallav1111/MauiLocationTracker/main/logo.png" alt="Logo" width="100"/>
MauiLocationTracker

**Cross-platform location tracking library for .NET MAUI**
Provides seamless background location updates on **Android** and **iOS**, with support for event-based live tracking, persistent logging, and export features.

---

## ✨ Features

* ✅ Background location tracking
* ✅ Continues tracking even when the app is minimized
* ✅ Foreground service support on Android
* ✅ CLLocationManager-based tracking on iOS
* ✅ Internal logging of tracked locations (in JSON format)
* ✅ Export and clear logs
* ✅ Realtime location updates via `LocationEventHub`
* ✅ Easy integration with .NET MAUI (Android & iOS)

---

## 📦 Installation

```bash
dotnet add package MauiLocationTracker
```

Or search for `MauiLocationTracker` on [NuGet](https://www.nuget.org/packages/MauiLocationTracker).

---

## ⚙️ Configuration

### Register Services

In your `MauiProgram.cs`:

```csharp
using LocationTracking;

builder.Services.AddLocationTracking();
```

This registers:

* `ILocationTracker` — controls location tracking lifecycle
* `ILocationLogger` — handles log persistence
* `LocationEventHub` — broadcasts location updates

---

## 📍 Basic Usage

### Start Tracking

```csharp
private readonly ILocationTracker _tracker;

public YourPage(ILocationTracker tracker)
{
    _tracker = tracker;
}

await _tracker.StartTrackingAsync();
```

### Stop Tracking

```csharp
await _tracker.StopTrackingAsync();
```

---

## ⚡ Live Location Updates

Subscribe to location changes:

```csharp
LocationEventHub.OnLocationReceived += location =>
{
    Console.WriteLine($"Location: {location.Latitude}, {location.Longitude}");
};
```

Unsubscribe when no longer needed:

```csharp
LocationEventHub.OnLocationReceived -= yourHandler;
```

---

## 🧾 Working with Logs

### Read all logs:

```csharp
var logs = await locationLogger.GetAllLocationTraceAsync();
```

### Clear stored logs:

```csharp
await locationLogger.ClearLogsAsync();
```

### Export log file path:

```csharp
var path = await locationLogger.ExportLogsAsync();
```

---

## 🛠 Android Setup

In your \*\*app project’s \*\*\`AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
<uses-permission android:name="android.permission.WAKE_LOCK" />
```

---

## 🍏 iOS Setup

In your \`Info.plist`:

```xml
<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>We need your location for background tracking.</string>
<key>UIBackgroundModes</key>
<array>
    <string>location</string>
</array>
```

---

## ✅ Roadmap

* Android/iOS background tracking ✅

* Event-based update support ✅

* desktop support

...... Loading

