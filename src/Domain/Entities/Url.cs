namespace CNJCrawler.Domain.Entities
{
    public class Url
    {
        public string Value { get; set; }
        public Url(string value)
        {
            Value = value;
        }
    }
}
