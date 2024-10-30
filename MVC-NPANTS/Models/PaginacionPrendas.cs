using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public class PaginacionPrendas
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<PrendaVestir> Prendas { get; set; } // Lista de prendas
    }
}
