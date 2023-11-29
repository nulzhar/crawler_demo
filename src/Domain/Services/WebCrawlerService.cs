using System.IO.Compression;
using System.Text;
using CNJCrawler.Domain.Entities;
using CNJCrawler.Domain.Interfaces;
using CNJCrawler.Domain.PageObjects;

namespace CNJCrawler.Domain.Services
{
    public class WebCrawlerService : IWebCrawlerService
    {
        public WebPage CrawlPage(Url url)
        {
            // Implementation of the crawling logic.
            // Fetch the web page content, parse links, etc.
            // This method might return a new WebPage entity representing the crawled content.
            
            // Simulate the crawling logic by fetching content.
            string pageContent = FetchPageContent(url.Value);

            // Use the Page Object to represent the crawled page.
            return new CrawledPage(url, pageContent);
        }

        // Simulate fetching content from a web page.
        private string FetchPageContent(string url)
        {
            // In a real implementation, you would make HTTP requests or use a library to fetch the page content.
            // This is a simplified example.
            // return $"Content of the page at {url}";
            using (HttpClient client = new HttpClient())
            {
                // Specify the URL
                // string url = "https://consultasaj.tjam.jus.br/cpopg/trocarPagina.do?paginaConsulta=5&paginaConsulta=4&paginaConsulta=5&conversationId=&cbPesquisa=NMPARTE&dadosConsulta.valorConsulta=ITAU+UNIBANCO+HOLDING&cdForo=-1";

                // Create the request headers
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:109.0) Gecko/20100101 Firefox/114.0");
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                // client.DefaultRequestHeaders.Add("Referer", "https://consultasaj.tjam.jus.br/cpopg/trocarPagina.do?paginaConsulta=4&paginaConsulta=5&conversationId=&cbPesquisa=NMPARTE&dadosConsulta.valorConsulta=ITAU+UNIBANCO+HOLDING&cdForo=-1");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                // client.DefaultRequestHeaders.Add("Cookie", "JSESSIONID=64DF7407D77E7FA23656DBCC312218EC.cpopg1; _ga_56ZBRJST4W=GS1.1.1700009056.1.1.1700009134.0.0.0; _ga=GA1.1.930176307.1700009056");
                client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");

                // Make the GET request
                HttpResponseMessage response = client.GetAsync(url).Result;

                // Check if the request was successful (status code 200)
                if (response.IsSuccessStatusCode)
                {
                    // Check the content type
                    if (response.Content.Headers.ContentType?.MediaType == "text/html")
                    {
                        // Decompress the GZIP-encoded content
                        Stream decompressedStream = response.Content.ReadAsStreamAsync().Result;
                        using (GZipStream gzipStream = new GZipStream(decompressedStream, CompressionMode.Decompress))
                        using (StreamReader reader = new StreamReader(gzipStream, Encoding.UTF8))
                        {
                            // Read the response content
                            string content = reader.ReadToEnd();

                            // Save the content to a file with UTF-8 encoding
                            SaveContentToFile(content, "output2.html");
                            
                            Console.WriteLine("Content saved successfully.");
                            return content;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unexpected content type: {response.Content.Headers.ContentType?.MediaType}");
                        return "";
                    }
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    return "";
                }
            }
        }

        private void SaveContentToFile(string content, string filePath)
        {
            // Use StreamWriter with UTF-8 encoding to write the content to a file
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.Write(content);
            }
        }
    }

    /*
    With this structure, the CrawledPage class encapsulates the structure and behavior of the crawled page.
    The WebCrawlerService uses this Page Object to represent the result of crawling a page.
    In a more sophisticated scenario, you might have methods in the CrawledPage class to extract information from the page content, navigate links, or perform other interactions.
    The key is to encapsulate the behavior related to the page within the CrawledPage class, promoting a clean and maintainable design.
    */
}
