using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DisApp24
{
    public class RssItemDetailsPage : ContentPage
    {
        public RssItemDetailsPage(RssItem item)
        {
            Title = item.Title;
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Children =
                {
                    new Label
                    {
                        Text = item.Title,
                        FontAttributes = FontAttributes.Bold,
                        Margin = new Thickness(0,10)
                    },
                    new Label
                    {
                        Text = item.PublishDate.ToString("dd.MM.yyyy HH:mm"),
                        Margin = new Thickness(0,0,0,10)
                    },
                    new Label
                    {
                        Text = WebUtility.HtmlDecode(item.Summary)
                    }
                }
                }
            };
        }
    }

}
