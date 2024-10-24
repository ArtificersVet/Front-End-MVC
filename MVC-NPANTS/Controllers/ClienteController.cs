using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_NPANTS.Models;

namespace MVC_NPANTS.Controllers
{
    public class ClienteController : Controller
    {
        private readonly HttpClient _httpClient;

        public ClienteController(IHttpClientFactory httpClientFactory)
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

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            SetAuthorizationHeader();

            var pagedResponse = await _httpClient.GetFromJsonAsync<PagedClientesResponse>($"clientes?page={page}");

            var viewModel = new PagedClientesResponse
            {
                Clientes = pagedResponse?.Clientes,
                CurrentPage = pagedResponse?.CurrentPage ?? 1,
                TotalPages = pagedResponse?.TotalPages ?? 1,
                PageSize = pagedResponse?.PageSize ?? 10
            };

            return View(viewModel);
        }



        // GET: ClienteController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            SetAuthorizationHeader();
            var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"clientes/{id}");

            if (cliente == null)
            {
                return NotFound();
            }

            if (cliente.TipoclienteId.HasValue)
            {
                var tipoCliente = await _httpClient.GetFromJsonAsync<TipoCliente>($"tipoclientes/{cliente.TipoclienteId.Value}");
                cliente.Tipocliente = tipoCliente;
            }

            return View(cliente);
        }

        // GET: ClienteController/Create
        public async Task<IActionResult> Create()
        {
            SetAuthorizationHeader();

            var tiposClientes = await _httpClient.GetFromJsonAsync<List<TipoCliente>>("tipoclientes");
            if (tiposClientes == null || !tiposClientes.Any())
            {
                ModelState.AddModelError("", "No se encontraron tipos de clientes.");
                ViewBag.TiposClientes = new SelectList(Enumerable.Empty<TipoCliente>(), "Id", "Nombre");
            }
            else
            {
                ViewBag.TiposClientes = new SelectList(tiposClientes, "Id", "Nombre");
            }

            return View();
        }

        // POST: ClienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            SetAuthorizationHeader();

            if (ModelState.IsValid)
            {
                if (cliente.TipoclienteId == null)
                {
                    ModelState.AddModelError("", "Debe seleccionar un tipo de cliente.");
                }
                else
                {
                    var response = await _httpClient.PostAsJsonAsync("clientes/create", cliente);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("", "No se pudo crear el cliente.");
                }
            }


            var tiposClientes = await _httpClient.GetFromJsonAsync<List<TipoCliente>>("tipoclientes");
            ViewBag.TiposClientes = new SelectList(tiposClientes, "Id", "Nombre");

            return View(cliente);
        }

        // GET: ClienteController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
             var clienteResponse = await _httpClient.GetFromJsonAsync<Cliente>($"clientes/{id}");
            if (clienteResponse == null)
            {
                return NotFound();
            }

             try
            {
                TipoClientesResponse tiposClientesResponse = await _httpClient.GetFromJsonAsync<TipoClientesResponse>("tipoclientes");
                if (tiposClientesResponse?.TipoClientes == null)
                {
                    return NotFound();
                }

                ViewBag.TiposClientes = tiposClientesResponse.TipoClientes;
            }
            catch (JsonException ex)
            {
                 Console.WriteLine($"Error de deserialización: {ex.Message}");
                return BadRequest("Error al obtener tipos de clientes");
            }

            return View(clienteResponse);

        }

        // POST: ClienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            SetAuthorizationHeader();
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"clientes/{cliente.Id}", cliente);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "No se pudo actualizar el cliente.");
            }


            var tiposClientes = await _httpClient.GetFromJsonAsync<List<TipoCliente>>("tipoclientes");
            ViewBag.TiposClientes = new SelectList(tiposClientes, "Id", "Nombre", cliente.TipoclienteId);

            return View(cliente);
        }

        // GET: ClienteController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {

            SetAuthorizationHeader();

            var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"clientes/{id}");

            if (cliente == null)
            {
                return NotFound();
            }
            if (cliente.TipoclienteId.HasValue)
            {
                var tipoCliente = await _httpClient.GetFromJsonAsync<TipoCliente>($"tipoclientes/{cliente.TipoclienteId.Value}");
                cliente.Tipocliente = tipoCliente;
            }
            return View(cliente);

        }

        // POST: ClienteController/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _httpClient.DeleteAsync($"clientes/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "No se pudo eliminar el cliente.");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Error al eliminar el cliente.");
                return RedirectToAction(nameof(Index));
            }



        }
    }

    public class TipoClientesResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<TipoCliente> TipoClientes { get; set; }
    }

}
