using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class Role
    {
        public Role()
        {
            usuarios = new HashSet<usuario>();
        }

        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<usuario> usuarios { get; set; }
    }
}
