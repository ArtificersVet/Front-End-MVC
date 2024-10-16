﻿using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public partial class EstiloTalla
    {
        public long Id { get; set; }
        public double consumoTela { get; set; }
        public long EstiloId { get; set; }
        public long TallaId { get; set; }

        [JsonPropertyName("Talla")]
        public virtual Talla? Talla { get; set; }
    }
}
