using CNJCrawler.Domain.Entities;
using CNJCrawler.Domain.Interfaces;
using CNJCrawler.Domain.PageObjects;

namespace CNJCrawler.Domain.Services
{
    public class BuscaAtivaService
    {
        private string _domain = "";
        public BuscaAtivaService(string domain)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException(nameof(domain));

            _domain = domain;
        }

        public void Iniciar(string uf, string sistema, string[] termos)
        {

            switch ($"{sistema}")
            {
                case "ESAJ": 
                    EsajSearch esajSearch = new EsajSearch(new WebCrawlerService(), _domain);
                    esajSearch.Termos = termos;
                    esajSearch.Uf = uf;
                    esajSearch.Buscar();
                    break;
                default:
                    break;
            }
        }
    }
}
