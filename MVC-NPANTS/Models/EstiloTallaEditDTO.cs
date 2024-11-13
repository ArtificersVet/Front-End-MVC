using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public class EstiloTallaEditDTO
    {

        public int Id { get; set; } // ID de la talla en la tabla EstiloTalla

        [Required(ErrorMessage = "La talla es requerida")]
        [JsonPropertyName("talla_id")]

        public int TallaId { get; set; } // ID de la talla en la tabla Talla

        [Required(ErrorMessage = "El consumo de tela es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El consumo de tela debe ser mayor que 0")]
        public decimal ConsumoTela { get; set; }


        public virtual Talla? Talla { get; set; }


    }
}
