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
using DisApp24.ViewModels;



namespace DisApp24
{
    public static class MauiProgram
    {

        public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<HttpClient>();

            mauiAppBuilder.Services.AddSingleton<IFirebaseAuthService, DisApp24.Services.FirebaseAuthService>();
            mauiAppBuilder.Services.AddSingleton<IFirebaseReservationService, DisApp24.Services.FirebaseReservationService>();
            mauiAppBuilder.Services.AddSingleton<IRssService, DisApp24.Services.RssService>();
            mauiAppBuilder.Services.AddSingleton<INavigationService, DisApp24.Services.NavigationService>();



            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {

            mauiAppBuilder.Services.AddSingleton<AppShell>();
            mauiAppBuilder.Services.AddSingleton<App>();
            mauiAppBuilder.Services.AddSingleton<RssPage>();
            mauiAppBuilder.Services.AddSingleton<ReservationPage>();
            mauiAppBuilder.Services.AddTransient<LoginPage>();
            mauiAppBuilder.Services.AddTransient<RegistrationPage>();
            mauiAppBuilder.Services.AddTransient<CalendarPage>();

            mauiAppBuilder.Services.AddSingleton<ViewModels.RssViewModel>();
            mauiAppBuilder.Services.AddSingleton<ViewModels.LoginPageViewModel>();
            mauiAppBuilder.Services.AddSingleton<ViewModels.RegistrationViewModel>();
            mauiAppBuilder.Services.AddSingleton<ViewModels.ReservationViewModel>();

            
            mauiAppBuilder.Services.AddTransient<RssItemDetailsPage>();
            mauiAppBuilder.Services.AddTransient<ViewModels.CalendarPageViewModel>();


            return mauiAppBuilder;
        }

        public static MauiApp CreateMauiApp()
        {


            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .RegisterServices()
                .RegisterViewModels()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                ;

           
            var appSettingsJson = EmbeddedResourceHelper.GetResourceText(typeof(MauiProgram).Assembly, "DisApp24.appsettings.json");
            var appSettings = JObject.Parse(appSettingsJson);

            var config = new AppConfig
            {
                RssUrl = appSettings["RssUrl"].ToString(),
                // Weitere Parameter können hier zugewiesen werden
            };

        // Register the config as a singleton
            builder.Services.AddSingleton<AppConfig>(config);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            return app;
        }
    }
}
