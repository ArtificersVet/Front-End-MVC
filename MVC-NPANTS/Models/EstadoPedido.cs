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

   
}
