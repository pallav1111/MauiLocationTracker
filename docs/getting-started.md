# 📦 Getting Started with MauiLocationTracker

This guide will help you quickly integrate **MauiLocationTracker** into your .NET MAUI project and start tracking location data on Android and iOS.

---

## ✅ Prerequisites

* [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui) project (iOS/Android targets)
* NuGet package: `MauiLocationTracker`

Install using:

```bash
dotnet add package MauiLocationTracker
```

Or via Visual Studio NuGet Manager.

---

## 🔧 Add to Your Project

### Step 1: Register Services

In your `MauiProgram.cs`:

```csharp
using LocationTracking;

builder.Services.AddLocationTracking();
```

### Step 2: Request Permissions

Use MAUI Essentials `Permissions.LocationAlways` for both platforms:

```csharp
var status = await Permissions.RequestAsync<Permissions.LocationAlways>();
```

### Step 3: Start Tracking

Inject and use `ILocationTracker`:

```csharp
public partial class MainPage : ContentPage
{
    private readonly ILocationTracker _tracker;

    public MainPage(ILocationTracker tracker)
    {
        InitializeComponent();
        _tracker = tracker;
    }

    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        await _tracker.StartTrackingAsync();
    }
}
```

To stop tracking:

```csharp
await _tracker.StopTrackingAsync();
```

---

## 📡 Listen for Updates

```csharp
LocationEventHub.OnLocationReceived += location =>
{
    Console.WriteLine($"Lat: {location.Latitude}, Lng: {location.Longitude}");
};
```

Unsubscribe when needed:

```csharp
LocationEventHub.OnLocationReceived -= yourHandler;
```

---

You’re now ready to track and observe live location updates!
