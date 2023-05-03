using DisApp24.Services;
using DisApp24.Models;

namespace DisApp24
{
    public partial class AppShell : Shell
    {
        

        public AppShell()
        {
            InitializeComponent();

            

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(CalendarPage), typeof(CalendarPage));

            Items.Add(CreateRssTab());
            Items.Add(CreateReservationTab());
        }

        private Tab CreateRssTab()
        {
            var rssPage = ServiceHelper.GetService<RssPage>();

            var shellContent = new ShellContent
            {
                Route = "rssPage",
                Content = rssPage
            };

            return new Tab
            {
                Title = "RSS",
                Icon = "AppIcon/rss_icon.png",
                Items = { shellContent }
            };
        }

        private Tab CreateReservationTab()
        {
            var reservationPage = ServiceHelper.GetService<ReservationPage>();

            var shellContent = new ShellContent
            {
                Route = "reservationpage",
                Content = reservationPage
            };

            return new Tab
            {
                Title = "Reservation",
                Icon = "AppIcon/calendar_icon.png",
                Items = { shellContent }
            };
        }
    }
}
