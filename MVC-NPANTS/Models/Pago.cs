using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public partial class Pago
    {
        public long Id { get; set; }

        [JsonPropertyName("fecha_pago")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaPago { get; set; }

        public decimal? Monto { get; set; }

        [JsonPropertyName("metodo_pago_id")]
        [Display (Name = "metodo de pago")]
        public long? MetodoPagoId { get; set; }

        [JsonPropertyName("pedido_id")]
        [Display (Name = "pedido")]
        public long? PedidoId { get; set; }

        [JsonPropertyName("metodo_de_pago")]
        public virtual MetodoPago? MetodoPago { get; set; }

        [JsonPropertyName("pedido")]
        public virtual Pedido? Pedido { get; set; }
    }
}
