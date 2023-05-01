using DisApp24.Services;
using DisApp24.Models;

namespace DisApp24
{
    public partial class AppShell : Shell
    {
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IRssService _rssService;
        private readonly AppConfig _config;

        public AppShell(IFirebaseAuthService firebaseAuthService, IRssService rssService, AppConfig config)
        {
            InitializeComponent();

            _firebaseAuthService = firebaseAuthService;
            _rssService = rssService;
            _config = config;

            Items.Add(CreateRssTab());
            Items.Add(CreateReservationTab());
        }

        private Tab CreateRssTab()
        {
            var rssPage = new RssPage(_firebaseAuthService, _rssService, _config);

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
            var reservationPage = new ReservationPage(_firebaseAuthService);

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
