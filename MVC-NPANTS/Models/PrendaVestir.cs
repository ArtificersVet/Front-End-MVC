using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class PrendaVestir
    {
        public long Id { get; set; }
        public string? Descripcion { get; set; }
        public string? Nombre { get; set; }
        public long? EstiloId { get; set; }
        public long? TelaId { get; set; }
        public long? TipoprendavestirId { get; set; }

        public virtual Estilo? Estilo { get; set; }
        public virtual Tela? Tela { get; set; }
        public virtual TipoPrendaVestir? Tipoprendavestir { get; set; }
    }
}
