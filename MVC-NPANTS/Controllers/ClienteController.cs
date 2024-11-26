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

        
        public async Task<IActionResult> Index(int page = 1, string searchString = null)
        {
            SetAuthorizationHeader();

            var viewModel = new PagedClientesResponse();

            try
            {
                // Obtener todos los clientes paginados desde la API
                var pagedResponse = await _httpClient.GetFromJsonAsync<PagedClientesResponse>($"clientes?page={page}");

                if (pagedResponse != null)
                {
                    var clientes = pagedResponse.Clientes;

                    // Verificar si hay un término de búsqueda
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        clientes = clientes.Where(tc =>
                            tc.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                        // Ajustar los datos de la vista con los resultados filtrados
                        viewModel.Clientes = clientes;
                        viewModel.TotalItems = clientes.Count;
                        viewModel.PageSize = pagedResponse.PageSize;
                        viewModel.TotalPages = (int)Math.Ceiling((double)clientes.Count / pagedResponse.PageSize);
                        viewModel.CurrentPage = 1; // Reiniciar a la primera página para la búsqueda
                    }
                    else
                    {
                        // Si no hay búsqueda, usar los resultados originales
                        viewModel = pagedResponse;
                    }

                    ViewBag.SearchString = searchString; // Mantener el término de búsqueda en la vista
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Si la API devuelve un 404, mostrar una lista vacía
                viewModel.Clientes = new List<Cliente>();
                viewModel.CurrentPage = 1;
                viewModel.TotalPages = 1;
                viewModel.PageSize = 10;
                viewModel.TotalItems = 0;
            }
            catch (Exception)
            {
                // Manejar errores genéricos
                viewModel.Clientes = new List<Cliente>();
            }

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

            var tiposClientes = await _httpClient.GetFromJsonAsync<PagedTipoClienteResponse>("tipoclientes?page=1&pageSize=10");

            if (tiposClientes == null || !tiposClientes.TipoClientes.Any())
            {
                ModelState.AddModelError("", "No se encontraron tipos de clientes.");
                ViewBag.TiposClientes = new SelectList(Enumerable.Empty<TipoCliente>(), "Id", "Nombre");
            }
            else
            {
                ViewBag.TiposClientes = new SelectList(tiposClientes.TipoClientes, "Id", "Nombre");
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

        var tiposClientes = await _httpClient.GetFromJsonAsync<PagedTipoClienteResponse>("tipoclientes");
        ViewBag.TiposClientes = new SelectList(tiposClientes.TipoClientes, "Id", "Nombre");

        return View(cliente);
    }


        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();

            // Cargar la lista de tipos de clientes
            var tiposClientes = await _httpClient.GetFromJsonAsync<PagedTipoClienteResponse>("tipoclientes");

            if (tiposClientes == null || !tiposClientes.TipoClientes.Any())
            {
                ModelState.AddModelError("", "No se encontraron tipos de clientes.");
                ViewBag.TiposClientes = new SelectList(Enumerable.Empty<TipoCliente>(), "Id", "Nombre");
            }
            else
            {
                ViewBag.TiposClientes = new SelectList(tiposClientes.TipoClientes, "Id", "Nombre");
            }

            // Obtener el cliente a editar
            var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"clientes/{id}");
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
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
                    var response = await _httpClient.PutAsJsonAsync($"clientes/{id}", cliente);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("", "No se pudo actualizar el cliente.");
                }
            }

            // Cargar la lista de tipos de clientes nuevamente si la actualización falla
            var tiposClientes = await _httpClient.GetFromJsonAsync<List<TipoCliente>>("tipoclientes");
            ViewBag.TiposClientes = new SelectList(tiposClientes, "Id", "Nombre");

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
