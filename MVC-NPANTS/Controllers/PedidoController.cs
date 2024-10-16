using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;
using System.Text;
using System.Text.Json;

namespace MVC_NPANTS.Controllers
{
    public class PedidoController : Controller
    {
        private readonly HttpClient _httpClient;

        public PedidoController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CRMAPI");
        }

        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        // GET: PedidoController
        public async Task<ActionResult> Index()
        {
            SetAuthorizationHeader();

            // Obtener la lista de pedidos
            var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("pedidos");

            if (pedidos == null)
            {
                Console.WriteLine("No se encontraron pedidos.");
                return View(new List<Pedido>());
            }

            return View(pedidos);
        }

        public async Task<IActionResult> Create()
        {
            SetAuthorizationHeader();

            // Obtener todas las tallas para el select
            var tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("tallas");

            // Obtener todos los estados de pedido
            var estadosPedido = await _httpClient.GetFromJsonAsync<List<EstadoPedido>>("estadospedido");

            // Obtener todas las prendas de vestir
            var prendasVestir = await _httpClient.GetFromJsonAsync<List<PrendaVestir>>("prendas");

            // Obtener todos los clientes
            var clientes = await _httpClient.GetFromJsonAsync<List<Cliente>>("clientes");

            // Pasar datos a la vista
            ViewBag.Tallas = tallas ?? new List<Talla>();  // Pasar tallas
            ViewBag.EstadosPedido = estadosPedido ?? new List<EstadoPedido>();  // Pasar estados de pedido
            ViewBag.PrendasVestir = prendasVestir ?? new List<PrendaVestir>();  // Pasar prendas de vestir
            ViewBag.Clientes = clientes ?? new List<Cliente>();  // Pasar clientes

            return View();
        }




        // Método para cargar los ViewBags nuevamente en caso de error
        private async Task LoadViewBags()
        {
            ViewBag.Tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("tallas") ?? new List<Talla>();
            ViewBag.EstadosPedido = await _httpClient.GetFromJsonAsync<List<EstadoPedido>>("estadospedido") ?? new List<EstadoPedido>();
            ViewBag.PrendasVestir = await _httpClient.GetFromJsonAsync<List<PrendaVestir>>("prendas") ?? new List<PrendaVestir>();
            ViewBag.Clientes = await _httpClient.GetFromJsonAsync<List<Cliente>>("clientes") ?? new List<Cliente>();
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.DeleteAsync($"pedidos/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // Manejar el error
                return View("Error", new { message = "Error al eliminar el pedido" });
            }
        }


    }
}