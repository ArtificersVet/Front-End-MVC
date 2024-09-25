using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class TipoCliente
    {
        public TipoCliente()
        {
            Clientes = new HashSet<Cliente>();
        }

        public long Id { get; set; }
        public string? Descripcion { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<Cliente> Clientes { get; set; }
    }
}
