# 🛠 Platform Configuration (Android & iOS)

This guide provides required platform-specific setup steps to enable background location tracking on **Android** and **iOS** using `MauiLocationTracker`.

---

## 🤖 Android Setup

### 1. Add Permissions to `AndroidManifest.xml`

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
```

> **Note:** Android 10+ requires `ACCESS_BACKGROUND_LOCATION` explicitly.

### 2. Android Versions

* Android 12+ requires runtime permission for background location.
* Always request location via MAUI Essentials `Permissions.LocationAlways`.

---

## 🍏 iOS Setup

### 1. Add Permissions to `Info.plist`

```xml
<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>This app requires location access in the background.</string>
<key>NSLocationWhenInUseUsageDescription</key>
<string>Location is used to track your position.</string>
<key>UIBackgroundModes</key>
<array>
    <string>location</string>
</array>
```

### 2. Additional Notes

* iOS requires user to accept background location prompt.
* Ensure `AllowsBackgroundLocationUpdates = true` is enabled in the tracker implementation (already handled internally).

---

Once configured, background tracking will continue even when the app is minimized (Android) or the screen is locked (iOS).
