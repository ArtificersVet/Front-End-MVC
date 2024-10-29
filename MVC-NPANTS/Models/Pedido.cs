using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public partial class Pedido
    {
        public Pedido()
        {
            HistorialPedidos = new HashSet<HistorialPedido>();
            Pagos = new HashSet<Pago>();
        }

        public long Id { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal? Saldo { get; set; }
        public sbyte? TipoPago { get; set; }
        public decimal? Total { get; set; }
        public long? ClienteId { get; set; } 
        
        public long? EstadoPedidoId { get; set; }
        [JsonPropertyName("clientes")]
        public virtual Cliente? Cliente { get; set; }
        [JsonPropertyName("estado_pedido")]
        public virtual EstadoPedido? EstadoPedido { get; set; }
        public virtual ICollection<HistorialPedido> HistorialPedidos { get; set; }
        public List<DetalleProducto> Detalles { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
    }

}
