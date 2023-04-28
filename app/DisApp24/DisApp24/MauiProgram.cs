using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Logging;
using DisApp24.Services;
using Syncfusion.Maui.Core.Hosting;
using DisApp24.Helpers;
using Newtonsoft.Json.Linq;
using DisApp24.Models;

namespace DisApp24
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            
            var appSettingsJson = EmbeddedResourceHelper.GetResourceText(typeof(MauiProgram).Assembly, "DisApp24.appsettings.json");
            var appSettings = JObject.Parse(appSettingsJson);

            var config = new AppConfig
            {
                RssUrl = appSettings["RssUrl"].ToString(),
                // Weitere Parameter können hier zugewiesen werden
            };

            // Register the config as a singleton
            builder.Services.AddSingleton(config);


            // Register platform-specific services
            builder.Services.AddSingleton<IFirebaseAuthService, DisApp24.Services.FirebaseAuthService>();
            builder.Services.AddSingleton<IRssService, DisApp24.Services.RssService>();

            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<App>();
            builder.Services.AddSingleton<RssPage>();
            builder.Services.AddSingleton<ReservationPage>();




#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
