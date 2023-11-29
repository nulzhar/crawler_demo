using CNJCrawler.Domain.Entities;

namespace CNJCrawler.Domain.Interfaces
{
    public interface IWebPageRepository
    {
        WebPage GetWebPageByUrl(Url url);
        void SaveWebPage(WebPage webPage);
    }
}