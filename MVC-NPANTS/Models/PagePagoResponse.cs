using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public class PagePagoResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        [JsonPropertyName("pagos")]
        public List<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
