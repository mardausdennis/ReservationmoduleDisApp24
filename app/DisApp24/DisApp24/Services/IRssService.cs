using System.Collections.Generic;
using System.Threading.Tasks;

namespace DisApp24.Services
{
    public interface IRssService
    {
        Task<List<RssItem>> GetRssFeedAsync(string rssUrl);
        string StripHtmlTags(string source);

        string StripHtmlContent(string source);
    }
}
