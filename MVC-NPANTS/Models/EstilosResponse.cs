namespace MVC_NPANTS.Models
{
    public class EstilosResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<Estilo>? Estilos { get; set; } // Cambia "estilos" a "Estilos" para seguir las convenciones de C#
    }
}
