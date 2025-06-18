using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;

namespace LocationTracking.Services;

[Service(Name = "com.locationtracking.RestartJobService", Permission = "android.permission.BIND_JOB_SERVICE", Exported = true)]
public class RestartJobService : JobService
{
    public override bool OnStartJob(JobParameters? @params)
    {
        if (ApplicationContext != null)
        {
            var intent = new Intent(ApplicationContext, typeof(AndroidLocationService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                ApplicationContext.StartForegroundService(intent);
            }
            else
            {
                ApplicationContext.StartService(intent);
            }
        }

        // Job is finished immediately
        JobFinished(@params, false);
        return true;
    }

    public override bool OnStopJob(JobParameters? @params) => false;
}