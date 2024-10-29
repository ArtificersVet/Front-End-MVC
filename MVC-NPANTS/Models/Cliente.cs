using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public partial class Cliente
    {
        public Cliente()
        {
            Pedidos = new HashSet<Pedido>();
        }

        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }
        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("telefono")]
        public string? Telefono { get; set; }

        [JsonPropertyName("tipocliente_id")]
        public long? TipoclienteId { get; set; }
        [JsonPropertyName("tipo_cliente")]

        public virtual TipoCliente? Tipocliente { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
