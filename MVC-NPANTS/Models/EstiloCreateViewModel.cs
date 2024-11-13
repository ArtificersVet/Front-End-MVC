using System.ComponentModel.DataAnnotations;

namespace MVC_NPANTS.Models
{
    public class EstiloCreateViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El tipo es requerido")]
        public string Tipo { get; set; }

        public List<EstiloTallaCreateDTO> Tallas { get; set; } = new List<EstiloTallaCreateDTO>();
    }
}
