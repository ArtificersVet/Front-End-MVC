using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public class PageEstadoPedidoResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        [JsonPropertyName("estadospedido")]
        public List<EstadoPedido> EstadosPedido { get; set; } = new List<EstadoPedido>();
    }
}
