using DisApp24.Services;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;

namespace DisApp24
{
    public partial class RssPage : ContentPage
    {
        public ObservableCollection<SyndicationItem> RssItems { get; set; }

        public RssPage()
        {
            InitializeComponent();
            RssItems = new ObservableCollection<SyndicationItem>();
            BindingContext = this;
            LoadRssFeed();
        }

        private async void LoadRssFeed()
        {
            string rssUrl = "https://techcrunch.com/feed/?guccounter=1&guce_referrer=aHR0cHM6Ly9ibG9nLmZlZWRzcG90LmNvbS8&guce_referrer_sig=AQAAAH8N3NIpb7YGXmrHrJfQCt2d-4s09Mh2JZ00FsgH6YVwy934jyPmogQmnp5Ifws_xnTN5CNnn9vN7CeVKGhLL1rdkot2VYHdTH6WVzgVurniasz7O_6ZSyBC7QtjoAIyj4FZkUd4hDSeHwwZQXD73tvpSfykbVbQ5d1b3H5fDZ63";
            var rssItems = await RssService.GetRssFeedAsync(rssUrl);

            // Fügt die RSS-Elemente zur ObservableCollection hinzu
            foreach (var item in rssItems)
            {
                RssItems.Add(item);
            }
        }
    }
}
