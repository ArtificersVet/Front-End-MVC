using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_NPANTS.Models;

namespace MVC_NPANTS.Controllers
{
    public class PagoController : Controller
    {

        private readonly HttpClient _httpClient;

        public PagoController(IHttpClientFactory httpClientFactory)
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
        // GET: PagoController
        public async Task<IActionResult> Index(int page = 1)
        {
            
            SetAuthorizationHeader();

            var pagedResponse = await _httpClient.GetFromJsonAsync<PagePagoResponse>($"pagos?page={page}");

            var viewModel = new PagePagoResponse
            {
                Pagos = pagedResponse?.Pagos,
                CurrentPage = pagedResponse?.CurrentPage ?? 1,
                TotalPages = pagedResponse?.TotalPages ?? 1,
                PageSize = pagedResponse?.PageSize ?? 10
            };

            return View(viewModel);
        }

        // GET: PagoController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            SetAuthorizationHeader();
            var pagos = await _httpClient.GetFromJsonAsync<Pago>($"pagos/{id}");

            if (pagos == null)
            {
                Console.WriteLine("no se encontro el pago");
            }
            return View(pagos);
        }

        // GET: PagoController/Create
        public async Task<IActionResult> Create()
        {
            SetAuthorizationHeader();
            var metodosPago = await _httpClient.GetFromJsonAsync<List<MetodoPago>>("metodospago");
            ViewBag.MetodosPago = new SelectList(metodosPago, "Id", "Nombre");
            var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("pedidos");
            ViewBag.Pedidos = new SelectList(pedidos, "Id", "Id");  // Asumiendo que quieres mostrar el Id del pedido
            return View();
        }

        // POST: PagoController/Create
        [HttpPost]
        public async Task<IActionResult> Create(Pago pagoOBJ)
        {
            SetAuthorizationHeader();
            try
            {
                var pagoCreate = await _httpClient.PostAsJsonAsync("pagos/create", pagoOBJ);
                if (pagoCreate.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
                else
                {
                    // Si hay un error, volvemos a cargar las listas para el formulario
                    var metodosPago = await _httpClient.GetFromJsonAsync<List<MetodoPago>>("metodospago");
                    ViewBag.MetodosPago = new SelectList(metodosPago, "Id", "Nombre");
                    var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("pedidos");
                    ViewBag.Pedidos = new SelectList(pedidos, "Id", "Id");
                    return View(pagoOBJ);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // Si hay una excepción, también volvemos a cargar las listas
                var metodosPago = await _httpClient.GetFromJsonAsync<List<MetodoPago>>("metodospago");
                ViewBag.MetodosPago = new SelectList(metodosPago, "Id", "Nombre");
                var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("pedidos");
                ViewBag.Pedidos = new SelectList(pedidos, "Id", "Id");
                return View(pagoOBJ);
            }
        }

        // GET: PagoController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            var pagoEdit = await _httpClient.GetFromJsonAsync<Pago>($"pagos/{id}");
            if (pagoEdit == null)
            {
                return NotFound();
            }
            var metodosPago = await _httpClient.GetFromJsonAsync<List<MetodoPago>>("metodospago");
            ViewBag.MetodosPago = new SelectList(metodosPago, "Id", "Nombre", pagoEdit.MetodoPagoId);
            var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("pedidos");
            ViewBag.Pedidos = new SelectList(pedidos, "Id", "Id", pagoEdit.PedidoId);
            return View(pagoEdit);
        }

        // POST: PagoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pago pago)
        {
            SetAuthorizationHeader();
            try
            {
                var pagosEdit = await _httpClient.PutAsJsonAsync($"pagos/{id}", pago);
                if (pagosEdit.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Si hay un error, volvemos a cargar las listas para el formulario
                    var metodosPago = await _httpClient.GetFromJsonAsync<List<MetodoPago>>("metodospago");
                    ViewBag.MetodosPago = new SelectList(metodosPago, "Id", "Nombre", pago.MetodoPagoId);
                    var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("pedidos");
                    ViewBag.Pedidos = new SelectList(pedidos, "Id", "Id", pago.PedidoId);
                    return View(pago);
                }
            }
            catch
            {
                // Si hay una excepción, también volvemos a cargar las listas
                var metodosPago = await _httpClient.GetFromJsonAsync<List<MetodoPago>>("metodospago");
                ViewBag.MetodosPago = new SelectList(metodosPago, "Id", "Nombre", pago.MetodoPagoId);
                var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("pedidos");
                ViewBag.Pedidos = new SelectList(pedidos, "Id", "Id", pago.PedidoId);
                return View(pago);
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            try
            {
                var pagosDelete = await _httpClient.DeleteAsync($"pagos/{id}");

                if (pagosDelete == null)
                {
                    Console.WriteLine("no se encontro el id");
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
