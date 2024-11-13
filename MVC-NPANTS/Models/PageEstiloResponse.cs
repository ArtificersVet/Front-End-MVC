namespace MVC_NPANTS.Models
{
    public class PageEstiloResponse
    {
        public List<Estilo> Estilos { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
