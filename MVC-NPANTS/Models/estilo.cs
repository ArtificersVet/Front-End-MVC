﻿using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class Estilo
    {
        public Estilo()
        {
            PrendaVestirs = new HashSet<PrendaVestir>();
        }

        public long Id { get; set; }
        public string? Nombre { get; set; }
        public string? Tipo { get; set; }

        public virtual ICollection<PrendaVestir> PrendaVestirs { get; set; }
    }
}
