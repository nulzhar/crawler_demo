using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CNJCrawler.Domain.Services;

class Program
{
    static async Task Main()
    {
        string domain = "https://esaj.tjsp.jus.br";
        string[] termos = new string[] { "ITAU+UNIBANCO+HOLDING"};
        BuscaAtivaService buscaAtivaService = new BuscaAtivaService(domain);
        buscaAtivaService.Iniciar(uf: "AM", sistema: "ESAJ", termos: termos);
        // await MakeRequest();
    }

    static async Task MakeRequest()
    {
        using (HttpClient client = new HttpClient())
        {
            // Specify the URL
            string url = "https://consultasaj.tjam.jus.br/cpopg/trocarPagina.do?paginaConsulta=5&paginaConsulta=4&paginaConsulta=5&conversationId=&cbPesquisa=NMPARTE&dadosConsulta.valorConsulta=ITAU+UNIBANCO+HOLDING&cdForo=-1";

            // Create the request headers
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:109.0) Gecko/20100101 Firefox/114.0");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Referer", "https://consultasaj.tjam.jus.br/cpopg/trocarPagina.do?paginaConsulta=4&paginaConsulta=5&conversationId=&cbPesquisa=NMPARTE&dadosConsulta.valorConsulta=ITAU+UNIBANCO+HOLDING&cdForo=-1");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Cookie", "JSESSIONID=64DF7407D77E7FA23656DBCC312218EC.cpopg1; _ga_56ZBRJST4W=GS1.1.1700009056.1.1.1700009134.0.0.0; _ga=GA1.1.930176307.1700009056");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");

            // Make the GET request
            HttpResponseMessage response = await client.GetAsync(url);

            // Check if the request was successful (status code 200)
            if (response.IsSuccessStatusCode)
            {
                // Check the content type
                if (response.Content.Headers.ContentType?.MediaType == "text/html")
                {
                    // Decompress the GZIP-encoded content
                    Stream decompressedStream = await response.Content.ReadAsStreamAsync();
                    using (GZipStream gzipStream = new GZipStream(decompressedStream, CompressionMode.Decompress))
                    using (StreamReader reader = new StreamReader(gzipStream, Encoding.UTF8))
                    {
                        // Read the response content
                        string content = reader.ReadToEnd();

                        // Save the content to a file with UTF-8 encoding
                        SaveContentToFile(content, "output2.html");

                        Console.WriteLine("Content saved successfully.");
                    }
                }
                else
                {
                    Console.WriteLine($"Unexpected content type: {response.Content.Headers.ContentType?.MediaType}");
                }
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            }
        }
    }

    static void SaveContentToFile(string content, string filePath)
    {
        // Use StreamWriter with UTF-8 encoding to write the content to a file
        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            writer.Write(content);
        }
    }
}
