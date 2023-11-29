using CNJCrawler.Domain.Entities;

namespace CNJCrawler.Domain.Aggregates
{
    public class WebCrawlingAggregate
    {
        public WebPage CurrentPage  { get; set; }
        public WebCrawlingAggregate(WebPage initialPage)
        {
            CurrentPage = initialPage ?? throw new ArgumentNullException(nameof(initialPage));
        }

        // Additional business logic for the crawling process can be added here.
    }
}
