using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;
using System.Net.Http.Json;

namespace MVC_NPANTS.Controllers
{
    public class EstadoPedidoController : Controller
    {
        private readonly HttpClient _httpClient;

        public EstadoPedidoController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CRMAPI");
        }

        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        // GET: EstadoPedido
        public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader();
            var estadosPedido = await _httpClient.GetFromJsonAsync<List<EstadoPedido>>("estadospedido");

            if (estadosPedido == null)
            {
                ViewBag.ErrorMessage = "No se encontraron estados de pedido.";
            }

            return View(estadosPedido);
        }

        // GET: EstadoPedido/Create
        public IActionResult Create()
        {
            SetAuthorizationHeader();
            return View();
        }

        // POST: EstadoPedido/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EstadoPedido estadoPedido)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("estadospedido/create", estadoPedido);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ErrorMessage = "Hubo un error al crear el estado de pedido.";
            return View(estadoPedido);
        }

        // GET: EstadoPedido/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            SetAuthorizationHeader();
            var estadoPedido = await _httpClient.GetFromJsonAsync<EstadoPedido>($"estadospedido/{id}");

            if (estadoPedido == null)
            {
                return NotFound();
            }

            return View(estadoPedido);
        }

        // POST: EstadoPedido/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, EstadoPedido estadoPedido)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"estadospedido/{id}", estadoPedido);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ErrorMessage = "Hubo un error al editar el estado de pedido.";
            return View(estadoPedido);
        }

        // GET: EstadoPedido/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            SetAuthorizationHeader();
            var estadoPedido = await _httpClient.GetFromJsonAsync<EstadoPedido>($"estadospedido/{id}");

            if (estadoPedido == null)
            {
                return NotFound();
            }

            return View(estadoPedido);
        }

        // POST: EstadoPedido/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"estadospedido/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ErrorMessage = "Hubo un error al eliminar el estado de pedido.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        // GET: EstadoPedido/Details/5
        public async Task<IActionResult> Details(long id)
        {
            SetAuthorizationHeader();
            var estadoPedido = await _httpClient.GetFromJsonAsync<EstadoPedido>($"estadospedido/{id}");

            if (estadoPedido == null)
            {
                return NotFound();
            }

            return View(estadoPedido);
        }
    }
}
