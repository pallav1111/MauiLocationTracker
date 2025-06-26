# 🚀 Advanced Usage with MauiLocationTracker

Once you’ve integrated the basic tracking functionality, you can explore more advanced capabilities of **MauiLocationTracker** to customize behavior and control.

---

## 🛠 Configure Tracking Behavior

Use `LocationTrackingOptions` to fine-tune how tracking behaves:

```csharp
builder.Services.AddLocationTracking(options =>
{
    options.Interval = TimeSpan.FromSeconds(30); // Set custom update interval
    options.Accuracy = LocationAccuracy.High;     // Choose accuracy level
    options.EnableBackgroundTracking = true;      // Allow background updates
});
```

---

## 📂 Access Logged Location Data

Access stored logs via `ILocationLogger`:

```csharp
var logger = serviceProvider.GetRequiredService<ILocationLogger>();

var logs = await logger.GetAllLocationTraceAsync();
foreach (var entry in logs)
{
    Console.WriteLine($"{entry.Timestamp}: {entry.Latitude}, {entry.Longitude}");
}
```

You can also export logs:

```csharp
var exportPath = await logger.ExportLogsAsync();
```

And clear them:

```csharp
await logger.ClearLogsAsync();
```

---

## 📡 Subscribe to Live Events

Use the static `LocationEventHub` to listen to real-time updates:

```csharp
LocationEventHub.OnLocationReceived += location =>
{
    Console.WriteLine($"Location: {location.Latitude}, {location.Longitude}");
};
```

---

## 🧪 Test in Background Mode

To test background location updates:

* On Android, swipe away the app or lock the screen. Ensure foreground service is declared in `AndroidManifest.xml`.
* On iOS, use a real device. Set `AllowsBackgroundLocationUpdates = true` in the platform service and enable `Location updates` in Xcode background modes.

---

✅ You now have full control of how location tracking behaves.