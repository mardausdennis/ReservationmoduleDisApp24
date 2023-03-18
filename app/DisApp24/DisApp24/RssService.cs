using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace DisApp24.Services
{
    public static class RssService
    {
        public static async Task<List<SyndicationItem>> GetRssFeedAsync(string rssUrl)
        {
            using var reader = XmlReader.Create(rssUrl);
            var feed = await Task.Run(() => SyndicationFeed.Load(reader));
            return feed.Items.ToList();
        }
    }
}
