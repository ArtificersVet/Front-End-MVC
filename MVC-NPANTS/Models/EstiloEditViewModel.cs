using System.ComponentModel.DataAnnotations;

namespace MVC_NPANTS.Models
{

   

    public class EstiloEditViewModel
    {
        public int Id { get; set; } // ID del estilo

        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El tipo es requerido")]
        public string Tipo { get; set; }

        [UniqueTallaId]
        public List<EstiloTallaEditDTO> Tallas { get; set; } = new List<EstiloTallaEditDTO>();
    }
}

