using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class Tela
    {
        public Tela()
        {
            PrendaVestirs = new HashSet<PrendaVestir>();
        }

        public long Id { get; set; }
        public string? Color { get; set; }
        public string? Nombre { get; set; }
        public double? Stock { get; set; }

        public virtual ICollection<PrendaVestir> PrendaVestirs { get; set; }
        public IEnumerable<Tela>? Telas { get; internal set; }
    }
}
