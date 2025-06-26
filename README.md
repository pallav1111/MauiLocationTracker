
# <img src="https://raw.githubusercontent.com/pallav1111/MauiLocationTracker/main/logo.png" alt="Logo" width="100"/>
MauiLocationTracker

A lightweight, cross-platform **.NET MAUI library** for tracking user location in the foreground and background on **Android** and **iOS**. Built for developers who want simple, observable, and extensible location tracking with optional logging.

[![NuGet](https://img.shields.io/nuget/v/LocationTracking.svg)](https://www.nuget.org/packages/LocationTracking)

---

## ✨ Features

- ✅ Cross-platform support (Android & iOS)
- 🛰️ Background location tracking (via ForegroundService on Android)
- 🔔 Live location update events
- 📁 Internal logging to local file (optional)
- 💡 Simple API via `ILocationTracker` interface
- ⚙️ Configurable tracking options: interval, accuracy, background behavior

---

## 📦 Installation

```bash
dotnet add package MauiLocationTracker
````

Or use the NuGet Package Manager in Visual Studio.

---

## 🚀 Getting Started

1. **Register services**

```csharp
using LocationTracking;

builder.Services.AddLocationTracking();
```

2. **Request location permissions**

```csharp
await Permissions.RequestAsync<Permissions.LocationAlways>();
```

3. **Start tracking**

```csharp
await _tracker.StartTrackingAsync();
```

4. **Listen to live updates**

```csharp
LocationEventHub.OnLocationReceived += location =>
{
    Console.WriteLine($"Lat: {location.Latitude}, Lng: {location.Longitude}");
};
```

📖 Full docs: [Getting Started](docs/getting-started.md)

---

## ⚙️ Configuration

Configure the tracking via `LocationTrackingOptions`:

```csharp
builder.Services.AddLocationTracking(options =>
{
    options.Interval = TimeSpan.FromMinutes(2);
    options.Accuracy = LocationAccuracy.High;
    options.EnableBackgroundTracking = true;
});
```

📘 See [Configuration](docs/configuration.md)

---

## 📱 Platform Setup

Make sure you configure:

* `AndroidManifest.xml`: location permissions + foreground service
* `Info.plist` for iOS: location background modes & usage description

🧭 See [Platform Setup Guide](docs/platform-setup.md)

---

## 🧪 Advanced Usage

* Access logged data
* Export logs
* Customize logger

🧠 [Advanced Usage](docs/advanced-usage.md)

---

## 🛠 Troubleshooting

* App killed = background tracking stops on Android unless JobScheduler is configured.
* iOS requires correct background modes.
* Ensure permissions are requested **before** starting tracking.

🆘 [Troubleshooting Guide](docs/troubleshooting.md)

---

## 🙋 Contributing

Have an idea or bug to report?
Feel free to [open an issue](https://github.com/pallav1111/MauiLocationTracker/issues) or submit a PR!

---

## 📃 License

MIT ©