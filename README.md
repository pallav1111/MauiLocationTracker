
# 🌍 MauiLocationTracker

A **cross-platform .NET MAUI** library for continuous location tracking on Android and iOS. Supports **background tracking**, **customizable intervals & accuracy**, and a plug-in logging system.

---

## ⚙️ Features

* **Real-time location updates** using native providers on both Android and iOS.
* **Background tracking** on Android via:

  * Foreground Service
  * `JobScheduler` for resilience across app swipes (not system kills)
* **Background mode** on iOS with `CLLocationManager`.
* Fully configurable:

  * Update interval
  * Accuracy levels
  * Choose between foreground-only or persistent background tracking
* Pluggable logger (`ILocationLogger`) to handle location data (e.g., save to file, send to server).

---

## 📦 Installation

Install via NuGet or CLI:

```bash
dotnet add package MauiLocationTracker
```

---

## 🚀 Getting Started

### 1. Sample App Setup

**Clone the repo** and open the `SampleApp` project in Visual Studio or Rider.

Ensure all projects build cleanly and restore dependencies.

---

### 2. Android Setup

In `Platforms/Android/AndroidManifest.xml`, add:

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
```

### 3. iOS Setup

In `Platforms/iOS/Info.plist`, add:

```xml
<key>UIBackgroundModes</key>
<array>
  <string>location</string>
  <string>fetch</string>
</array>
<key>NSLocationWhenInUseUsageDescription</key>
<string>Used to track location.</string>
<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>Tracks location continuously even in background.</string>
```

---

### 4. Configure DI (`MauiProgram.cs`)

```csharp
builder.Services.AddSingleton(new LocationTrackingOptions
{
    Accuracy = LocationAccuracy.High,
    Interval = TimeSpan.FromSeconds(30),
    EnableBackgroundTracking = true // true for Android background
});
```

---

### 5. Use in App

```csharp
public partial class MainPage : ContentPage
{
    readonly ILocationTracker tracker;

    public MainPage(ILocationTracker tracker)
    {
        InitializeComponent();
        this.tracker = tracker;
    }

    async void Start_Clicked(object sender, EventArgs e) =>
        await tracker.StartTrackingAsync();

    async void Stop_Clicked(object sender, EventArgs e) =>
        await tracker.StopTrackingAsync();
}
```

---

## 🛠 Platform Behavior

| Platform    | Background Behavior                  | Survives App Swipe | Survives App Kill |
| ----------- | ------------------------------------ | ------------------ | ----------------- |
| **Android** | ✔️ Foreground service + JobScheduler | ✅ Yes              | ❌ No              |
| **iOS**     | ✔️ CLLocationManager background mode | ✅ Yes              | ❌ No              |

---

## 📝 Contribution

1. Fork this repo
2. Create new branch
3. Add feature or fix
4. Update README/tests
5. Submit PR

