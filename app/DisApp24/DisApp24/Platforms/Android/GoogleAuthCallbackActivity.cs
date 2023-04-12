using Android.App;
using Android.Content.PM;
using Microsoft.Maui;


namespace DisApp24
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
        Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
        DataScheme = "com.companyname.disapp24")]
    public class GoogleAuthCallbackActivity : WebAuthenticatorCallbackActivity
    {
    }
}
