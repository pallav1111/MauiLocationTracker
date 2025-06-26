# ⚙️ Configuration

Customize location tracking behavior using the `LocationTrackingOptions` class. This page explains the available options and how to apply them.

---

## 📘 LocationTrackingOptions

When registering the service in `MauiProgram.cs`, you can pass a configuration instance:

```csharp
builder.Services.AddLocationTracking(new LocationTrackingOptions
{
    Interval = TimeSpan.FromMinutes(2),
    Accuracy = LocationAccuracy.High,
    EnableBackgroundTracking = true,
    LogInternally = true
});
```

### 🔧 Available Options

| Property                   | Type               | Description                                                        |
| -------------------------- | ------------------ | ------------------------------------------------------------------ |
| `Interval`                 | `TimeSpan`         | How frequently to request location updates. Default: 5 minutes.    |
| `Accuracy`                 | `LocationAccuracy` | Controls the GPS accuracy level. Default: `Balanced`.              |
| `EnableBackgroundTracking` | `bool`             | Enables background service on Android / background updates on iOS. |
| `LogInternally`            | `bool`             | If true, logs are stored locally and can be exported.              |

---

## 🛠 Update Configuration at Runtime

If needed, you can update options dynamically:

```csharp
_locationTracker.UpdateConfiguration(new LocationTrackingOptions
{
    Interval = TimeSpan.FromSeconds(30),
    Accuracy = LocationAccuracy.Best
});
```

---