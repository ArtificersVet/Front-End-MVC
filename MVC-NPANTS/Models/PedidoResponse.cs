using MVC_NPANTS.Models;

public class PedidoResponse
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public List<Pedido> Pedidos { get; set; }
}