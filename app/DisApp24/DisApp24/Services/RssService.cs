using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DisApp24.Services
{
    public class RssService : IRssService
    {
        public async Task<List<RssItem>> GetRssFeedAsync(string rssUrl)
        {
            using var reader = XmlReader.Create(rssUrl);
            var feed = await Task.FromResult(SyndicationFeed.Load(reader));
            return feed.Items.Select(item => new RssItem
            {
                Title = item.Title.Text,
                Summary = StripHtmlTags(item.Summary.Text),
                PublishDate = item.PublishDate.DateTime
            }).ToList();
        }

        private string StripHtmlTags(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }


    }
}
