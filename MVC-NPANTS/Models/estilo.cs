using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class Estilo
    {
        public Estilo()
        {
            PrendaVestirs = new HashSet<PrendaVestir>();
            EstiloTallas = new List<EstiloTalla>(); // Relación con EstiloTalla
        }

        public long Id { get; set; }
        public string? Nombre { get; set; }
        public string? Tipo { get; set; }

        public virtual ICollection<PrendaVestir> PrendaVestirs { get; set; }
        public virtual ICollection<EstiloTalla> EstiloTallas { get; set; } // Asociación a EstiloTalla
    }
}
