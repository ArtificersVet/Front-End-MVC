using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class Cliente
    {
        public Cliente()
        {
            Pedidos = new HashSet<Pedido>();
        }

        public long Id { get; set; }
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public long? TipoclienteId { get; set; }

        public virtual TipoCliente? Tipocliente { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
