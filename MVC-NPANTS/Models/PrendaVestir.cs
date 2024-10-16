using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public partial class PrendaVestir
    {
        public long Id { get; set; }
        public string? Descripcion { get; set; }
        public string? Nombre { get; set; }

        [JsonPropertyName("estilo_id")]
        public long? EstiloId { get; set; }
        [JsonPropertyName("tela_id")]
        public long? TelaId { get; set; }
        [JsonPropertyName("tipoprendavestir_id")]
        public long? TipoprendavestirId { get; set; }

        public virtual Estilo? Estilo { get; set; }
        public virtual Tela? Tela { get; set; }
        public virtual TipoPrendaVestir? Tipoprendavestir { get; set; }

    }
}
