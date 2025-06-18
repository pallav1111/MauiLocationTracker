using Android.App;
using Android.Content;
using Android.OS;
using LocationTracking.Services;

namespace LocationTracking.Receivers;

[BroadcastReceiver(Enabled = true, Exported = true, Permission = "android.permission.RECEIVE_BOOT_COMPLETED", DirectBootAware = true)]
[IntentFilter([Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted])]
public class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        // ReSharper disable once RedundantJumpStatement
        if (intent?.Action != Intent.ActionBootCompleted) return;

        if (context is null) return;
        
        var serviceIntent = new Intent(context, typeof(AndroidLocationService));
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            context.StartForegroundService(serviceIntent);
        }
        else
        {
            context.StartService(serviceIntent);
        }
    }
}