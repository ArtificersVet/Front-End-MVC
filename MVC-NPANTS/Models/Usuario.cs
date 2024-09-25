using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            HistorialPedidos = new HashSet<HistorialPedido>();
        }

        public int Id { get; set; }
        public int? RolId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public virtual Role? Rol { get; set; }
        public virtual ICollection<HistorialPedido> HistorialPedidos { get; set; }
    }
}
