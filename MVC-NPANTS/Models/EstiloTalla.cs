﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public class EstiloTalla
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "La talla es requerida")]
        public int TallaId { get; set; }

        [Required(ErrorMessage = "El consumo de tela es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El consumo de tela debe ser mayor que 0")]
        public decimal ConsumoTela { get; set; }

        public Talla? Talla { get; set; }

    }
}
