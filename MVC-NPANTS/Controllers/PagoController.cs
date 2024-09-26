using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index()
        {
            
            SetAuthorizationHeader();
            var pagos = await _httpClient.GetFromJsonAsync<List<Pago>>("pagos");

            if (pagos == null || !pagos.Any())
            {
                ViewBag.Message = "No se encontraron registros de pagos.";
            }

            return View(pagos);
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
            ViewBag.MetodosPago = metodosPago;

             var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("pedidos");  
            ViewBag.Pedidos = pedidos;

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

                else Console.WriteLine("dio error");

                return View();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View();
            }
        }

        // GET: PagoController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            var pedidoEdit = await _httpClient.GetFromJsonAsync<Pago>($"pagos/{id}");

            if (pedidoEdit == null)
            {
                Console.WriteLine("no se encontro el id");
            }

            var metodosPago = await _httpClient.GetFromJsonAsync<List<MetodoPago>>("metodospago");
            ViewBag.MetodosPago = metodosPago;

            var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("pedidos");
            ViewBag.Pedidos = pedidos;

            return View(pedidoEdit);
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
                    return View();
                }
            }
            catch
            {
                return View();
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
