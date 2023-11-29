using System.Diagnostics;
using System.Text.RegularExpressions;
using CNJCrawler.Domain.Entities;
using CNJCrawler.Domain.Interfaces;
using CNJCrawler.Domain.Services;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace CNJCrawler.Domain.PageObjects
{
    public class EsajSearch
    {
        private IWebCrawlerService _webCrawlerService;
        private HtmlDocument _document;
        private string _domain;

        public string[] Termos { get; set; }
        public string Uf { get; set; }

        public EsajSearch(IWebCrawlerService webCrawlerService, string domain)
        {
            _webCrawlerService = webCrawlerService;
            _domain = domain;
        }

        public void Buscar()
        {
            int paginaConsulta = 1;
            // string termo = "ITAU+UNIBANCO+HOLDING";
            // TODO: VALIDAR SE TERMOS TEM ELEMENTOS ANTES DE INICIAR A BUSCA
            string termo = Termos[0];
            string url = $"{_domain}/cpopg/trocarPagina.do?paginaConsulta={paginaConsulta}&conversationId=&cbPesquisa=NMPARTE&dadosConsulta.valorConsulta={termo}&cdForo=-1";
            WebPage page = _webCrawlerService.CrawlPage(url: new Entities.Url(url));

            _document = new HtmlDocument();
            _document.LoadHtml(page.Content);

            int contador = ContadorDeProcessos;
            var lista = ProcessosLocalizados;

            // TODO: INTRODUZIR NAVEGACAO DE PAGINA
            SaveListToJsonFile(lista, $"lista_{termo}_{Uf}.txt");

            return;
        }

        public int ContadorDeProcessos
        {
            get
            {
                var texto = _document.GetElementbyId("contadorDeProcessos").InnerText;
                bool sucesso = int.TryParse(Regex.Replace(texto, @"[^\d]", ""), out int contador);
                return sucesso ? contador : -1;
            }
        }

        public IEnumerable<ProcessoBusca> ProcessosLocalizados
        {
            get
            {
                try
                {
                    List<ProcessoBusca> processos = new List<ProcessoBusca>();
                    IEnumerable<HtmlNode> nodes = _document.DocumentNode.Descendants(0).Where(n => n.HasClass("home__lista-de-processos"));
                    foreach (HtmlNode node in nodes)
                    {
                        var processoNode = node.Descendants(0).Where(n => n.HasClass("linkProcesso")).FirstOrDefault();
                        var linkProcesso = processoNode.Attributes[0].Value;
                        var numeroProcesso = processoNode.InnerText;
                        
                        Match match = Regex.Match(numeroProcesso, @"(\d{7}-\d{2}\.\d{4}\.\d{1,2}\.\d{2}\.\d{4})");
                        if (match.Success)
                        {
                            numeroProcesso = match.Groups[1].Value;
                            Console.WriteLine("Extracted pattern: " + numeroProcesso);
                        }
                        else
                        {
                            Console.WriteLine("Pattern not found in the input text.");
                        }

                        string identificadorExterno = "";
                        match = Regex.Match(linkProcesso, @"processo\.codigo=(\w+)&");
                        if (match.Success)
                        {
                            identificadorExterno = match.Groups[1].Value;
                            Console.WriteLine("Extracted pattern: " + identificadorExterno);
                        }
                        else
                        {
                            Console.WriteLine("Pattern not found in the input text.");
                        }

                        var tipoDeParticipacao = node.Descendants(0).Where(n => n.HasClass("tipoDeParticipacao")).FirstOrDefault()?.InnerText ?? "";
                        match = Regex.Match(tipoDeParticipacao, @"\b(Requerente|Requerido)\b");
                        if (match.Success)
                        {
                            tipoDeParticipacao = match.Groups[1].Value;
                            Console.WriteLine("Extracted pattern: " + tipoDeParticipacao);
                        }
                        else
                        {
                            Console.WriteLine("Pattern not found in the input text.");
                        }

                        var nomeParte = node.Descendants(0).Where(n => n.HasClass("nomeParte")).FirstOrDefault()?.InnerText ?? "";
                        nomeParte = Regex.Replace(nomeParte, @"[\n\t]", "").Trim().ToUpper();

                        var classe = node.Descendants(0).Where(n => n.HasClass("classeProcesso")).FirstOrDefault()?.InnerText ?? "";
                        classe = Regex.Replace(classe, @"[\n\t]", "").Trim().ToUpper();

                        var assunto = node.Descendants(0).Where(n => n.HasClass("assuntoPrincipalProcesso")).FirstOrDefault()?.InnerText ?? "";
                        assunto = Regex.Replace(assunto, @"[\n\t]", "").Trim().ToUpper();

                        var dataEDistribuicao = node.Descendants(0).Where(n => n.HasClass("dataLocalDistribuicaoProcesso")).FirstOrDefault()?.InnerText ?? "";
                        string data = "";
                        DateTime dataFormatada = new DateTime();
                        string localDistribuicao = "";
                        match = Regex.Match(dataEDistribuicao, @"\b\d{2}/\d{2}/\d{4}\b");
                        if (match.Success)
                        {
                            data = match.Value;
                            DateTime.TryParse(data, out dataFormatada);
                            Console.WriteLine("Extracted pattern: " + data);
                        }
                        else
                        {
                            Console.WriteLine("Pattern not found in the input text.");
                        }
                        match = Regex.Match(dataEDistribuicao, @"(?<=- ).*");
                        if (match.Success)
                        {
                            localDistribuicao = match.Groups[0].Value;
                            Console.WriteLine("Extracted pattern: " + localDistribuicao);
                        }
                        else
                        {
                            Console.WriteLine("Pattern not found in the input text.");
                        }

                        processos.Add(new ProcessoBusca{
                            IdentificadorExterno = identificadorExterno,
                            NumeroProcesso = numeroProcesso,
                            TipoParte = tipoDeParticipacao,
                            NomeParte = nomeParte,
                            ClasseProcesso = classe,
                            AssuntoProcesso = assunto,
                            DataRecebidaProcesso = dataFormatada,
                            LocalDistribuicao = localDistribuicao,
                        });
                    }
                    return processos;
                }
                catch (System.Exception)
                {
                    Console.WriteLine("Falha na captura dos dados");
                    return new List<ProcessoBusca>();
                }
            }
        }

        public void SaveListToJsonFile(IEnumerable<object> myList, string filePath)
        {
            // Serialize the list to JSON
            string json = JsonConvert.SerializeObject(myList, Formatting.Indented);

            // Write the JSON to a text file
            File.WriteAllText(filePath, json);
        }

    }

    public class ProcessoBusca
    {
        public string IdentificadorExterno { get; set; }
        public string NumeroProcesso { get; set; }
        public string TipoParte { get; set; }
        public string NomeParte { get; set; }
        public string ClasseProcesso { get; set; }
        public string AssuntoProcesso { get; set; }
        public DateTime DataRecebidaProcesso { get; set; }
        public string LocalDistribuicao { get; set; }
    }
}
