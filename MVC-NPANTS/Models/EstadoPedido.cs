using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public partial class EstadoPedido
    {
        public EstadoPedido()
        {
            HistorialPedidos = new HashSet<HistorialPedido>();
            Pedidos = new HashSet<Pedido>();
        }

        public long Id { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<HistorialPedido> HistorialPedidos { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
    }

    public class EstadoPedidosResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        [JsonPropertyName("estadosPedido")]
        public List<EstadoPedido> estadoPedidos { get; set; }
    }
}
