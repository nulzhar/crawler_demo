namespace CNJCrawler.Domain.Entities
{
    public class WebPage
    {
        public Url Url { get; set;}
        public string Content { get; set;}
        public WebPage(Url url, string content)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Content = content;
        }
    }
}

