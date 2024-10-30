using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public class PageTipoClienteResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        [JsonPropertyName("tipoclientes")]
        public List<TipoCliente> TipoClientes { get; set; } = new List<TipoCliente>();
    }
}

