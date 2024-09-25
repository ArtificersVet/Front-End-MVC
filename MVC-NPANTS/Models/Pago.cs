using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class Pago
    {
        public long Id { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal? Monto { get; set; }
        public long? MetodoPagoId { get; set; }
        public long? PedidoId { get; set; }

        public virtual MetodoPago? MetodoPago { get; set; }
        public virtual Pedido? Pedido { get; set; }
    }
}
