using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class TipoPrendaVestir
    {
        public TipoPrendaVestir()
        {
            PrendaVestirs = new HashSet<PrendaVestir>();
        }

        public long Id { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<PrendaVestir> PrendaVestirs { get; set; }
        public IEnumerable<TipoPrendaVestir>? TiposPrenda { get; internal set; }
    }
}
