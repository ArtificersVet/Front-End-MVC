using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public class PageTelaResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        [JsonPropertyName("telas")]
        public List<Tela> Telas { get; set; } = new List<Tela>();
    }
}

