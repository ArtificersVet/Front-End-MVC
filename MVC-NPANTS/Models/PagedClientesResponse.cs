namespace MVC_NPANTS.Models
{
    public class PagedClientesResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<Cliente> Clientes { get; set; } = new List<Cliente>(); 
    }
    
}