using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class HistorialPedido
    {
        public long Id { get; set; }
        public string? Comentario { get; set; }
        public DateTime? Fecha { get; set; }
        public long? EstadoPedidoId { get; set; }
        public long? PedidoId { get; set; }
        public int? UsuarioId { get; set; }

        public virtual EstadoPedido? EstadoPedido { get; set; }
        public virtual Pedido? Pedido { get; set; }
        public virtual Usuario? Usuario { get; set; }
    }
}
