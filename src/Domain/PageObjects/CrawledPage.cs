using CNJCrawler.Domain.Entities;

namespace CNJCrawler.Domain.PageObjects
{
    public class CrawledPage : WebPage
    {
        // public string Content { get; private set; }
        // Add properties for other page elements if needed.

        public CrawledPage(Url url, string content) : base(url, content)
        {
            // base.Content = content;
            // Initialize other page elements if needed.
        }
    }
}