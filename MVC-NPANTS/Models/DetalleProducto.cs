using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public partial  class DetalleProducto
    {
        public long Id { get; set; }
        public long PedidoId { get; set; }
        public long PrendaVestirId { get; set; }
        //[JsonPropertyName("talla_id")]
        public long TallaId { get; set; }
        public int Cantidad { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal TotalPieza { get; set; }
        public decimal? ConsumoTela { get; set; }
        public decimal? SubTotal { get; set; }

        // Propiedades de navegación
        public virtual Pedido? Pedido { get; set; }
        public virtual PrendaVestir? PrendaVestir { get; set; }
        public virtual Talla? Talla { get; set; }
    }
}
