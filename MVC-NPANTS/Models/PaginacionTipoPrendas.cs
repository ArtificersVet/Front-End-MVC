using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public class PaginacionTipoPrendas
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        [JsonPropertyName("tipoprendasvestir")]
        public List<TipoPrendaVestir> TipoPrendas { get; set; } = new List<TipoPrendaVestir>();
    }
}
