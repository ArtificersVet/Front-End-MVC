using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class Talla
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }

        public List<EstiloTalla> Estilos { get; set; } = new List<EstiloTalla>();
    }
}
