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

            // Realiza la solicitud a la API
            var response = await _httpClient.GetAsync($"clientes/{id}"); // Asegúrate de que esta URL sea correcta

            // Verifica si la respuesta es exitosa
            if (response.IsSuccessStatusCode)
            {
                // Intenta deserializar la respuesta
                var pagedResponse = await response.Content.ReadFromJsonAsync<PagedClientesResponse>();

                // Verifica si la deserialización fue exitosa
                if (pagedResponse == null)
                {
                    return StatusCode(500, "Error al deserializar la respuesta del servidor.");
                }

                // Verifica si la lista de clientes no es nula y está inicializada
                if (pagedResponse.Clientes == null)
                {
                    return StatusCode(500, "La lista de clientes no está disponible.");
                }

                // Busca el cliente con el ID especificado
                var cliente = pagedResponse.Clientes.FirstOrDefault(c => c.Id == id);

                if (cliente != null)
                {
                    // Devuelve la vista con el cliente encontrado
                    return View(cliente);
                }
                else
                {
                    // Manejar el caso en que no se encuentra el cliente
                    return NotFound($"Cliente con ID {id} no encontrado.");
                }
            }
            else
            {
                // Manejar el error de respuesta
                return StatusCode((int)response.StatusCode, "Error al obtener el cliente.");
            }
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

    public class EditClienteViewModel
    {
        public Cliente Cliente { get; set; }
        public IEnumerable<SelectListItem> TiposClientes { get; set; } // Asegúrate de que sea IEnumerable<SelectListItem>
    }

}
