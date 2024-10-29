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

    public class PagedTipoClientesResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<TipoCliente> TipoClientes { get; set; } = new List<TipoCliente>();
    }
}
