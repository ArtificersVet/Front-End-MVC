using System.ComponentModel.DataAnnotations;

namespace MVC_NPANTS.Models
{
    public class EstiloTallaCreateDTO
    {
        [Required(ErrorMessage = "La talla es requerida")]
        public int TallaId { get; set; }

        [Required(ErrorMessage = "El consumo de tela es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El consumo de tela debe ser mayor que 0")]
        public decimal ConsumoTela { get; set; }
    }
}
