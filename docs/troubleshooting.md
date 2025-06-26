# 🛠️ Troubleshooting MauiLocationTracker

This section provides solutions to common problems and edge cases encountered while using **MauiLocationTracker**.

---

## 📌 No Location Updates

### ✅ Ensure Permissions Are Granted

* Use `Permissions.LocationAlways` and check status before tracking.
* iOS: Verify that `NSLocationAlwaysAndWhenInUseUsageDescription` is set in `Info.plist`.
* Android: Check runtime permissions for:

    * `ACCESS_FINE_LOCATION`
    * `ACCESS_COARSE_LOCATION`
    * `ACCESS_BACKGROUND_LOCATION`

### ✅ Enable Background Modes

* iOS: In **Info.plist**, add:

```xml
<key>UIBackgroundModes</key>
<array>
  <string>location</string>
</array>
```

* Android: Ensure foreground service is started correctly.

---

## 📌 App Killed on Android

If the app is force closed using "swipe + clear all":

* Android system stops all associated services.
* You can explore `JobScheduler` or `AlarmManager` for more advanced use cases.

> Note: Some restrictions apply on Android 12+ for background work.

---

## 📌 Tracking Stops on iOS After Some Time

* iOS may throttle updates to save battery.
* Ensure `AllowsBackgroundLocationUpdates` is set to `true` in the CLLocationManager.
* Do not pause location updates automatically.

---

## 📌 No Logs Available

* Internal logger only stores data if `LogInternally` is enabled in `LocationTrackingOptions`.
* Check app's storage permissions (especially on Android 11+ scoped storage).
* Use `.ExportLogsAsync()` to verify logs were written.

---

## 📌 Cannot Build iOS Project

Check that these entries exist in `Info.plist`:

```xml
<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>We use your location to track your route.</string>

<key>NSLocationWhenInUseUsageDescription</key>
<string>Location is used for tracking when app is active.</string>
```

---

Still stuck? [Open an Issue](https://github.com/pallav1111/MauiLocationTracker/issues) on GitHub for help.
