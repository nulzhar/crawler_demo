using CNJCrawler.Domain.Entities;

namespace CNJCrawler.Domain.Interfaces
{
    public interface IWebCrawlerService
    {
        WebPage CrawlPage(Url url);
    }
}