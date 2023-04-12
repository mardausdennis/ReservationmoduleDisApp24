using DisApp24.Services;

namespace DisApp24
{
    public partial class AppShell : Shell
    {
        private readonly IFirebaseAuthService _firebaseAuthService;

        public AppShell(IFirebaseAuthService firebaseAuthService)
        {
            InitializeComponent();

            _firebaseAuthService = firebaseAuthService;

            Items.Add(CreateRssTab());
            Items.Add(CreateReservationTab());
        }

        private Tab CreateRssTab()
        {
            var rssPage = new RssPage(_firebaseAuthService);

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
